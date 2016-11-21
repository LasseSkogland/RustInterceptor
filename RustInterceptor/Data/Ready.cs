using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rust_Interceptor.Data {
	class Ready {

		internal ProtoBuf.ClientReady protobuf;
		public ProtoBuf.ClientReady Protobuf { get { return protobuf; } }
		Dictionary<string, string> clientInfo;
		public Dictionary<string, string> ClientInfo { get { return clientInfo; } }



		public Ready(Packet p) {
			protobuf = ProtoBuf.ClientReady.Deserialize(p);
			clientInfo = new Dictionary<string, string>();
			protobuf.clientInfo.ForEach(item => clientInfo.Add(item.name, item.value));
		}

	}
}
