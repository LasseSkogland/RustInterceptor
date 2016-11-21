using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class GroupDestroy {

		internal uint groupId;
		public uint GroupID { get{ return groupId; } }

		public GroupDestroy(Packet p) {
			groupId = p.GroupID();
		}
	}
}
