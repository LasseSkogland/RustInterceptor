using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class Effect {

		internal EffectData protobuf;
		public uint Bone { get { return protobuf.bone; } }
		public uint Entity { get { return protobuf.entity; } }
		public UnityEngine.Vector3 Normal { get { return protobuf.normal; } }
		public int Number { get { return protobuf.number; } }
		public UnityEngine.Vector3 Origin { get { return protobuf.origin; } }
		public uint PooledStringID { get { return protobuf.pooledstringid; } }
		public float Scale { get { return protobuf.scale; } }
		public ulong Source { get { return protobuf.source; } }
		public uint Type { get { return protobuf.type; } }

		public Effect(Packet p) {
			protobuf = EffectData.Deserialize(p);
		}
	}
}
