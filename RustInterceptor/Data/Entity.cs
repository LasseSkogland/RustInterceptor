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

		/* BaseEntity */
		internal uint num = 0;
		public uint Number { get { return num; } }
		public Vector3 Position { get { return protobuf.baseEntity.pos; } }
		public Vector3 Rotation { get { return protobuf.baseEntity.rot; } }

		/* BasePlayer */
		public string PlayerName { get { return protobuf.basePlayer.name; } }
		public ulong PlayerUserID { get { return protobuf.basePlayer.userid; } }
		public bool IsPlayer { get { return protobuf.basePlayer != null; } }
		public ProtoBuf.PlayerInventory Inventory { get { return protobuf.basePlayer.inventory; } }

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

		public static void UpdatePosition(Packet packet) {
			while (packet.unread > 28L) {
				var id = packet.UInt32();
				CheckEntity(id);
				/* Read 2 Vector3f in form of 3 floats each, Position and Rotation */
				entities[id].protobuf.baseEntity.pos.Set(packet.Float(), packet.Float(), packet.Float());
				entities[id].protobuf.baseEntity.rot.Set(packet.Float(), packet.Float(), packet.Float());
			}
		}

		public static uint CreateOrUpdate(Packet packet) {
			/* Entity Number, for internal use */
			var num = packet.UInt32();
			ProtoBuf.Entity proto = global::ProtoBuf.Entity.Deserialize(packet);
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