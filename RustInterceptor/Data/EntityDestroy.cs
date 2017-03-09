namespace Rust_Interceptor.Data {
	public class EntityDestroy {
		public uint UID { get; private set; }
		public byte DestroyMode { get; private set; }

		public EntityDestroy(Packet p) {
			UID = p.UInt32();
			DestroyMode = p.UInt8();
		}
	}
}
