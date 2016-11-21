using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class Input_Message {
		internal InputMessage protobuf;

		public UnityEngine.Vector3 AimAngles { get { return protobuf.aimAngles; } }
		public int Buttons { get { return protobuf.buttons; } }

		public Input_Message(InputMessage proto) {
			protobuf = proto;
		}
	}
}
