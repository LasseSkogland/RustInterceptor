using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class BaseModelState {
		internal ModelState protobuf;
		public float Aiming { get { return protobuf.aiming; } }
		public bool Ducked { get { return protobuf.ducked; } }
		public int Flags { get { return protobuf.flags; } }
		public bool Flying { get { return protobuf.flying; } }
		public bool Jumped { get { return protobuf.jumped; } }
		public UnityEngine.Vector3 LookDirection { get { return protobuf.lookDir; } }
		public bool OnGround { get { return protobuf.onground; } }
		public bool OnLadder { get { return protobuf.onLadder; } }
		public bool Sleeping { get { return protobuf.sleeping; } }
		public bool Sprinting { get { return protobuf.sprinting; } }
		public float WaterLevel { get { return protobuf.waterLevel; } }

		public BaseModelState(ModelState proto) {
			protobuf = proto;
		}

	}
}
