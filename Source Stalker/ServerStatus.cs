using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ICSharpCode.SharpZipLib.BZip2;

//https://developer.valvesoftware.com/wiki/Server_queries

namespace Source_Stalker {
    class ServerStatus {
        private string _hostname;
        private IPHostEntry resolvedHost;
        public short port=DefaultPort;

        private Socket client;

		private bool waitingForInfo;
		private bool waitingForPlayers;
		private bool waitingForRules;

		public A2S_INFO_Response info;
        public A2S_RULES_Response rules;
        public A2S_PLAYER_Response players;

		public DateTime queryTime;
        private DateTime responseTime;

		public TimeSpan PingTime { get => responseTime - queryTime; }

        public enum StateEnum {
            INVALID,
            HOSTNAME_UNRESOLVED,
            HOSTNAME_RESOLVED,
            HOSTNAME_INVALID,
            QUERY_SENT,
            ANSWER_RECEIVED,
            TIME_OUT
        }

        private StateEnum _state = StateEnum.INVALID;

        public event Action<ServerStatus> StateChanged;


        private static readonly byte[] QueryPrefix = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        private const short DefaultPort = 27015;

        public ServerStatus() {
            client = new Socket(SocketType.Dgram, ProtocolType.Udp);
        }

        public int Timeout { get => client.ReceiveTimeout; set => client.ReceiveTimeout = value; }

        public string HostName {
            get => _hostname;
            set {
                _hostname = value;
                resolveHostname();
            }
        }

        public string Address {
            get {
                if(_hostname == null) return null;
                return $"{_hostname}:{port}";
            }
            set {
                var parts = value.Split(new[] { ':' });
                if(parts.Length != 2) {
                    port = DefaultPort;
                    HostName = value;
                } else {
                    port = short.Parse(parts[1]);
                    HostName = parts[0];
                }
            }
        }

        public class BadAddressException : Exception {
            public BadAddressException(string message) : base(message) {
            }
        }

        internal StateEnum State {
            get => _state;
            set {
                if(_state == value) return;
                _state = value;
                if(StateChanged != null) {
                    StateChanged.Invoke(this);
                }
            }
        }

        private async void resolveHostname() {
            resolvedHost = null;

			if(IPAddress.TryParse(_hostname,out IPAddress addr)) {
				client.Connect(addr, port);
				State = StateEnum.HOSTNAME_RESOLVED;
				return;
			}

            State = StateEnum.HOSTNAME_UNRESOLVED;
            try {
                resolvedHost = await Dns.GetHostEntryAsync(_hostname);
				if(resolvedHost.AddressList.Length<1) {
					State = StateEnum.HOSTNAME_INVALID;
					return;
				}
                client.Connect(resolvedHost.AddressList[0], port);
                State = StateEnum.HOSTNAME_RESOLVED;
            } catch(SocketException err) {
                if(err.ErrorCode != (int)SocketError.HostNotFound) throw;
                State = StateEnum.HOSTNAME_INVALID;
            }
        }

        public async Task Update() {
            State = StateEnum.QUERY_SENT;
            try {

				queryTime = DateTime.Now;
				waitingForInfo = true;
                SendQuery(new A2S_INFO_Request());

				waitingForRules = true;
                SendQuery(new A2S_RULES_Query());

				waitingForPlayers = true;
				SendQuery(new A2S_PLAYER_Query());

                while(State==StateEnum.QUERY_SENT) {
					var response = await AwaitResponse();
					ResponseReceived(response);
				}

            } catch(SocketException err) {
                if(err.ErrorCode != (int)SocketError.TimedOut) throw;
                State = StateEnum.TIME_OUT;
            }
        }

		private void ResponseReceived(BaseResponse response) {
			switch(response) {
				case A2S_INFO_Response infoR:
					info = infoR;
					responseTime = DateTime.Now;
					waitingForInfo = false;
					break;

				case A2S_RULES_Response rulesR:
					rules = rulesR;
					waitingForRules = false;
					break;

				case A2S_PLAYER_Response playersR:
					players = playersR;
					waitingForPlayers = false;
					break;

				case A2S_SERVERQUERY_GETCHALLENGE_Response challenge:
					if(waitingForRules) {
						SendQuery(new A2S_RULES_Query(challenge.Challenge));
					} else if(waitingForPlayers) {
						SendQuery(new A2S_PLAYER_Query(challenge.Challenge));
					}

					break;
			}

			if(!(waitingForInfo || waitingForRules || waitingForPlayers)) {
				State = StateEnum.ANSWER_RECEIVED;
			}
		}


		public bool IsReadyForUpdate {
            get {
                switch(_state) {
                    case StateEnum.ANSWER_RECEIVED: return true;
                    case StateEnum.HOSTNAME_RESOLVED: return true;
                    case StateEnum.TIME_OUT: return true;
                }
                return false;
            }
        }

        private void SendQuery(BaseQuery q) {
			byte[] ba;
			using(MemoryStream ms = new MemoryStream()) {
				ms.Write(QueryPrefix, 0, QueryPrefix.Length);
				q.BuildQuery(ms);
				ba = ms.ToArray();
			}

			client.Send(ba);
		}

		private async Task<BaseResponse> AwaitResponse() {
			var reader = new ResponseReader(client);
			return await reader.ReadResponse();
		}

        private class ResponseReader {
            private byte[][] responses;

            private uint ResponseID;

            private Socket client;

            public ResponseReader(Socket client) {
                this.client = client;
            }

            private BaseResponse ReadResponseDatagram(byte[] ba) {
                return ReadResponseDatagram(new BinaryReader(new MemoryStream(ba)));
            }

            private BaseResponse ReadResponseDatagram(BinaryReader r) {
                uint Header = r.ReadUInt32();
                if(Header == 0xFFFFFFFF) {
                    return ParseResponseMessage(r);
                } else if(Header == 0xFFFFFFFE) {
                    return ParseSplitResponse(r);
                }
                throw new NotImplementedException();
            }

            private BaseResponse ParseResponseMessage(BinaryReader r) {
                byte messageType = r.ReadByte();

                switch(messageType) {
                    case 0x49: return new A2S_INFO_Response(r);
                    case 0x41: return new A2S_SERVERQUERY_GETCHALLENGE_Response(r);
                    case 0x45: return new A2S_RULES_Response(r);
                    case 0x44: return new A2S_PLAYER_Response(r);

                    case 0x6D://obsolete goldsource info request response
                    case 0x6A://obsolete ping request response
                    default:
                        throw new NotImplementedException();
                }
            }

            private BaseResponse ParseSplitResponse(BinaryReader r) {
                uint ID = r.ReadUInt32();
                byte total = r.ReadByte();
                byte number = r.ReadByte();
                ushort maxPacketSize = r.ReadUInt16();

                if(responses == null) {
                    responses = new byte[total][];
                    ResponseID = ID;
                }
                if(ID != ResponseID) {
                    return null;//bogus response
                }
                responses[number] = r.ReadRemainingBytes();

                int totalSize = 0;
                for(byte responseIndex = 0; responseIndex < responses.Length; ++responseIndex) {
                    var response = responses[responseIndex];
                    if(response == null) return null;
                    totalSize += response.Length;
                }

                Stream s = new MemoryStream(totalSize);
                for(byte responseIndex = 0; responseIndex < responses.Length; ++responseIndex) {
                    var response = responses[responseIndex];
                    s.Write(response, 0, response.Length);
                }
                s.Position = 0;

                const uint CompressionFlag = 0x80000000;
                if((ID & CompressionFlag) == CompressionFlag) {
                    using(var compr = new BinaryReader(s, Encoding.Default, true)) {
                        uint DecompressedSize = r.ReadUInt32();
                        uint CRC32Val = r.ReadUInt32();
                        s = new BZip2InputStream(s);
                    }
                }

                return ReadResponseDatagram(new BinaryReader(s));
            }

            internal Task<BaseResponse> ReadResponse() {
                var t = new Task<BaseResponse>(delegate {
                    while(true) {
                        byte[] ba = new byte[1400];
                        var ep = client.RemoteEndPoint;
                        int recvCount = client.ReceiveFrom(ba, ref ep);
                        Array.Resize<byte>(ref ba, recvCount);
                        BaseResponse r = ReadResponseDatagram(ba);
                        if(r != null) return r;
                    }
                });
                t.Start();
                return t;
            }
        }

        private class A2S_INFO_Request : BaseQuery {

            private const byte Header = 0x54;
            private const string Payload = "Source Engine Query";

            public override void BuildQuery(Stream s) {
                using(BinaryWriter w = new BinaryWriter(s, Encoding.UTF8)) {
                    w.Write(Header);
                    w.Write(Payload.ToArray());
                    w.Write((byte)0);
                }
            }
        }

        public class A2S_INFO_Response : BaseResponse {
            public byte Protocol;
            public string ServerName;
            public string Map;
            public string Folder;
            public string Game;
            public short ID;
            public byte PlayerCount;
            public byte MaxPlayerCount;
            public byte BotCount;
            public ServerTypeEnum ServerType;
            public EnvironmentEnum Environment;
            public bool PasswordRequired;
            public bool VACEnabled;
            public string GameVersion;
            public ushort PortNumber;
            public ushort STVPortNumber;
            public string STVHostName;
            public string Tags;
            public ulong ServerSteamId;

            private const byte HasPortNumber = 0x80;
            private const byte HasSteamId = 0x10;
            private const byte HasSTV = 0x40;
            private const byte HasTags = 0x20;
            private const byte HasLongGameId = 0x01;

            public byte FreePlayerSlots { get => (byte)(MaxPlayerCount - PlayerCount); }
            public byte RealPlayers { get => (byte)(PlayerCount - BotCount); }

            public A2S_INFO_Response(Stream s) {
                Read(s);
            }
            public A2S_INFO_Response(BinaryReader r) {
                Read(r);
            }

            private void Read(Stream s) {
                using(BinaryReader r = new BinaryReader(s, Encoding.UTF8)) {
                    Read(r);
                }
            }

            private void Read(BinaryReader r) {
                Protocol = r.ReadByte();
                ServerName = r.ReadNullTerminatedString();
                Map = r.ReadNullTerminatedString();
                Folder = r.ReadNullTerminatedString();
                Game = r.ReadNullTerminatedString();
                ID = r.ReadInt16();
                PlayerCount = r.ReadByte();
                MaxPlayerCount = r.ReadByte();
                BotCount = r.ReadByte();
                ServerType = ParseServerType(r.ReadByte());
                Environment = ParseEnvironment(r.ReadByte());
                PasswordRequired = r.ReadBoolean();
                VACEnabled = r.ReadBoolean();
                GameVersion = r.ReadNullTerminatedString();
                byte EDF = r.ReadByte();
                if((EDF & HasPortNumber) == HasPortNumber) {
                    PortNumber = r.ReadUInt16();
                }
                if((EDF & HasSteamId) == HasSteamId) {
                    ServerSteamId = r.ReadUInt64();
                }
                if((EDF & HasSTV) == HasSTV) {
                    STVPortNumber = r.ReadUInt16();
                    STVHostName = r.ReadNullTerminatedString();
                }
                if((EDF & HasTags) == HasTags) {
                    Tags = r.ReadNullTerminatedString();
                }
            }

            private static ServerTypeEnum ParseServerType(byte v) {
                switch((char)v) {
                    case 'd': return ServerTypeEnum.DEDICATED;
                    case 'l': return ServerTypeEnum.LOCAL;
                    case 'p': return ServerTypeEnum.PROXY;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            private EnvironmentEnum ParseEnvironment(byte v) {
                switch((char)v) {
                    case 'l': return EnvironmentEnum.LINUX;
                    case 'w': return EnvironmentEnum.WINDOWS;
                    case 'm':
                    case 'o': return EnvironmentEnum.MAC;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            public enum ServerTypeEnum {
                DEDICATED = 'd',
                LOCAL = 'l',
                PROXY = 'p'
            }

            public enum EnvironmentEnum {
                LINUX = 'l',
                WINDOWS = 'w',
                MAC = 'm'
            }
        }

        private class BaseChallengeQuery : BaseQuery {
            private byte Header;
            public uint Challenge;

            public BaseChallengeQuery(byte Header) {
                this.Header = Header;
                Challenge = 0xFFFFFFFF;
            }

            public BaseChallengeQuery(byte Header, uint challenge) {
                this.Header = Header;
                Challenge = challenge;
            }

            public override void BuildQuery(Stream s) {
                using(BinaryWriter w = new BinaryWriter(s, Encoding.UTF8)) {
                    w.Write(Header);
                    w.Write(Challenge);
                }
            }
        }

        private class A2S_RULES_Query : BaseChallengeQuery {
            private const byte Header = 0x56;
            public A2S_RULES_Query() : base(Header) { }
            public A2S_RULES_Query(uint challenge) : base(Header, challenge) { }
        }

        public class A2S_RULES_Response : BaseResponse {

            public Dictionary<string, string> rules;

            public A2S_RULES_Response(BinaryReader r) {
                rules = new Dictionary<string, string>();

                ushort ruleCount = r.ReadUInt16();
                for(ushort ruleIndex = 0; ruleIndex < ruleCount; ++ruleIndex) {
                    string key = r.ReadNullTerminatedString();
                    string value = r.ReadNullTerminatedString();
                    rules.Add(key, value);
                }
            }
        }

        private class A2S_PLAYER_Query : BaseChallengeQuery {
            private const byte Header = 0x55;
            public A2S_PLAYER_Query() : base(Header) { }
            public A2S_PLAYER_Query(uint challenge) : base(Header, challenge) { }
        }

        public class A2S_PLAYER_Response : BaseResponse {

            public Player[] Players;

            public A2S_PLAYER_Response(BinaryReader r) {
                byte playerCount = r.ReadByte();

                Players = new Player[playerCount];

                for(byte playerIndex = 0; playerIndex < playerCount; ++playerIndex) {
                    byte index = r.ReadByte();
                    string name = r.ReadNullTerminatedString();
                    int score = r.ReadInt32();
                    float playtime = r.ReadSingle();

                    Players[playerIndex] = new Player(name, score, playtime);
                }
            }

            public class Player {
                public string Name;
                public int Score;
                public float Playtime;

                public Player(string name, int score, float playtime) {
                    Name = name;
                    Score = score;
                    Playtime = playtime;
                }

                public override string ToString() {
                    return $"{Name} {Score}";
                }
            }
        }

        public class A2S_SERVERQUERY_GETCHALLENGE_Response : BaseResponse {
            public uint Challenge;
            public A2S_SERVERQUERY_GETCHALLENGE_Response(BinaryReader r) {
                Challenge = r.ReadUInt32();
            }
        }

        private abstract class BaseQuery {
            abstract public void BuildQuery(Stream s);
        }
        public abstract class BaseResponse {
        }
    }


}
