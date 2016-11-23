using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Data {

	public class Entity {
		internal ProtoBuf.Entity protobuf;
		public ProtoBuf.Entity Protobuf { get { return protobuf; } }

		/* BaseNetworkable */
		public uint UID { get { return protobuf.baseNetworkable.uid; } }
		public uint Group { get { return protobuf.baseNetworkable.group; } }
		public uint PrefabID { get { return protobuf.baseNetworkable.prefabID; } }

		/* BaseEntity */
		internal uint num = 0;
		public uint Number { get { return num; } }
		public Vector3 Position { get { return protobuf.baseEntity.pos; } }
		public Vector3 Rotation { get { return protobuf.baseEntity.rot; } }
		public int Flags { get { return protobuf.baseEntity.flags; } }
		public ulong SkinID { get { return protobuf.baseEntity.skinid; } }
		public bool CreatedThisFrame { get { return protobuf.createdThisFrame; } }
		public uint HeldEntityUID { get { return protobuf.heldEntity.itemUID; } }

		/* BasePlayer */
		public bool IsPlayer { get { return protobuf.basePlayer != null; } }
		internal BasePlayer player;
		public BasePlayer Player { get { return player; } }

		/* BuildingBlock */
		internal BaseBuildingBlock buildingBlock;
		public BaseBuildingBlock BuildingBlock { get { return buildingBlock; } }

		/* Environment */
		internal BaseEnvironment environment;
		public BaseEnvironment Environment { get { return environment; } }

		/* Corpse */
		public uint Corpse_ParentID { get { return protobuf.corpse.parentID; } }

		/* Parent */
		public uint Parent_UID { get { return protobuf.parent.uid; } }
		public uint Parent_Bone { get { return protobuf.parent.bone; } }

		/* Keylock */
		public int KeyLock_Code { get { return protobuf.keyLock.code; } }

		/* CodeLock */
		internal BaseCodeLock codeLock;
		public BaseCodeLock CodeLock { get { return codeLock; } }

		/* Entity Slots */
		public uint EntitySlots_SlotLock { get { return protobuf.entitySlots.slotLock; } }
		public uint EntitySlots_SlotFireMod { get { return protobuf.entitySlots.slotFireMod; } }

		/* Building Privilege */
		internal BaseBuildingPrivilege buildingPrivilege;
		public BaseBuildingPrivilege BuildingPrivilege { get { return buildingPrivilege; } }

		/* Storage Box */
		internal BaseItem.BaseItemContainer storageBoxContents;
		public BaseItem.BaseItemContainer StorageBox { get { return storageBoxContents; } }

		/* Resource */
		internal BaseResource resource;
		public BaseResource Resource { get { return resource; } }

		/* WorldItem */
		internal BaseItem worldItem;
		public BaseItem WorldItem { get { return worldItem; } }

		/* List of all received entities */
		internal static Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();
		public static List<Entity> Entities { get { return entities.Values.ToList(); } }

		/* List of all received players */
		internal static List<Entity> players = new List<Entity>();
		public static List<Entity> Players { get { return players; } }

		/* Local Player is sent first, so it should be first */
		public static Entity LocalPlayer { get { return Players.First(); } }

		public static bool CheckEntity(uint id) {
			return entities.ContainsKey(id);
		}

		public static void UpdatePosition(Packet p) {
			/* EntityPosition packets may contain multiple positions */
			while (p.unread >= 28L) {
				/* Entity UID */
				var id = p.UInt32();
				CheckEntity(id);
				/* Read 2 Vector3 in form of 3 floats each, Position and Rotation */
				entities[id].protobuf.baseEntity.pos.Set(p.Float(), p.Float(), p.Float());
				entities[id].protobuf.baseEntity.rot.Set(p.Float(), p.Float(), p.Float());
			}
		}

		public static uint CreateOrUpdate(Packet p) {
			/* Entity Number/Order, for internal use */
			var num = p.UInt32();
			ProtoBuf.Entity proto = global::ProtoBuf.Entity.Deserialize(p);
			/* All Networkables have Unique Identifiers */
			var id = proto.baseNetworkable.uid;
			if (CheckEntity(id))
				entities[id].protobuf = proto;
			else {
				entities[id] = new Entity(proto);
				entities[id].num = num;
			}
			return id;
		}

		public Entity(ProtoBuf.Entity proto) {
			protobuf = proto;
			player = new BasePlayer(proto.basePlayer);
			resource = new BaseResource(proto.resource);
			buildingBlock = new BaseBuildingBlock(proto.buildingBlock);
			worldItem = new BaseItem(proto.worldItem.item);
			environment = new BaseEnvironment(proto.environment);
			resource = new BaseResource(proto.resource);
			codeLock = new BaseCodeLock(proto.codeLock);
			buildingPrivilege = new BaseBuildingPrivilege(proto.buildingPrivilege);
			if(proto.storageBox != null) storageBoxContents = new BaseItem.BaseItemContainer(proto.storageBox.contents);


			if (IsPlayer) {
				players.Add(this);
			}
		}

		/* Extracted Classes (Lazy) */

		public class BaseResource {
			internal ProtoBuf.BaseResource protobuf;
			public float Health { get { return protobuf.health; } }
			public int Stage { get { return protobuf.stage; } }
			public BaseResource(ProtoBuf.BaseResource proto) { protobuf = proto; }
		}

		public class BaseBuildingBlock {
			internal ProtoBuf.BuildingBlock protobuf;
			public bool BeingDemolished { get { return protobuf.beingDemolished; } }
			public uint BuildingID { get { return protobuf.buildingID; } }
			public int Grade { get { return protobuf.grade; } }
			public float Stability { get { return protobuf.stability; } }
			public BaseBuildingBlock(ProtoBuf.BuildingBlock proto) { protobuf = proto; }
		}

		public class BaseBuildingPrivilege {
			internal List<BasePlayerNameID> users;
			public List<BasePlayerNameID> Users { get { return users; } }
			public BaseBuildingPrivilege(ProtoBuf.BuildingPrivilege proto) {
				users = new List<BasePlayerNameID>();
				proto.users.ForEach(item => users.Add(new BasePlayerNameID(item)));
			}
		}

		public class BasePlayerNameID {
			internal ProtoBuf.PlayerNameID protobuf;
			public ulong ID { get { return protobuf.userid; } }
			public string Name { get { return protobuf.username; } }
			public BasePlayerNameID(ProtoBuf.PlayerNameID proto) { protobuf = proto; }
		}

		public class BaseEnvironment {
			internal ProtoBuf.Environment protobuf;
			public float Clouds { get { return protobuf.clouds; } }
			public long DateTime { get { return protobuf.dateTime; } }
			public float Fog { get { return protobuf.fog; } }
			public float Rain { get { return protobuf.rain; } }
			public float Wind { get { return protobuf.wind; } }
			public BaseEnvironment(ProtoBuf.Environment proto) { protobuf = proto; }
		}

		public class BaseCodeLock {
			ProtoBuf.CodeLock protobuf;
			public bool HasCode { get { return protobuf.hasCode; } }
			internal BasePrivate _private;
			public BasePrivate Private { get { return _private; } }

			public BaseCodeLock(ProtoBuf.CodeLock proto) {
				protobuf = proto;
				_private = new BasePrivate(protobuf.pv);
			}

			public class BasePrivate {
				ProtoBuf.CodeLock.Private protobuf;
				public string Code { get { return protobuf.code; } }
				public List<ulong> Users { get { return protobuf.users; } }
				public BasePrivate(ProtoBuf.CodeLock.Private proto) { protobuf = proto; }
			}
		}


	}
}