using Henke37.IOUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Henke37.Valve.Source.ServerQuery {

	internal class A2S_INFO_Request : BaseQuery {

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
		public byte ProtocolVersion;
		public string ServerName;
		public string Map;
		public string Folder;
		public string Game;
		public short Id;
		public byte PlayerCount;
		public byte MaxPlayerCount;
		public byte BotCount;
		public ServerTypeEnum ServerType;
		public Platform Environment;
		public bool PasswordRequired;
		public bool VACEnabled;
		public string GameVersion;
		public ushort PortNumber;
		public ushort SourceTVPortNumber;
		public string SourceTVHostName;
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

		internal void Read(Stream s) {
			using(BinaryReader r = new BinaryReader(s, Encoding.UTF8)) {
				Read(r);
			}
		}

		internal void Read(BinaryReader r) {
			ProtocolVersion = r.ReadByte();
			ServerName = r.ReadNullTerminatedUTF8String();
			Map = r.ReadNullTerminatedUTF8String();
			Folder = r.ReadNullTerminatedUTF8String();
			Game = r.ReadNullTerminatedUTF8String();
			Id = r.ReadInt16();
			PlayerCount = r.ReadByte();
			MaxPlayerCount = r.ReadByte();
			BotCount = r.ReadByte();
			ServerType = ParseServerType(r.ReadByte());
			Environment = ParseEnvironment(r.ReadByte());
			PasswordRequired = r.ReadBoolean();
			VACEnabled = r.ReadBoolean();
			GameVersion = r.ReadNullTerminatedUTF8String();
			byte EDF = r.ReadByte();
			if((EDF & HasPortNumber) == HasPortNumber) {
				PortNumber = r.ReadUInt16();
			}
			if((EDF & HasSteamId) == HasSteamId) {
				ServerSteamId = r.ReadUInt64();
			}
			if((EDF & HasSTV) == HasSTV) {
				SourceTVPortNumber = r.ReadUInt16();
				SourceTVHostName = r.ReadNullTerminatedUTF8String();
			}
			if((EDF & HasTags) == HasTags) {
				Tags = r.ReadNullTerminatedUTF8String();
			}
		}

		private static ServerTypeEnum ParseServerType(byte v) {
			switch((char)v) {
				case 'd': return ServerTypeEnum.Dedicated;
				case 'l': return ServerTypeEnum.Local;
				case 'p': return ServerTypeEnum.Proxy;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private Platform ParseEnvironment(byte v) {
			switch((char)v) {
				case 'l': return Platform.Linux;
				case 'w': return Platform.Windows;
				case 'm':
				case 'o': return Platform.Mac;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public enum ServerTypeEnum {
			Dedicated = 'd',
			Local = 'l',
			Proxy = 'p'
		}

		public enum Platform {
			Linux = 'l',
			Windows = 'w',
			Mac = 'm'
		}
	}

	internal class BaseChallengeQuery : BaseQuery {
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

	internal class A2S_RULES_Query : BaseChallengeQuery {
		private const byte Header = 0x56;
		public A2S_RULES_Query() : base(Header) { }
		public A2S_RULES_Query(uint challenge) : base(Header, challenge) { }
	}

	public class A2S_RULES_Response : BaseResponse, IReadOnlyDictionary<string, string> {

		public Dictionary<string, string> Rules;

		public A2S_RULES_Response(BinaryReader r) {
			Rules = new Dictionary<string, string>();

			ushort ruleCount = r.ReadUInt16();
			for(ushort ruleIndex = 0; ruleIndex < ruleCount; ++ruleIndex) {
				string key = r.ReadNullTerminatedUTF8String();
				string value = r.ReadNullTerminatedUTF8String();
				Rules.Add(key, value);
			}
		}

		public bool TryGetCVar(string varName, out string varValue) => Rules.TryGetValue(varName, out varValue);

		public bool ContainsKey(string key) {
			return Rules.ContainsKey(key);
		}

		public bool TryGetValue(string key, out string value) {
			return Rules.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
			return Rules.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Rules.GetEnumerator();
		}

		public string this[string varName] {
			get { return Rules[varName]; }
		}

		public string NextMap {
			get {
				if(Rules.TryGetValue("sm_nextmap", out string smNextMap)) return smNextMap;
				if(Rules.TryGetValue("nextlevel", out string nextMap)) return nextMap;
				return null;
			}
		}

		public IEnumerable<string> Keys => Rules.Keys;

		public IEnumerable<string> Values => Rules.Values;

		public int Count => Rules.Count;
	}

	internal class A2S_PLAYER_Query : BaseChallengeQuery {
		private const byte Header = 0x55;
		public A2S_PLAYER_Query() : base(Header) { }
		public A2S_PLAYER_Query(uint challenge) : base(Header, challenge) { }
	}

	public class A2S_PLAYER_Response : BaseResponse, IReadOnlyList<A2S_PLAYER_Response.Player> {

		public Player[] Players;

		public A2S_PLAYER_Response(BinaryReader r) {
			byte playerCount = r.ReadByte();

			Players = new Player[playerCount];

			for(byte playerIndex = 0; playerIndex < playerCount; ++playerIndex) {
				byte index = r.ReadByte();
				string name = r.ReadNullTerminatedUTF8String();
				int score = r.ReadInt32();
				float playtime = r.ReadSingle();

				Players[playerIndex] = new Player(name, score, playtime);
			}
		}

		public Player this[int index] => Players[index];

		public int Count => Players.Length;

		public IEnumerator<Player> GetEnumerator() {
			return ((IEnumerable<Player>)Players).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Players.GetEnumerator();
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

	internal class A2S_SERVERQUERY_GETCHALLENGE_Response : BaseResponse {
		public uint Challenge;
		public A2S_SERVERQUERY_GETCHALLENGE_Response(BinaryReader r) {
			Challenge = r.ReadUInt32();
		}
	}

	internal abstract class BaseQuery {
		abstract public void BuildQuery(Stream s);
	}
	public abstract class BaseResponse {
	}
}