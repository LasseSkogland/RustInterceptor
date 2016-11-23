namespace Rust_Interceptor.Data {
	public class GroupChange {
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
