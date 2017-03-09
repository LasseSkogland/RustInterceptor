## Rust Interceptor
### Currently Implemented:
- **Client - Server "Proxy"**
- **Packet Serialization**(JSON formatted)
- **Packet Parsers for packets that does not contain Protocol Buffers**
- **Basic Entity Handler**
 
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

### Examples
**Advanced Users: [Have a look at this](https://github.com/SharpUmbrella/RustInterceptor/blob/master/RustInterceptor/SimpleInterceptor.cs)**

**Example for Beginners(noobs)**
``` csharp
using System;
using Rust_Interceptor;
using Rust_Interceptor.Data;

class Program : SimpleInterceptor {

	public Program() : base() {
		Interceptor.AddPacketsToFilter(Packet.Rust.ConsoleCommand, Packet.Rust.ConsoleMessage); // Filter packets, you will only receive the packets defined in this function, remove this line to receive all packets
		Interceptor.ClientPackets = true; // Receive client packets, in this example you would receive both Server and Client Packets
		Interceptor.CommandPrefix = "RI."; // Command Prefix for "sv" command, in this example you could send a command to this program with "sv RI.randomValue 24" and receive OnCommand("randomValue 24")
		Interceptor.Start();
	}

	public override void OnCommand(string command) {
		if (command.StartsWith("randomValue")) {
			var str = command.Split(' ');
			Console.WriteLine("{0} = {1}", str[0], int.Parse(str[1]));
		}
	}

	public override void OnPacket(Packet packet) {
		switch (packet.rustID) {
			case Packet.Rust.ConsoleMessage:
				ConsoleMessage message = new ConsoleMessage(packet);
				Console.WriteLine("Console Message from Server: {0}", message.Message);
				break;
		}
	}

	public override void OnEntity(Entity entity) {
		if (entity.IsPlayer)
			if (entity.IsLocalPlayer) Console.WriteLine("OMG is it really you {0} :O", entity.Data.basePlayer.name);
			else Console.WriteLine("Meh, you're not that special {0}", entity.Data.basePlayer.name);
	}

	public override void OnEntityDestroy(EntityDestroy destroyInfo) {
		Console.WriteLine("Entity with UID({0}) got destroyed :'(", destroyInfo.UID);
	}

	private static void Main(string[] args) {
		new Program();
	}
}

```
## For those fealing generous
Do not donate if you feel you need to, donate if you want to :)

Paypal.me: paypal.me/LasseSkogland

Bitcoin: 3BYt2fDDd1kQUAWVKR51o9fxcU4eggc8Xq

Bitcoin QR:
![Bitcoin QR](http://i.imgur.com/Q7S8buL.png)

