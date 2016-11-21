using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class Tick {

		internal PlayerTick protobuf;
		public PlayerTick Protobuf { get { return protobuf; } }
		public uint ActiveItem { get { return protobuf.activeItem; } }
		public UnityEngine.Vector3 EyePosition { get { return protobuf.eyePos;  } }
		public UnityEngine.Vector3 Position { get { return protobuf.position; } }
		public InputMessage InputState { get{ return protobuf.inputState; } }
		public UnityEngine.Vector3 AimAngles { get { return protobuf.inputState.aimAngles; } }
		public int Buttons { get { return protobuf.inputState.buttons; } }
		public ModelState ModelState { get { return protobuf.modelState;  } }
		public float Aiming { get { return protobuf.modelState.aiming; } }
		public bool Ducked { get { return protobuf.modelState.ducked; } }
		public int Flags { get { return protobuf.modelState.flags; } }
		public bool Flying { get { return protobuf.modelState.flying; } }
		public bool Jumped { get { return protobuf.modelState.jumped} }
		public UnityEngine.Vector3 LookDirection { get { return protobuf.modelState.lookDir; } }
		public bool OnGround { get{ return protobuf.modelState.onground; } }
		public bool OnLadder { get{ return protobuf.modelState.onLadder; } }
		public bool Sleeping { get { return protobuf.modelState.sleeping; } }
		public bool Sprinting { get { return protobuf.modelState.sprinting;  } }
		public float WaterLevel { get { return protobuf.modelState.waterLevel; } }
		

		public Tick(Packet p) {
			protobuf = PlayerTick.Deserialize(p);
		}

	}
}
