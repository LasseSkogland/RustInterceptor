using Rust_Interceptor;
using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Rust_Interceptor {
	public class ProtoGen {

		public static string GenerateProtoBufStructures() {
			StringBuilder str = new StringBuilder();
			Assembly rustData = Assembly.LoadFrom("Rust.Data.dll"); ;
			var types = rustData.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IProto)));
			foreach (var type in types) {
				str.AppendLine(String.Format("{0}: ", type.Name));
				str.AppendLine(ObjectDumper.Dump(Activator.CreateInstance(type)));
			}
			return str.ToString();
		}

		private static object InstantiateType(Type type) {
			try {
				ProtoBuf.Item item = new ProtoBuf.Item();
				var element = Activator.CreateInstance(type);
				IntPtr baseAddress = TypeDelegator.GetTypeHandle(element).Value;
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields) {
					if (field.FieldType.IsEquivalentTo(typeof(String)))
						field.SetValue(element, "RI");
					else if (field.FieldType.IsEquivalentTo(typeof(Byte[])))
						field.SetValue(element, new byte[0]);
					else if (typeof(IList).IsAssignableFrom(field.FieldType)) {
						var itemType = field.FieldType.GetGenericArguments()[0];
						object listItem = null;
						if (itemType.IsEquivalentTo(typeof(ProtoBuf.PlayerNameID))) listItem = InstantiateType(typeof(ProtoBuf.PlayerNameID));
						else if (itemType.IsEquivalentTo(typeof(ProtoBuf.RespawnInformation.SpawnOptions))) listItem = InstantiateType(typeof(ProtoBuf.RespawnInformation.SpawnOptions));
						else if (itemType.IsEquivalentTo(typeof(ProtoBuf.ClientReady.ClientInfo))) listItem = InstantiateType(typeof(ProtoBuf.ClientReady.ClientInfo));
						else listItem = Activator.CreateInstance(itemType);
						var list = Activator.CreateInstance(field.FieldType);
						field.FieldType.GetMethod("Add").Invoke(list, new[] { listItem });
						field.SetValue(element, list);
					} else if (typeof(IProto).IsAssignableFrom(field.FieldType)) {
						field.SetValue(element, InstantiateType(field.FieldType));
					} else field.SetValue(element, Activator.CreateInstance(field.FieldType));
				}
				return element;
			} catch (Exception ex) {
				Console.WriteLine(ex);
			}
			return null;
		}

		public static void GenerateMegaProto() {
			Assembly rustData = Assembly.LoadFrom("Rust.Data.dll");
			var types = rustData.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IProto)));
			StringBuilder megaFile = new StringBuilder();
			foreach (var type in types) {
				String tmp = GenerateProtoBuf(type);
				if (tmp == null) continue;
				megaFile.AppendLine(tmp);
			}
			File.WriteAllText("mega.proto", megaFile.ToString());
		}

		static Key GetKeySkipData(Stream stream) {
			Key key = ProtocolParser.ReadKey(stream);
			ProtocolParser.SkipKey(stream, key);
			return key;
		}

		public static String GenerateProtoBuf(Type protoBufClass) {
			//UInt32 fieldID = 1;
			StringBuilder str = new StringBuilder();
			//List<String> imports = new List<String>();
			//String header = "syntax = \"proto3\";" + Environment.NewLine;
			//str.Append(header);
			str.AppendLine(String.Format("message {0} {{", protoBufClass.Name));
			/*Action<String> addImport = new Action<String>(import => {
				String importStr = String.Format("import \"{0}.proto\";", import);
				if (!imports.Contains(importStr, StringComparer.OrdinalIgnoreCase)) {
					imports.Add(importStr);
				}
			}); //*/
			if (!typeof(IProto).IsAssignableFrom(protoBufClass)) return null;
			var element = InstantiateType(protoBufClass);
			MemoryStream stream = new MemoryStream();
			try {
				((IProto)element).WriteToStream(stream);
			} catch (Exception ex) {
				Console.WriteLine("Failed to write: {0}", element);
				Console.WriteLine(ex);
				return "";
			}
			stream.Position = 0;
			FieldInfo[] members = element.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var memberInfo in members) {
				if (memberInfo == null) continue;
				if (memberInfo.Name.Equals("ShouldPool")) continue;
				var type = memberInfo.FieldType;
				var fieldName = memberInfo.FieldType.Name;
				if (typeof(IProto).IsAssignableFrom(type)) {
					//addImport(type.Name);
					fieldName = type.Name;
				} else if (type.IsValueType || type == typeof(String) || type == typeof(Byte[]) || typeof(IList).IsAssignableFrom(type)) {
					if (typeof(Boolean).IsEquivalentTo(type)) fieldName = "bool";
					else if (typeof(Single).IsEquivalentTo(type)) fieldName = "float";
					else if (typeof(Byte[]).IsEquivalentTo(type)) fieldName = "bytes";
					else if (typeof(Vector3).IsEquivalentTo(type)) {
						fieldName = "Vector3Serialized";
						//addImport("UnityEngine/Vector3Serialized");
					} else if (typeof(Ray).IsEquivalentTo(type)) {
						fieldName = "RaySerialized";
						//addImport("UnityEngine/RaySerialized");
					} else if (typeof(IList).IsAssignableFrom(type)) {
						var itemType = type.GetGenericArguments()[0];
						var itemTypeName = itemType.Name;
						if (itemType.IsValueType) {
							itemTypeName = itemTypeName.ToLower();
							if (itemType == typeof(Single)) itemTypeName = "float";
						}/* else if (typeof(IProto).IsAssignableFrom(itemType)) {
							addImport(itemTypeName);
						}//*/
						fieldName = "repeated " + itemTypeName;
					} else if(type.IsValueType) fieldName = fieldName.ToLower();
				}
				Key key = GetKeySkipData(stream);
				str.AppendLine(String.Format("\t{0} {1} = {2};", fieldName, memberInfo.Name, key.Field));
			}
			str.AppendLine("}");
			//imports.Sort(StringComparer.OrdinalIgnoreCase);
			//String importString = Environment.NewLine + String.Join(Environment.NewLine, imports) + Environment.NewLine + Environment.NewLine;
			//str.Insert(header.Length, importString);
			return str.ToString();
		}
	}
}
