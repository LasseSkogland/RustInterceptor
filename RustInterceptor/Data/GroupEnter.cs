using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class GroupEnter {

		internal uint groupId;
		public uint GroupID { get { return groupId; } }

		public GroupEnter(Packet p) {
			groupId = p.GroupID();
		}
	}
}
