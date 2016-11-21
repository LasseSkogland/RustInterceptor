using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class Tick {

		internal PlayerTick protobuf;
		public uint ActiveItem { get { return protobuf.activeItem; } }
		public UnityEngine.Vector3 EyePosition { get { return protobuf.eyePos;  } }
		public UnityEngine.Vector3 Position { get { return protobuf.position; } }
		public Input_Message inputState;
		public Input_Message InputState { get{ return inputState; } }
		public Model_State modelState;
		public Model_State ModelState { get { return modelState;  } }
		
		

		public Tick(Packet p) {
			protobuf = PlayerTick.Deserialize(p);
			modelState = new Model_State(protobuf.modelState);
			inputState = new Input_Message(protobuf.inputState);
		}

	}
}
