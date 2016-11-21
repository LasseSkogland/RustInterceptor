using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class GroupChange {
		internal uint entityId;
		public uint EntityID { get{ return entityId; } }
		internal uint groupId;
		public uint GroupID { get{ return groupId; } }

		public GroupChange(Packet p) {
			entityId = p.EntityID();
			groupId = p.GroupID();
		}
	}
}
