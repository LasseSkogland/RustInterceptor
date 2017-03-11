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
				var element = Activator.CreateInstance(type);
				IntPtr baseAddress = TypeDelegator.GetTypeHandle(element).Value;
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				foreach (var field in fields) {
					if (field.FieldType.IsEquivalentTo(typeof(String))) field.SetValue(element, "");
					else if (typeof(Byte[]).IsEquivalentTo(field.FieldType))
						field.SetValue(element, new byte[0]);
					else if (typeof(IList).IsAssignableFrom(field.FieldType)) {
						var itemType = field.FieldType.GetGenericArguments()[0];
						var listItem = InstantiateType(itemType);
						var list = Activator.CreateInstance(field.FieldType);
						field.FieldType.GetMethod("Add").Invoke(list, new[] { listItem });
						field.SetValue(element, list);
					} else field.SetValue(element, Activator.CreateInstance(field.FieldType));
				}
				return element;
			} catch (Exception ex) {
				Console.WriteLine(ex);
				Console.ReadKey();
			}
			return null;
		}

		public static void GenerateProtoFiles(String path = @".\proto\") {
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			Assembly rustData = Assembly.LoadFrom("Rust.Data.dll"); ;
			var types = rustData.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IProto)));
			foreach (var type in types) {
				String tmp = GenerateProtoBuf(type);
				if (tmp == null) continue;
				File.WriteAllText(Path.Combine(path, type.Name + (partial ? "proto.partial" : ".proto")), tmp);
			}
		}

		static Key GetKeySkipData(Stream stream) {
			Key key = ProtocolParser.ReadKey(stream);
			ProtocolParser.SkipKey(stream, key);
			return key;
		}

		static bool partial = false;
		public static String GenerateProtoBuf(Type protoBufClass) {
			partial = false;
			UInt32 fieldID = 1;
			StringBuilder str = new StringBuilder();
			List<String> imports = new List<String>();
			String header = "syntax = \"proto3\";" + Environment.NewLine;
			str.Append(header);
			str.AppendLine(String.Format("message {0} {{", protoBufClass.Name));
			Action<String> addImport = new Action<String>(import => {
				if (!imports.Contains(import)) {
					String importStr = String.Format("import \"{0}.proto\";", import);
					imports.Add(importStr);
				}
			});
			if (!typeof(IProto).IsAssignableFrom(protoBufClass)) return null;
			var element = InstantiateType(protoBufClass);
			MemoryStream stream = new MemoryStream();
			((IProto)element).WriteToStream(stream);
			stream.Position = 0;
			FieldInfo[] members = element.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var memberInfo in members) {
				if (memberInfo == null) continue;
				if (memberInfo.Name.Equals("ShouldPool")) continue;
				var type = memberInfo.FieldType;
				var fieldName = memberInfo.FieldType.Name;
				if (!(type.IsValueType || type.IsAssignableFrom(typeof(String)) || typeof(IProto).IsAssignableFrom(type))) {
					addImport(type.Name);
				} else {
					if (typeof(Boolean).IsEquivalentTo(type)) fieldName = "bool";
					else if (typeof(Single).IsEquivalentTo(type)) fieldName = "float";
					else if (typeof(Byte[]).IsEquivalentTo(type)) fieldName = "bytes";
					else if (typeof(Vector3).IsEquivalentTo(type)) {
						fieldName = "Vector3Serialzed";
						addImport("UnityEngine\\Vector3Serialized");
					} else if (typeof(Ray).IsEquivalentTo(type)) {
						fieldName = "RaySerialized";
						addImport("UnityEngine\\RaySerialized");
					} else fieldName = fieldName.ToLower();

					if (typeof(IList).IsAssignableFrom(type)) {
						fieldName = "repeated " + type.GetGenericArguments()[0].Name.ToLower();
					}
					Key key = GetKeySkipData(stream);
					str.AppendLine(String.Format("\t{0} {1} = {2};", fieldName, memberInfo.Name, key.Field));
				}
			}
			str.AppendLine("}");
			imports.Sort(StringComparer.OrdinalIgnoreCase);
			String importString = Environment.NewLine + String.Join(Environment.NewLine, imports) + Environment.NewLine + Environment.NewLine;
			str.Insert(header.Length, importString);
			return str.ToString();
		}
	}
}
