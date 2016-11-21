using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Facepunch.Network.Raknet;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text;

namespace Rust_Interceptor {
	public class RustInterceptor {
		public bool ClientPackets = false;
		public bool RememberPackets = false;
		public bool RememberFilteredOnly = false;

		private bool isAlive;
		public bool IsAlive {
			get {
				return isAlive;
			}
		}
		private static Packet serverPacket;
		private static Packet clientPacket;
		static List<Packet> remeberedPackets;
		static List<Packet.Rust> packetFilter;

		private readonly string serverIP;
		private readonly int serverPort;
		private readonly int listenPort;
		private readonly Queue<Packet> packetQueue;

		private RakNetPeer clientPeer;
		private RakNetPeer serverPeer;
		private readonly Thread backgroundThread;

		public RustInterceptor(string server = "127.0.0.1", int port = 28015, int listenPort = 5678) {
			serverIP = server;
			serverPort = port;
			this.listenPort = listenPort;
			serverPacket = new Packet();
			clientPacket = new Packet();
			backgroundThread = new Thread(BackgroundThread);
			packetQueue = new Queue<Packet>();
			remeberedPackets = new List<Packet>();
			packetFilter = new List<Packet.Rust>();

		}

		public static ulong serverGUID {
			get { return serverPacket.incomingGUID; }
		}

		public static ulong clientGUID {
			get { return clientPacket.incomingGUID; }
		}

		public void AddPacketToFilter(Packet.Rust packetType) {
			packetFilter.Add(packetType);
		}

		public void AddPacketsToFilter(params Packet.Rust[] packetTypes) {
			foreach (Packet.Rust type in packetTypes) {
				packetFilter.Add(type);
			}
		}

		public bool IsFiltered(Packet p) {
			if (p.type != Packet.PacketType.RUST) return false;
			if (packetFilter.Count == 0)
				return true;
			else
				return packetFilter.Contains((Packet.Rust)p.packetID);
		}

		public void Start() {
			isAlive = true;
			backgroundThread.Start();
		}

		public void Stop() {
			isAlive = false;
			while (backgroundThread.IsAlive) Thread.Sleep(1);
		}

		public Packet[] LoadPackets(string filename = "packets.json") {
			var json = File.ReadAllText(filename);
			return JsonConvert.DeserializeObject<Packet[]>(json);
		}

		public void SavePackets(Packet[] packet, string filename = "packets.json", Formatting formatting = Formatting.Indented, bool informative = true) {
			var outs = File.Create(filename);
			Serializer.informativeDump = informative;
			string json = Serializer.Serialize(packet);
			outs.Write(Encoding.ASCII.GetBytes(json), 0, json.Length);
			outs.Flush();
			outs.Close();
		}

		public void SavePackets(List<Packet> packets, string filename = "packets.json", Formatting formatting = Formatting.Indented, bool informative = true) {
			SavePackets(packets.ToArray(), filename, formatting, informative);
		}

		public void SavePackets(string filename = "packets.json", Formatting formatting = Formatting.Indented, bool informative = true) {
			if (!RememberPackets) {
				Console.WriteLine("There are no packets to save. Set 'RememberPackets' before you start or pass in packets to this function");
				return;
			}
			SavePackets(remeberedPackets.ToArray(), filename, formatting, informative);
		}

		public Packet GetPacket(bool wait = true) {
			while ((packetQueue.Count < 1) && wait) Thread.Sleep(1);
			lock (packetQueue) {
				return packetQueue.Dequeue();
			}
		}

		private void BackgroundThread() {
			Thread thread = new Thread(() => Clipboard.SetText(string.Format("connect 127.0.0.1:{0}", listenPort)));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			Console.WriteLine("Server address copied to Clipboard (F1 -> Paste -> Enter)");
			Console.WriteLine("Listening on {0}", listenPort);
			clientPeer = RakNetPeer.CreateServer("0.0.0.0", listenPort, 1);
			var emptyPacket = false;
			var hasClientPacket = false;
			var hasServerPacket = false;
			while (isAlive) {
				hasClientPacket = clientPeer.Receive();
				hasServerPacket = (serverPeer == null ? false : serverPeer.Receive());
				if (!hasClientPacket && !hasServerPacket) {
					Thread.Sleep(1);
					continue;
				}
				if (hasClientPacket) {
					var ticks = DateTime.Now.Ticks;
					clientPacket.Receive(clientPeer);
					var packet = clientPacket.Clone();
					if (packet.Length == packet.Position) {
						emptyPacket = true;
					}
					if (packet.type == Packet.PacketType.RAKNET) {
						switch ((Packet.RakNet)packet.packetID) {
							case Packet.RakNet.NEW_INCOMING_CONNECTION:
								Console.Write("Starting Proxy: ");
								serverPeer = RakNetPeer.CreateConnection(serverIP, serverPort, 32, 100, 1000);
								while (!serverPeer.Receive()) Thread.Sleep(10);
								serverPacket.Receive(serverPeer);
								serverPacket.Send(clientPeer, clientGUID);
								serverPacket.Clear();
								Console.WriteLine("Started");
								break;
							case Packet.RakNet.CONNECTION_LOST:
								isAlive = false;
								Console.WriteLine("Client Disconnected");
								break;
						}
						if (!isAlive) break;
					} else if (ClientPackets && !emptyPacket && IsFiltered(packet)) {
						lock (packetQueue) {
							packetQueue.Enqueue(packet);
						}
					}
					clientPacket.Send(serverPeer, serverGUID);
					clientPacket.Clear();
					packet.delay = (DateTime.Now.Ticks - ticks);
					if (RememberFilteredOnly && !IsFiltered(packet) && !ClientPackets) continue;
					if (RememberPackets)
						lock (remeberedPackets) {
							remeberedPackets.Add(packet);
						}
				}
				if (hasServerPacket) {
					var ticks = DateTime.Now.Ticks;
					serverPacket.Receive(serverPeer);
					var packet = serverPacket.Clone();
					emptyPacket = false;
					if (packet.Position == packet.Length) {
						emptyPacket = true;
					}
					if (IsFiltered(packet) && !emptyPacket) {
						lock (packetQueue) {
							packetQueue.Enqueue(packet);
						}
					}
					serverPacket.Send(clientPeer, clientGUID);
					serverPacket.Clear();
					packet.delay = (DateTime.Now.Ticks - ticks);
					if (RememberFilteredOnly && !IsFiltered(packet)) continue;
					if (RememberPackets)
						lock (remeberedPackets) {
							remeberedPackets.Add(packet);
						}
				}

			}
		}
	}
}