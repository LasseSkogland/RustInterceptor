using System.Collections.Generic;

namespace Rust_Interceptor.Data {
	public class BaseItem {
		internal ProtoBuf.Item protobuf;

		public uint UID { get { return protobuf.UID; } }
		public int ItemID { get { return protobuf.itemid; } }
		public int Slot { get { return protobuf.slot; } }
		public int Amount { get { return protobuf.amount; } }
		public int Flags { get { return protobuf.flags; } }
		public float RemoveTime { get { return protobuf.removetime; } }
		public float LockTime { get { return protobuf.locktime; } }
		public uint WorldEntity { get { return protobuf.worldEntity; } }
		internal BaseInstanceData instanceData;
		public BaseInstanceData InstanceData { get { return instanceData; } }
		public uint HeldEntity { get { return protobuf.heldEntity; } }
		internal BaseConditionData conditionData;
		public BaseConditionData ConditionData { get { return conditionData; } }
		internal List<BaseOwnerFraction> owners;
		public List<BaseOwnerFraction> Owners { get { return owners; } }
		public string Name { get { return protobuf.name; } }
		public string Text { get { return protobuf.text; } }
		public ulong SkinID { get { return protobuf.skinid; } }
		internal BaseItemContainer contents;
		public BaseItemContainer Contents { get { return contents; } }

		public BaseItem(ProtoBuf.Item proto) {
			protobuf = proto;
			instanceData = new BaseInstanceData(proto.instanceData);
			conditionData = new BaseConditionData(proto.conditionData);
			owners = new List<BaseOwnerFraction>();
			proto.owners.ForEach(item => owners.Add(new BaseOwnerFraction(item)));
			contents = new BaseItemContainer(proto.contents);
		}


		public class BaseConditionData {
			internal ProtoBuf.Item.ConditionData protobuf;
			public float Condition { get { return protobuf.condition; } }
			public float MaxCondition { get { return protobuf.maxCondition; } }
			public BaseConditionData(ProtoBuf.Item.ConditionData proto) { protobuf = proto; }
		}

		public class BaseInstanceData {
			internal ProtoBuf.Item.InstanceData protobuf;
			public int DataInt { get { return protobuf.dataInt; } }
			public int BlueprintTarget { get { return protobuf.blueprintTarget; } }
			public int BlueprintAmount { get { return protobuf.blueprintAmount; } }
			public BaseInstanceData(ProtoBuf.Item.InstanceData proto) { protobuf = proto; }
		}

		public class BaseOwnerFraction {
			internal ProtoBuf.OwnerFraction protobuf;
			public float Fraction { get { return protobuf.fraction; } }
			public ulong UserID { get { return protobuf.userid; } }
			public BaseOwnerFraction(ProtoBuf.OwnerFraction proto) { protobuf = proto; }
		}

		public class BaseItemContainer {
			internal ProtoBuf.ItemContainer protobuf;
			public uint UID { get { return protobuf.UID; } }
			public int Slots { get { return protobuf.slots; } }
			public float Temperature { get { return protobuf.temperature; } }
			public int Flags { get { return protobuf.flags; } }
			public int AllowedContents { get { return protobuf.allowedContents; } }
			public int MaxStackSize { get { return protobuf.maxStackSize; } }
			public int AllowedItem { get { return protobuf.allowedItem; } }
			public List<int> AvailableSlots { get { return protobuf.availableSlots; } }
			internal List<BaseItem> contents;
			public List<BaseItem> Contents { get { return contents; } }

			public BaseItemContainer(ProtoBuf.ItemContainer proto) {
				protobuf = proto;
				contents = new List<BaseItem>();
				proto.contents.ForEach(item => contents.Add(new BaseItem(item)));
			}
		}
	}
}
