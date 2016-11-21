using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Packets {
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public class Entity : Attribute{
		private static Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();

		public static Entity[] Entities {
			get { return entities.Values.ToArray(); }
		}

		public static bool CheckEntity(uint id) {
			if (entities.ContainsKey(id))
				return true;
			entities.Add(id, new Entity());
			return false;
		}
		internal ProtoBuf.Entity _protobuf;
		public ProtoBuf.Entity protobuf {
			get {
				return _protobuf;
			}
		}

		public uint uid {
			get { return protobuf.baseNetworkable.uid; }
		}

		internal uint _num = 0;
		public uint num { get { return _num; } }

		public uint group {
			get { return protobuf.baseNetworkable.group; }
		}

		internal Vector3 _pos = new Vector3();
		public Vector3 pos {
			get { return _pos; }
		}

		internal Vector3 _rot = new Vector3();
		public Vector3 rot {
			get { return _rot; }
		}

		internal Entity() {
			_protobuf = new ProtoBuf.Entity();
		}

		public static void UpdatePosition(Packet packet) {
			while (packet.unread > 28L) {
				var id = packet.UInt32();
				CheckEntity(id);
				entities[id]._pos.Set(packet.Float(), packet.Float(), packet.Float());
				entities[id]._rot.Set(packet.Float(), packet.Float(), packet.Float());
			}

		}

		public static void CreateOrUpdate(Packet packet) {
			var num = packet.UInt32();
			var proto = ProtoBuf.Entity.Deserialize(packet);
			var id = proto.baseNetworkable.uid;
			CheckEntity(id);
			var entity = entities[id];
			entity._num = num;
			entity._protobuf = proto;
			entities[id] = entity;
		}
	}
}