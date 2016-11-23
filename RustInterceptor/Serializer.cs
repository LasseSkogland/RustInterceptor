using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Rust_Interceptor {
	public class Serializer {

		internal static void Serialize(JsonWriter writer, object obj) {
			JsonSerializer serializer = new JsonSerializer();
			serializer.Formatting = Formatting.Indented;
			serializer.ContractResolver = new JsonResolver();
			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			serializer.MaxDepth = int.MaxValue;
			serializer.Serialize(writer, obj);
			writer.Flush();
		}

		public static string Serialize(object obj, bool informative = true) {
			informativeDump = informative;
			StringWriter stream = new StringWriter();
			JsonWriter writer = new JsonTextWriter(stream);
			Serialize(writer, obj);
			return stream.ToString();
		}

		public static void Serialize(object obj, string filename, bool informative = true) {
			informativeDump = informative;
			JsonWriter writer = new JsonTextWriter(File.CreateText(filename));
			Serialize(writer, obj);
		}

		public static object Deserialize(string json) {
			JsonReader reader = new JsonTextReader(new StringReader(json));
			JsonSerializer serializer = new JsonSerializer();
			serializer.ContractResolver = new JsonResolver();
			serializer.MaxDepth = int.MaxValue;
			return serializer.Deserialize(reader);
		}

		public static object DeserializeFile(string filename) {
			return Deserialize(File.ReadAllText(filename));
		}

		internal class JsonResolver : DefaultContractResolver {
			internal static Vector3Converter vector3Converter = new Vector3Converter();
			internal static PacketConverter packetConverter = new PacketConverter();
			internal static EntityConverter entityConverter = new EntityConverter();

			public override JsonContract ResolveContract(Type type) {
				JsonContract contract = base.ResolveContract(type);
				if (typeof(Packet).IsAssignableFrom(type)) contract.Converter = packetConverter;
				if (typeof(Vector3).IsAssignableFrom(type)) contract.Converter = vector3Converter;
				if (typeof(Data.Entity).IsAssignableFrom(type)) contract.Converter = entityConverter;
				if (typeof(string).IsAssignableFrom(type)) contract.DefaultCreator = () => { return ""; };
				return contract;
			}

			protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
				var props = type.GetProperties(bindingAttr: BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				List<JsonProperty> jsonProps = new List<JsonProperty>();

				foreach (var prop in props) {
					jsonProps.Add(base.CreateProperty(prop, memberSerialization));
				}

				foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
					jsonProps.Add(base.CreateProperty(field, memberSerialization));
				}

				jsonProps.ForEach(p => {
					if (p.PropertyName.Equals("_disposed") || p.PropertyName.Equals("ShouldPool")) {
						p.Writable = false;
						p.Readable = false;
						p.Ignored = true;
					} else {
						p.Writable = true;
						p.Readable = true;
						p.Ignored = false;
					}

				});
				return jsonProps;
			}
		}

		internal static bool informativeDump = false;
		internal class PacketConverter : JsonConverter {

			public bool Informative {
				get { return informativeDump; }
			}

			public override bool CanConvert(Type objectType) {
				return typeof(Packet).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				var jsonObject = JObject.Load(reader);
				var packet = new Packet();
				JToken token = null;
				Action<string> Deserialize = (string name) => {
					jsonObject.TryGetValue(name, StringComparison.CurrentCulture, out token);
				};
				Deserialize("ID");
				packet.packetID = token.Value<byte>();
				Deserialize("TypeID");
				packet.type = (Packet.PacketType)token.Value<ulong>();
				Deserialize("Length");
				packet.incomingLength = token.Value<int>();
				Deserialize("GUID");
				packet.incomingGUID = token.Value<ulong>();

				Deserialize("Data");
				JTokenReader read = new JTokenReader(token);
				byte[] data = read.ReadAsBytes();
				packet.Write(data, 0, data.Length);
				packet.Position = 1;
				return packet;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				serializer.ContractResolver = new JsonResolver();
				var packet = (Packet)value;
				writer.WriteStartObject();
				Action<string, object> Serialize = (string name, object obj) => {
					writer.WritePropertyName(name);
					serializer.Serialize(writer, obj);
				};
				if (Informative) {
					Serialize(string.Format("Name ({0})", packet.packetID), packet.GetName());
					Serialize("Type", packet.GetPacketTypeName());
					Serialize("Origin", packet.GetOrigin());
					Serialize("Delay", packet.delay);
				} else {
					Serialize("ID", packet.packetID);
					Serialize("TypeID", packet.type);
					Serialize("Length", packet.incomingLength);
					Serialize("GUID", packet.incomingGUID);
					Serialize("Data", packet.baseStream.ToArray());
				}
				if (packet.length > 1 && Informative) {
					if (packet.type == Packet.PacketType.RUST) {
						writer.WritePropertyName("Content");
						writer.WriteStartObject();
						packet.Position = 1;
						switch (packet.rustID) {
							case Packet.Rust.Approved:
								Serialize("ProtoBuf", ProtoBuf.Approval.Deserialize(packet));
								break;
							case Packet.Rust.Auth:
								var bytes = packet.BytesWithSize();
								Serialize("AuthData", BitConverter.ToString(bytes).Replace("-", ""));
								break;
							case Packet.Rust.ConsoleCommand:
								Serialize("Command", packet.String());
								break;
							case Packet.Rust.ConsoleMessage:
								Serialize("Message", packet.String());
								break;
							case Packet.Rust.DisconnectReason:
								Serialize("Reason", packet.String());
								break;
							case Packet.Rust.Effect:
								Serialize("ProtoBuf", EffectData.Deserialize(packet));
								break;
							case Packet.Rust.Entities:
								Serialize("Num", packet.UInt32());
								Serialize("ProtoBuf", ProtoBuf.Entity.Deserialize(packet));
								break;
							case Packet.Rust.EntityDestroy:
								Serialize("UID", packet.UInt32());
								Serialize("Destroy Mode", packet.UInt8());
								break;
							case Packet.Rust.EntityPosition:
								writer.WritePropertyName("Entity Positions");
								writer.WriteStartArray();
								while (packet.unread >= 28L) {
									writer.WriteStartObject();
									Serialize("Entity ID", packet.EntityID());
									Serialize("Position", packet.Vector3());
									Serialize("Rotation", packet.Vector3());
									writer.WriteEndObject();
								}
								writer.WriteEndArray();
								break;
							case Packet.Rust.GiveUserInformation:
								Serialize("ProtocolVersion", packet.UInt8());
								Serialize("Steam ID", packet.UInt64());
								Serialize("Protocol", packet.UInt32());
								Serialize("OS Name", packet.String());
								Serialize("Steam Name", packet.String());
								Serialize("Branch", packet.String());
								break;
							case Packet.Rust.GroupChange:
								Serialize("Entity ID", packet.EntityID());
								Serialize("Group ID", packet.GroupID());
								break;
							case Packet.Rust.GroupDestroy:
								Serialize("Group ID", packet.GroupID());

								break;
							case Packet.Rust.GroupEnter:
								Serialize("Group ID", packet.GroupID());
								break;
							case Packet.Rust.GroupLeave:
								Serialize("Group ID", packet.GroupID());
								break;
							case Packet.Rust.Message:
								Serialize("String 1 (PlayerName?)", packet.String());
								Serialize("String 2 (PlayerMsg?)", packet.String());
								break;
							case Packet.Rust.RPCMessage:
								Serialize("UID", packet.UInt32());
								Serialize("Name ID", packet.UInt32());
								Serialize("Source Connection", packet.UInt64());
								break;
							case Packet.Rust.Ready:
								Serialize("ProtoBuf", ProtoBuf.ClientReady.Deserialize(packet));
								break;
							case Packet.Rust.Tick:
								Serialize("ProtoBuf", PlayerTick.Deserialize(packet));
								break;
							default:
								break;
						}
						writer.WriteEndObject();
					}
				}

				writer.WriteEndObject();
			}
		}

		internal class EntityConverter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return typeof(Data.Entity).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				throw new NotImplementedException();
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				var entity = (Data.Entity)value;
				Action<string, object> Serialize = (string name, object obj) => {
					writer.WritePropertyName(name);
					serializer.Serialize(writer, obj);
				};
				writer.WriteStartObject();
				if (entity.IsPlayer) {
					Serialize("Player Name", entity.Player.Name);
					Serialize("User ID", entity.Player.UserID);
				}
				Serialize("UID", entity.UID);
				Serialize("Group", entity.Group);
				Serialize("Number", entity.Number);
				Serialize("Player", entity.IsPlayer);
				Serialize("Position", entity.Position);
				Serialize("Rotation", entity.Rotation);
				//if (entity.IsPlayer) Serialize("Inventory", entity.Inventory);
				writer.WriteEndObject();
			}
		}

		internal class Vector3Converter : JsonConverter {
			public override bool CanConvert(Type objectType) {
				return typeof(Vector3).IsAssignableFrom(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
				Vector3 vec;
				float[] data = serializer.Deserialize<float[]>(reader);
				vec = new Vector3(data[0], data[1], data[2]);
				return vec;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
				var vec = (Vector3)value;
				Action<string, object> Serialize = (string name, object obj) => {
					writer.WritePropertyName(name);
					serializer.Serialize(writer, obj);
				};
				writer.WriteStartObject();
				Serialize("x", vec.x);
				Serialize("y", vec.y);
				Serialize("z", vec.z);
				writer.WriteEndObject();
			}
		}

	}
}
