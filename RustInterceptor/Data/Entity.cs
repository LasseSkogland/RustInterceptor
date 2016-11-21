using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Data {

	public class Entity {

		/* BaseNetworkable */
		/* See packet_structures.json for ProtoBuf layout */
		internal ProtoBuf.Entity protobuf;
		public ProtoBuf.Entity Protobuf { get { return protobuf; } }
		public uint UID { get { return protobuf.baseNetworkable.uid; } }
		public uint Group { get { return protobuf.baseNetworkable.group; } }
		public uint PrefabID { get { return protobuf.baseNetworkable.prefabID; } }

		/* BaseEntity */
		internal uint num = 0;
		public uint Number { get { return num; } }
		public Vector3 Position { get { return protobuf.baseEntity.pos; } }
		public Vector3 Rotation { get { return protobuf.baseEntity.rot; } }
		public int Entity_Flags { get { return protobuf.baseEntity.flags; } }
		public ulong SkinID { get { return protobuf.baseEntity.skinid; } }

		/* BasePlayer */
		#region BasePlayer
		public bool IsPlayer { get { return protobuf.basePlayer != null; } }
		public string PlayerName { get { return protobuf.basePlayer.name; } }
		public ulong PlayerUserID { get { return protobuf.basePlayer.userid; } }
		public int PlayerFlags { get { return protobuf.basePlayer.playerFlags; } }
		public uint HeldEntity { get { return protobuf.basePlayer.heldEntity; } }
		public float Health { get { return protobuf.basePlayer.health; } }
		public float SkinCol { get { return protobuf.basePlayer.skinCol; } }
		public float SkinTex { get { return protobuf.basePlayer.skinTex; } }
		public float SkinMesh { get { return protobuf.basePlayer.skinMesh; } }
		public ProtoBuf.PlayerInventory Inventory { get { return protobuf.basePlayer.inventory; } }
		#region Metabolism
		public ProtoBuf.PlayerMetabolism Metabolism { get { return protobuf.basePlayer.metabolism; } }
		public float Metabolism_Health { get { return protobuf.basePlayer.metabolism.health; } }
		public float Calories { get { return protobuf.basePlayer.metabolism.calories; } }
		public float Hydration { get { return protobuf.basePlayer.metabolism.hydration; } }
		public float Heartrate { get { return protobuf.basePlayer.metabolism.heartrate; } }
		public float Temperature { get { return protobuf.basePlayer.metabolism.temperature; } }
		public float Poison { get { return protobuf.basePlayer.metabolism.poison; } }
		public float RadiationLevel { get { return protobuf.basePlayer.metabolism.radiation_level; } }
		public float Wetness { get { return protobuf.basePlayer.metabolism.wetness; } }
		public float Dirtyness { get { return protobuf.basePlayer.metabolism.dirtyness; } }
		public float Oxygen { get { return protobuf.basePlayer.metabolism.oxygen; } }
		public float Bleeding { get { return protobuf.basePlayer.metabolism.bleeding; } }
		public float RadiationPoisoning { get { return protobuf.basePlayer.metabolism.radiation_poisoning; } }
		public float Comfort { get { return protobuf.basePlayer.metabolism.comfort; } }
		public float PendingHealth { get { return protobuf.basePlayer.metabolism.pending_health; } }
		#endregion
		#region ModelState
		public ModelState ModelState { get { return protobuf.basePlayer.modelState; } }
		public float Aiming { get { return protobuf.basePlayer.modelState.aiming; } }
		public bool Ducked { get { return protobuf.basePlayer.modelState.ducked; } }
		public int ModelState_Flags { get { return protobuf.basePlayer.modelState.flags; } }
		public bool Flying { get { return protobuf.basePlayer.modelState.flying; } }
		public bool Jumped { get { return protobuf.basePlayer.modelState.jumped; } }
		public Vector3 LookDirection { get { return protobuf.basePlayer.modelState.lookDir; } }
		public bool OnGround { get { return protobuf.basePlayer.modelState.onground; } }
		public bool OnLadder { get { return protobuf.basePlayer.modelState.onLadder; } }
		public bool Sleeping { get { return protobuf.basePlayer.modelState.sleeping; } }
		public bool Sprinting { get { return protobuf.basePlayer.modelState.sprinting; } }
		public float WaterLevel { get { return protobuf.basePlayer.modelState.waterLevel; } }
		#endregion
		public ProtoBuf.PersistantPlayer PersistantData { get { return protobuf.basePlayer.persistantData; } }
		public ProtoBuf.PlayerLifeStory CurrentLife { get { return protobuf.basePlayer.currentLife; } }
		public ProtoBuf.PlayerLifeStory PreviousLife { get { return protobuf.basePlayer.previousLife; } }
		#endregion

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
			while (p.unread > 28L) {
				var id = p.UInt32();
				CheckEntity(id);
				/* Read 2 Vector3f in form of 3 floats each, Position and Rotation */
				entities[id].protobuf.baseEntity.pos.Set(p.Float(), p.Float(), p.Float());
				entities[id].protobuf.baseEntity.rot.Set(p.Float(), p.Float(), p.Float());
			}
		}

		public static uint CreateOrUpdate(Packet p) {
			/* Entity Number, for internal use */
			var num = p.UInt32();
			ProtoBuf.Entity proto = global::ProtoBuf.Entity.Deserialize(p);
			/* All Networkables have Unique Identifiers */
			var id = proto.baseNetworkable.uid;

			if (CheckEntity(id))
				entities[id].protobuf = proto;
			else {
				entities[id] = new Entity(num, proto);
			}
			return id;
		}

		public Entity(uint num, ProtoBuf.Entity proto) {
			this.num = num;
			protobuf = proto;
			if (IsPlayer) players.Add(this);
		}
	}
}