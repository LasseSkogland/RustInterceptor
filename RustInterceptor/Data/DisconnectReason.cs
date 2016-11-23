namespace Rust_Interceptor.Data {
	public class DisconnectReason {

		internal string reason;
		public string Reason { get { return reason; } }

		public DisconnectReason(Packet p) {
			reason = p.String();
		}

	}
}
