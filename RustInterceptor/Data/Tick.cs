using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class Tick {

		internal PlayerTick protobuf;
		public uint ActiveItem { get { return protobuf.activeItem; } }
		public UnityEngine.Vector3 EyePosition { get { return protobuf.eyePos;  } }
		public UnityEngine.Vector3 Position { get { return protobuf.position; } }
		public Input_Message inputState;
		public Input_Message InputState { get{ return inputState; } }
		public BaseModelState modelState;
		public BaseModelState ModelState { get { return modelState;  } }
		
		

		public Tick(Packet p) {
			protobuf = PlayerTick.Deserialize(p);
			modelState = new BaseModelState(protobuf.modelState);
			inputState = new Input_Message(protobuf.inputState);
		}

	}
}
