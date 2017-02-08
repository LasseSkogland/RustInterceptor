using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rust_Interceptor {
    interface PacketHandler {

        void OnClientPacket(ref Packet packet);
        void OnServerPacket(ref Packet packet);

    }
}
