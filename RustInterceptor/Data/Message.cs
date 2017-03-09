namespace Rust_Interceptor.Data {
	public class Message {

		/* Something for the Loading Screen */
		public string Title { get; private set; }
		public string Subtitle { get; private set; }

		public Message(Packet p) {
			Title = p.String();
			Subtitle = p.String();
		}
	}
}
