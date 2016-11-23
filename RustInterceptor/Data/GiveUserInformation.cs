namespace Rust_Interceptor.Data {
	public class GiveUserInformation {
		internal byte protocolVersion;
		public byte ProtocolVersion { get{ return protocolVersion; } }
		internal ulong steamId;
		public ulong SteamID { get { return steamId; } }
		internal uint protocol;
		public uint Protocol { get { return protocol; } }
		internal string osName;
		public string OSName { get { return osName; } }
		internal string steamName;
		public string SteamName { get { return steamName; } }
		internal string branch;
		public string Branch { get { return branch; } }


		public GiveUserInformation(Packet p) {
			protocolVersion = p.UInt8();
			steamId = p.UInt64();
			protocol = p.UInt32();
			osName = p.String();
			steamName = p.String();
			branch = p.String();
		}

	}
}
