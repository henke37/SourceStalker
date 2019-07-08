using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Henke37.IOUtils;
using ICSharpCode.SharpZipLib.BZip2;

//https://developer.valvesoftware.com/wiki/Server_queries

namespace Henke37.Valve.Source.ServerQuery {
    public class ServerStatus {
        private string _hostname;
        private IPHostEntry resolvedHost;
        public short Port=DefaultPort;

        private Socket client;

		private bool waitingForInfo;
		private bool waitingForPlayers;
		private bool waitingForRules;

		public A2S_INFO_Response Info;
        public A2S_RULES_Response Rules;
        public A2S_PLAYER_Response Players;

		private DateTime QueryTime;
        private DateTime responseTime;

		public TimeSpan PingTime { get => responseTime - QueryTime; }

        public enum QueryState {
            Invalid,
			HostNameResolving,
            HostNameResolved,
            HostNameInvalid,
            QuerySent,
            AnswerReceived,
            TimeOut
        }

        private QueryState _state = QueryState.Invalid;

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
                return $"{_hostname}:{Port}";
            }
            set {
                var parts = value.Split(new[] { ':' });
                if(parts.Length != 2) {
                    Port = DefaultPort;
                    HostName = value;
                } else {
                    Port = short.Parse(parts[1]);
                    HostName = parts[0];
                }
            }
        }

        public class BadAddressException : Exception {
            public BadAddressException(string message) : base(message) {
            }
        }

        public QueryState State {
            get => _state;
            private set {
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
				client.Connect(addr, Port);
				State = QueryState.HostNameResolved;
				return;
			}

            State = QueryState.HostNameResolving;
            try {
                resolvedHost = await Dns.GetHostEntryAsync(_hostname);
				if(resolvedHost.AddressList.Length<1) {
					State = QueryState.HostNameInvalid;
					return;
				}
                client.Connect(resolvedHost.AddressList[0], Port);
                State = QueryState.HostNameResolved;
            } catch(SocketException err) {
                if(err.ErrorCode != (int)SocketError.HostNotFound) throw;
                State = QueryState.HostNameInvalid;
            }
        }

        public async Task Update() {
            State = QueryState.QuerySent;
            try {

				QueryTime = DateTime.Now;
				waitingForInfo = true;
                SendQuery(new A2S_INFO_Request());

				waitingForRules = true;
                SendQuery(new A2S_RULES_Query());

                while(State==QueryState.QuerySent) {
					var response = await AwaitResponse();
					ResponseReceived(response);
				}

            } catch(SocketException err) {
                if(err.ErrorCode != (int)SocketError.TimedOut) throw;
                State = QueryState.TimeOut;
            }
        }

		private void ResponseReceived(BaseResponse response) {
			switch(response) {
				case A2S_INFO_Response infoR:
					Info = infoR;
					responseTime = DateTime.Now;
					waitingForInfo = false;
					break;

				case A2S_RULES_Response rulesR:
					Rules = rulesR;
					waitingForRules = false;

					waitingForPlayers = true;
					SendQuery(new A2S_PLAYER_Query());
					break;

				case A2S_PLAYER_Response playersR:
					Players = playersR;
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
				State = QueryState.AnswerReceived;
			}
		}


		public bool IsReadyForUpdate {
            get {
                switch(_state) {
                    case QueryState.AnswerReceived: return true;
                    case QueryState.HostNameResolved: return true;
                    case QueryState.TimeOut: return true;
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


    }


}
