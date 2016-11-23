namespace Rust_Interceptor.Data {
	public class GroupEnter {

		internal uint groupId;
		public uint GroupID { get { return groupId; } }

		public GroupEnter(Packet p) {
			groupId = p.GroupID();
		}
	}
}
