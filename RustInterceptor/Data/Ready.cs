using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class Ready {
		
		Dictionary<string, string> clientInfo;
		public Dictionary<string, string> ClientInfo { get { return clientInfo; } }



		public Ready(Packet p) {
			var protobuf = ProtoBuf.ClientReady.Deserialize(p);
			clientInfo = new Dictionary<string, string>();
			protobuf.clientInfo.ForEach(item => clientInfo.Add(item.name, item.value));
		}

	}
}
