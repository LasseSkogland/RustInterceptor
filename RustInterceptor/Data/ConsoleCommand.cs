using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class ConsoleCommand {

		internal string command;
		public string Command { get { return command; } }

		public ConsoleCommand(Packet p) {
			command = p.String();
		}
	}
}
