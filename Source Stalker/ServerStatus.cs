using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

//https://developer.valvesoftware.com/wiki/Server_queries

namespace Source_Stalker {
    class ServerStatus {
        private string _hostname;

        private enum State {
            INVALID,
            UNRESOLVED,
            QUERY_SENT,
            PARTIAL_ANSWER_RECEIVED,
            ANSWER_RECEIVED
        }

        private State state=State.INVALID;

        public ServerStatus() {

        }

        public string HostName {
            get => _hostname;
            set {
                _hostname = value;
                state = State.UNRESOLVED;
                resolveHostname();
            }
        }

        private void resolveHostname() {
            throw new NotImplementedException();
        }

        private void SendQuery(UdpClient client, BaseQuery q) {
            byte[] ba ;
            using(MemoryStream ms = new MemoryStream()) {
                q.BuildQuery(ms);
                ba= ms.ToArray();
            }

            client.Send(ba, ba.Length);
        }

        private class ResponseReader {
            private byte[][] responses;

            private void ReadResponseDatagram(BinaryReader r) {
                uint Header = r.ReadUInt32();
                if(Header==0xFFFFFFFF) {
                    ParseResponseMessage(r);
                } else if(Header==0xFFFFFFFE) {
                    ParseSplitResponse(r);
                }
            }

            private BaseResponse ParseResponseMessage(BinaryReader r) {
                byte messageType = r.ReadByte();
                switch(messageType) {
                    case 0x49: return new A2S_INFO_Response(r);

                    case 0x6D://obsolete goldsource info request response
                    case 0x41://challenge number request response
                    case 0x44://player info request response
                    case 0x45://rules request response
                    case 0x6A://obsolete ping request response
                    default:
                        throw new NotImplementedException();
                }
            }

            private void ParseSplitResponse(BinaryReader r) {
                throw new NotImplementedException();
            }
        }

        private class A2S_INFO_Request : BaseQuery {

            private const byte Header = 0x54;
            private const string Payload = "Source Engine Query";

            public override void BuildQuery(Stream s) {
                using(BinaryWriter w = new BinaryWriter(s,Encoding.UTF8)) {
                    w.Write(Header);
                    w.Write(Payload.ToArray());
                    w.Write((byte)0);
                }
            }
        }

        private class A2S_INFO_Response : BaseResponse {
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

            public A2S_INFO_Response(Stream s) {
                Read(s);
            }
            public A2S_INFO_Response(BinaryReader r) {
                Read(r);
            }

            private void Read(Stream s) {
                using(BinaryReader r=new BinaryReader(s,Encoding.UTF8)) {
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
                    ServerSteamId=r.ReadUInt64();
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
                DEDICATED='d',
                LOCAL='l',
                PROXY='p'
            }

            public enum EnvironmentEnum {
                LINUX='l',
                WINDOWS='w',
                MAC='m'
            }
        }

        private abstract class BaseQuery {
            abstract public void BuildQuery(Stream s);
        }
        private abstract class BaseResponse {
        }
    }


}
