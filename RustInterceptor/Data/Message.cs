using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class Message {

		/* Not tested and could be wrong, but should parse */
		internal string playerName;
		public string PlayerName { get { return playerName; } }
		internal string playerMessage;
		public string PlayerMessage { get { return playerMessage; } }

		public Message(Packet p) {
			playerName = p.String();
			playerMessage = p.String();
		}
	}
}
