using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class EntityDestroy {

		internal uint uid;
		public uint UID { get{ return uid; } }
		internal byte destroyMode;
		public byte DestroyMode { get{ return destroyMode; } }

		public EntityDestroy(Packet p) {
			uid = p.UInt32();
			destroyMode = p.UInt8();
		}
	}
}
