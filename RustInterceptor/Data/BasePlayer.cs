namespace Rust_Interceptor.Data {
	public class BasePlayer {
		internal ProtoBuf.BasePlayer protobuf;
		public string Name { get { return protobuf.name; } }
		public ulong UserID { get { return protobuf.userid; } }
		public int Flags { get { return protobuf.playerFlags; } }
		public uint HeldEntity { get { return protobuf.heldEntity; } }
		public float Health { get { return protobuf.health; } }
		public float SkinCol { get { return protobuf.skinCol; } }
		public float SkinTex { get { return protobuf.skinTex; } }
		public float SkinMesh { get { return protobuf.skinMesh; } }

		public ProtoBuf.PlayerInventory Inventory { get { return protobuf.inventory; } }

		internal PlayerMetabolism metabolism;
		public PlayerMetabolism Metabolism { get { return metabolism; } }
		
		internal BaseModelState modelState;
		public BaseModelState ModelState { get { return modelState; } }

		public ProtoBuf.PersistantPlayer PersistantData { get { return protobuf.persistantData; } }
		public ProtoBuf.PlayerLifeStory CurrentLife { get { return protobuf.currentLife; } }
		public ProtoBuf.PlayerLifeStory PreviousLife { get { return protobuf.previousLife; } }

		public BasePlayer(ProtoBuf.BasePlayer proto) {
			protobuf = proto;
			metabolism = new PlayerMetabolism(proto.metabolism);
			modelState = new BaseModelState(proto.modelState);
			metabolism = new PlayerMetabolism(proto.metabolism);
		}
	}
}
