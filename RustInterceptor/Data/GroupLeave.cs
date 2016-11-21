using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class GroupLeave {

		internal uint groupId;
		public uint GroupID { get { return groupId; } }

		public GroupLeave(Packet p) {
			groupId = p.GroupID();
		}
	}
}
