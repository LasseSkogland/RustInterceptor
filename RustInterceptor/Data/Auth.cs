using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	public class Auth {

		internal byte[] data;
		public byte[] Data { get { return data; } }

		public Auth(Packet p) {
			data = p.BytesWithSize();
		}
	}
}
