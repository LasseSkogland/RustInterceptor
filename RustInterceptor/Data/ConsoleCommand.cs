namespace Rust_Interceptor.Data {
	public class ConsoleCommand {

		internal string command;
		public string Command { get { return command; } }

		public ConsoleCommand(Packet p) {
			command = p.String();
		}
	}
}
