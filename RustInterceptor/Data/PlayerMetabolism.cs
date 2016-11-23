namespace Rust_Interceptor.Data {
	public class PlayerMetabolism {
		internal ProtoBuf.PlayerMetabolism protobuf;
		public float Health { get { return protobuf.health; } }
		public float Calories { get { return protobuf.calories; } }
		public float Hydration { get { return protobuf.hydration; } }
		public float Heartrate { get { return protobuf.heartrate; } }
		public float Temperature { get { return protobuf.temperature; } }
		public float Poison { get { return protobuf.poison; } }
		public float RadiationLevel { get { return protobuf.radiation_level; } }
		public float Wetness { get { return protobuf.wetness; } }
		public float Dirtyness { get { return protobuf.dirtyness; } }
		public float Oxygen { get { return protobuf.oxygen; } }
		public float Bleeding { get { return protobuf.bleeding; } }
		public float RadiationPoisoning { get { return protobuf.radiation_poisoning; } }
		public float Comfort { get { return protobuf.comfort; } }
		public float PendingHealth { get { return protobuf.pending_health; } }

		public PlayerMetabolism(ProtoBuf.PlayerMetabolism proto) {
			protobuf = proto;
		}
	}
}
