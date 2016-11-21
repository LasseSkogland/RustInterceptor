using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Data {

	public class Entity {
		
		internal ProtoBuf.Entity protobuf;
		public ProtoBuf.Entity ProtoBuf { get { return protobuf; } }

		/* BasePlayer */
		public string PlayerName { get { return protobuf.basePlayer.name; } }
		public ulong PlayerUserID { get { return protobuf.basePlayer.userid; } }
		public ProtoBuf.PlayerInventory Inventory { get { return protobuf.basePlayer.inventory; } }
		public bool IsPlayer { get { return protobuf.basePlayer != null; } }

		/* BaseNetworkable */
		public uint UID { get { return protobuf.baseNetworkable.uid; } }
		public uint Group { get { return protobuf.baseNetworkable.group; } }

		/* BaseEntity */
		internal uint num = 0;
		public uint Number { get { return num; } }
		public Vector3 Position { get { return protobuf.baseEntity.pos; } }
		public Vector3 Rotation { get { return protobuf.baseEntity.rot; } }

		internal Entity(uint num, ProtoBuf.Entity proto) {
			this.num = num;
			protobuf = proto;
			if (IsPlayer) players.Add(this);
		}
		
		internal static Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();
		internal static List<Entity> players = new List<Entity>();
		public static List<Entity> Entities { get { return entities.Values.ToList(); } }
		public static List<Entity> Players { get { return players; } }
		/* Local Player is sent first, so it should be first */
		public static Entity LocalPlayer { get { return Players.First(); } }

		public static bool CheckEntity(uint id) {
			return entities.ContainsKey(id);
		}

		public static void UpdatePosition(Packet packet) {
			while (packet.unread > 28L) {
				var id = packet.UInt32();
				CheckEntity(id);
				entities[id].protobuf.baseEntity.pos.Set(packet.Float(), packet.Float(), packet.Float());
				entities[id].protobuf.baseEntity.rot.Set(packet.Float(), packet.Float(), packet.Float());
			}
		}

		public static uint CreateOrUpdate(Packet packet) {
			var num = packet.UInt32();
			var proto = global::ProtoBuf.Entity.Deserialize(packet);
			var id = proto.baseNetworkable.uid;
			if (CheckEntity(id))
				entities[id].protobuf = proto;
			else {
				entities[id] = new Entity(num, proto);
			}
			return id;
		}
	}
}