using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class RPCMessage {

		internal uint uid;
		public uint UID { get { return uid; } }
		internal uint nameId;
		public uint NameID { get { return nameId; } }
		internal ulong sourceConnection;
		public ulong SourceConnection { get { return sourceConnection; } }

		public RPCMessage(Packet p) {
			uid = p.UInt32();
			nameId = p.UInt32();
			sourceConnection = p.UInt64();
		}
	}
}
