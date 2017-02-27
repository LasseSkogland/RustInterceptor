## Rust Interceptor
### Currently Implemented:
- **Client - Server "Proxy"**
- **Packet Serialization**(JSON formatted)
- **Packet Parsers:**
 - **Entities** (Partial, this one's big)
 - **All other handlers are complete**
 
### Dependencies:
- Steam\steamapps\common\Rust\RustClient_Data\Managed\Rust.Data.dll
- Steam\steamapps\common\Rust\RustClient_Data\Managed\UnityEngine.dll
- Steam\steamapps\common\Rust\RustClient_Data\Plugins\RakNet.dll

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
	RustInterceptor.FindDependencies();
	RustInterceptor rusti;
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
	rusti = new RustInterceptor(server: ip, port: port);
	Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) => {
		rusti.SavePackets("packets.json");
	};
	rusti.ClientPackets = true;
	rusti.AddPacketToFilter(0);
	rusti.Start();
	while (rusti.IsAlive) {
		System.Threading.Thread.Sleep(10);
	}
	Console.WriteLine("Shutting down...");
	rusti.SavePackets("packets.json");
}
```
**HandleEntities**
``` csharp
private static void Main(string[] args) {
	RustInterceptor.FindDependencies();
	RustInterceptor rusti;
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
	rusti = new RustInterceptor(server: ip, port: port);
	rusti.ClientPackets = false;
	rusti.RememberPackets = false;
	Packet packet;
	Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs arg) => {
		if (arg.Cancel) rusti.Stop();
	};
	rusti.Start();
	while (rusti.IsAlive) {
		rusti.GetPacket(out packet);
		switch ((Packet.Rust)packet.packetID) {
			case Packet.Rust.Entities:
				Data.Entity.CreateOrUpdate(packet);
				break;
			case Packet.Rust.EntityPosition:
				Data.Entity.UpdatePosition(packet);
				break;
		}
	}
	Console.WriteLine("Shutting down...");
	Console.ReadKey();
}
```

Bitcoin: 3BYt2fDDd1kQUAWVKR51o9fxcU4eggc8Xq
Paypal.me: paypal.me/LasseSkogland

![Bitcoin QR](http://i.imgur.com/Q7S8buL.png)
