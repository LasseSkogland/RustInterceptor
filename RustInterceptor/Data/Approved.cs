using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class Approved {

		internal ProtoBuf.Approval protobuf;
		public string Checksum { get { return protobuf.checksum; } }
		public string Hostname { get{ return protobuf.hostname; } }
		public uint IpAdress { get { return protobuf.ipaddress; } }
		public string Level { get { return protobuf.level; } }
		public uint LevelSeed { get { return protobuf.levelSeed; } }
		public uint LevelSize { get { return protobuf.levelSize; } }
		public bool Modded { get { return protobuf.modded; } }
		public bool Official { get { return protobuf.official; } }
		public int Port { get { return protobuf.port; } }
		public ulong SteamID { get { return protobuf.steamid; } }
		
		public Approved(Packet p) {
			protobuf = ProtoBuf.Approval.Deserialize(p);
		}

	}
}
