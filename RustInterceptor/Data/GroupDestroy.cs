namespace Rust_Interceptor.Data {
	public class GroupDestroy {

		internal uint groupId;
		public uint GroupID { get{ return groupId; } }

		public GroupDestroy(Packet p) {
			groupId = p.GroupID();
		}
	}
}
