using Rust_Interceptor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rust_Interceptor {
	public abstract class SimpleInterceptor {

		public RustInterceptor Interceptor;

		public abstract void OnCommand(string command);
		public abstract void OnPacket(Packet packet);
		public abstract void OnEntity(Entity entity);
		public abstract void OnEntityDestroy(EntityDestroy destroyInfo);

		internal void internalOnPacket(Packet packet) {
			Entity entity;
			switch (packet.rustID) {
				case Packet.Rust.Entities:
					ProtoBuf.Entity entityInfo;
					uint num = Data.Entity.ParseEntity(packet, out entityInfo);
					entity = Entity.CreateOrUpdate(num, entityInfo);
					if (entity != null) OnEntity(entity);
					return;
				case Packet.Rust.EntityPosition:
					List<Data.Entity.EntityUpdate> updates = Data.Entity.ParsePositions(packet);
					List<Entity> entities = null;
					if (updates.Count == 1) {
						entity = Entity.UpdatePosistion(updates[0]);
						if (entity != null) (entities = new List<Entity>()).Add(entity);
					} else if (updates.Count > 1) {
						entities = Entity.UpdatePositions(updates);
					}
					if (entities != null) entities.ForEach(item => OnEntity(item));
					return;
				case Packet.Rust.EntityDestroy:
					EntityDestroy destroyInfo = new EntityDestroy(packet);
					Entity.CreateOrUpdate(destroyInfo);
					OnEntityDestroy(destroyInfo);
					return;
				
			}
			OnPacket(packet);
		}

		public SimpleInterceptor() {
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
			Interceptor = new RustInterceptor(ip, port);
			Interceptor.AddPacketsToFilter(Packet.Rust.Entities, Packet.Rust.EntityDestroy, Packet.Rust.EntityPosition);
			Interceptor.commandCallback = OnCommand;
			Interceptor.packetHandlerCallback = internalOnPacket;
		}
	}
}
