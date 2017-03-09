using SilentOrbit.ProtocolBuffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
			var element = Activator.CreateInstance(type);
			IntPtr baseAddress = TypeDelegator.GetTypeHandle(element).Value;
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields) {
				if (field.FieldType.IsEquivalentTo(typeof(String))) field.SetValue(element, "");
				else if (typeof(IList).IsAssignableFrom(field.FieldType)) {
					var itemType = field.FieldType.GetGenericArguments()[0];
					var listItem = InstantiateType(itemType);
					var list = Activator.CreateInstance(field.FieldType);
					field.FieldType.GetMethod("Add").Invoke(list, new[] { listItem });
					field.SetValue(element, list);
				} else field.SetValue(element, Activator.CreateInstance(field.FieldType));
			}
			return element;
		}

		public static void TryGenerateProtoFiles(String path = @".\proto\") {
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
			Assembly rustData = Assembly.LoadFrom("Rust.Data.dll"); ;
			var types = rustData.GetTypes().Where(item => item.GetInterfaces().Contains(typeof(IProto)));
			foreach (var type in types) {
				String tmp = GenerateProtoBuf(type);
				if (tmp == null) continue;
				File.WriteAllText(Path.Combine(path, type.Name + (partial ? "proto.partial" : ".proto")), tmp);
			}
		}
		static bool partial = false;
		public static String GenerateProtoBuf(Type protoBufClass) {
			partial = false;
			StringBuilder str = new StringBuilder();
			try {
				if (!typeof(IProto).IsAssignableFrom(protoBufClass)) return null;
				var element = InstantiateType(protoBufClass);
				DummmyStream stream = new DummmyStream();
				((IProto)element).WriteToStream(stream);
				byte[] bytes = stream.baseStream.ToArray();
				stream.Position = 0;
				FieldInfo[] members = element.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
				
				String header = "syntax = \"proto3\";" + Environment.NewLine;
				str.Append(header);
				str.AppendLine(String.Format("message {0} {{", protoBufClass.Name));
				int importIndex = header.Length;
				foreach (var memberInfo in members) {
					if (memberInfo.Name.Equals("ShouldPool")) continue;
					if (memberInfo == null) continue;
					var type = memberInfo.FieldType;
					var fieldName = memberInfo.FieldType.Name;
					if (!(type.IsValueType || type.IsAssignableFrom(typeof(String)) || typeof(IList).IsAssignableFrom(type))) {
						String import = String.Format("import \"{0}.proto\";{1}", type.Name, Environment.NewLine);
						str.Insert(importIndex, import);
						importIndex += import.Length;
					} else {
						if (typeof(Boolean).IsEquivalentTo(type)) fieldName = "bool";
						else if (typeof(Single).IsEquivalentTo(type)) fieldName = "float";
						else if (typeof(Byte[]).IsEquivalentTo(type)) fieldName = "bytes";
						else fieldName = fieldName.ToLower();
					}
					int num = stream.ReadByte();
					if (num == -1) {

					}
					Key key = ProtocolParser.ReadKey((byte)num, stream);
					uint fieldId = 0;
					fieldId = key.Field;
					if (typeof(IList).IsAssignableFrom(type)) {
						fieldName = "repeated " + type.GetGenericArguments()[0].Name;
					}
					str.AppendLine(String.Format("\t{0} {1} = {2};", fieldName, memberInfo.Name, fieldId));
					if (type.IsAssignableFrom(typeof(Vector3))) stream.Skip(1);
					else if (type.IsAssignableFrom(typeof(String))) stream.Skip(4);
					else if (typeof(IProto).IsAssignableFrom(type)) {
						stream.Skip(1);
					}
				}
				str.AppendLine("}");
				return str.ToString();
			} catch (Exception) {
				Console.WriteLine("Failed to generate {0}, writing partial proto", protoBufClass.Name);
				partial = true;
				return str.ToString();
			}
		}

		class DummmyStream : Stream {
			public MemoryStream baseStream = new MemoryStream();

			public override bool CanRead {
				get {
					return true;
				}
			}

			public override bool CanSeek {
				get {
					return true;
				}
			}

			public override bool CanWrite {
				get {
					return true;
				}
			}

			public override long Length {
				get {
					return baseStream.Length;
				}
			}

			public override long Position {
				get {
					return baseStream.Position;
				}

				set {
					baseStream.Position = value;
				}
			}

			public override void Flush() {
				baseStream.Flush();
			}

			public override void WriteByte(byte value) {
				baseStream.WriteByte(value);
			}

			public override int ReadByte() {
				return baseStream.ReadByte();
			}

			public override int Read(byte[] buffer, int offset, int count) {
				return count;
			}

			public override long Seek(long offset, SeekOrigin origin) {
				return 0;
			}

			public void Skip(long bytes) {
				baseStream.Seek(bytes, SeekOrigin.Current);
			}

			public override void SetLength(long value) {
				return;
			}

			public override void Write(byte[] buffer, int offset, int count) {
				return;
			}
		}
	}
}
