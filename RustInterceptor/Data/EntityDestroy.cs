namespace Rust_Interceptor.Data
{
	public class EntityDestroy
    {
        internal static uint uid;
		public uint UID { get{ return uid; } }
		internal static byte destroyMode;
		public byte DestroyMode { get{ return destroyMode; } }

		public static bool Destroy(Packet p) {
			uid = p.UInt32();
			destroyMode = p.UInt8();
            return Entity.Destroy(uid, destroyMode);
		}
	}
}