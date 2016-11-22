using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Data {

	public class Entity {

		/* BaseNetworkable */
		public uint UID { get { return protobuf.baseNetworkable.uid; } }
		public uint Group { get { return protobuf.baseNetworkable.group; } }
		public uint PrefabID { get { return protobuf.baseNetworkable.prefabID; } }

		/* BaseEntity */
		internal uint num = 0;
		public uint Number { get { return num; } }
		internal ProtoBuf.Entity protobuf;
		public ProtoBuf.Entity Protobuf { get { return protobuf; } }
		public Vector3 Position { get { return protobuf.baseEntity.pos; } }
		public Vector3 Rotation { get { return protobuf.baseEntity.rot; } }
		public int Entity_Flags { get { return protobuf.baseEntity.flags; } }
		public ulong SkinID { get { return protobuf.baseEntity.skinid; } }

		internal BasePlayer player;
		public BasePlayer Player { get { return player; } }
		public bool IsPlayer { get { return protobuf.basePlayer != null; } }

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
			if (IsPlayer) players.Add(this);
		}
	}
}