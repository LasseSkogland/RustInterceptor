using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rust_Interceptor.Data {

	public class Entity {
		public static void UpdatePosition(Packet p) {
			/* EntityPosition packets may contain multiple positions */
			while (p.unread >= 28L) {
				/* Entity UID */
				var id = p.UInt32();
				/* Read 2 Vector3 in form of 3 floats each, Position and Rotation */
				Vector3 Postion = new Vector3(p.Float(), p.Float(), p.Float());
				Vector3 Rotation = new Vector3(p.Float(), p.Float(), p.Float());
			}
		}

        public static void CreateOrUpdate(Packet p) {
			/* Entity Number/Order, for internal use */
			var num = p.UInt32();
			ProtoBuf.Entity proto = global::ProtoBuf.Entity.Deserialize(p);
        }

	}
}
