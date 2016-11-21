using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class ConsoleMessage {

		internal string message;
		public string Message { get{ return message; } }

		public ConsoleMessage(Packet p) {
			message = p.String();
		}

	}
}
