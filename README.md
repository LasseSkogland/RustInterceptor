## Rust Interceptor
### Currently Implemented:
- **Client - Server "Proxy"**
- **Packet Serialization**(JSON formatted)
- **Packet Parsers:**
 - **Entities** (Partial)
 - **EntityPosition**

### Disclaimer
- **This is not a packet forger, and never will be.**
- This product is meant for educational purposes only.
- This is work in progress and subject to change.
- Void where prohibited.
- No other warranty expressed or implied.
- Some assembly required.
- Batteries not included.
- Use only as directed.
- Do not use while operating a motor vehicle or heavy equipment.

## Usage
Under construction...

### Examples
**Dump Packets**
``` csharp
private static void Main(string[] args) {
	RustInterceptor interceptor;
	Console.Write("Server IP: ");
	string ip = Console.ReadLine();
	int port = -1;
	while (port == -1) {
		Console.Write("Server Port: ");
		try {
			port = int.Parse(Console.ReadLine().Trim());
		} catch (Exception) {
			Console.WriteLine("Try again...");
		}
	}
	interceptor = new RustInterceptor(server: ip, port: port);
	Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) => {
		interceptor.SavePackets("packets.json");
	};
	interceptor.ClientPackets = true;
	interceptor.AddPacketToFilter(0);
	interceptor.Start();
	while (interceptor.IsAlive()) {
		Thread.Sleep(10);
	}
	Console.WriteLine("Shutting down...");
	interceptor.SavePackets("packets.json");
}
```
**HandleEntities**
``` csharp
private static void Main(string[] args) {
	RustInterceptor interceptor;
	Console.Write("Server IP: ");
	string ip = Console.ReadLine();
	int port = -1;
	while (port == -1) {
		Console.Write("Server Port: ");
		try {
			port = int.Parse(Console.ReadLine().Trim());
		} catch (Exception) {
			Console.WriteLine("Try again...");
		}
	}
	interceptor = new RustInterceptor(server: ip, port: port);
	interceptor.ClientPackets = false;
	interceptor.AddPacketToFilter(Packet.Rust.Entities);
    interceptor.AddPacketToFilter(Packet.Rust.EntityPosition);
	interceptor.Start();
	Packet packet;
	while (interceptor.IsAlive()) {
		try {
			packet = interceptor.GetPacket();
			switch ((Rust)packet.packetID) {
				case Packet.Rust.Entities:
					Packets.Entity.CreateOrUpdate(packet);
					break;
				case Packet.Rust.EntityPosition:
					Packets.Entity.UpdatePosition(packet);
					break;
			}
		} catch (Exception) {}
	}
}
```

Bitcoin: 3BYt2fDDd1kQUAWVKR51o9fxcU4eggc8Xq

![Bitcoin QR](http://i.imgur.com/Q7S8buL.png)