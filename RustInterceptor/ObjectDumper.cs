/*
 * Slighty modified ObjectDumper from http://stackoverflow.com/a/10478008
 * Modified to dump types instead of values.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rust_Interceptor {
	public class ObjectDumper {
		private int _level;
		private readonly int _indentSize;
		private readonly StringBuilder _stringBuilder;
		private readonly List<int> _hashListOfFoundElements;

		private ObjectDumper(int indentSize) {
			_indentSize = indentSize;
			_stringBuilder = new StringBuilder();
			_hashListOfFoundElements = new List<int>();
		}

		public static string Dump(object element) {
			return Dump(element, 2);
		}

		public static string Dump(object element, int indentSize) {
			var instance = new ObjectDumper(indentSize);
			return instance.DumpElement(element);
		}

		private string DumpElement(object element) {
			if (element is ValueType || element is string) {
				Write(FormatValue(element));
			} else {
				var objectType = element.GetType();
				if (!typeof(IEnumerable).IsAssignableFrom(objectType)) {
					Write("{{{0}}}", objectType.FullName);
					_hashListOfFoundElements.Add(element.GetHashCode());
					_level++;
				}
				var isList = false;
				var enumerableElement = element as IEnumerable;
				if (enumerableElement != null) {
					foreach (object item in enumerableElement) {
						if (item is IEnumerable && !(item is string)) {
							_level++;
							DumpElement(item);
							_level--;
						} else {
							if (!AlreadyTouched(item))
								DumpElement(item);
							else
								Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
						}
					}
				} else {
					MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
					foreach (var memberInfo in members) {
						if (memberInfo.Name.Equals("ShouldPool")) continue;
						var fieldInfo = memberInfo as FieldInfo;
						var propertyInfo = memberInfo as PropertyInfo;

						if (fieldInfo == null && propertyInfo == null)
							continue;

						var type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
						object value = fieldInfo != null
										   ? fieldInfo.GetValue(element)
										   : propertyInfo.GetValue(element, null);
						isList = typeof(IList).IsAssignableFrom(type) && type.Name.StartsWith("List");
						if (type.IsValueType || type == typeof(string)) {
							Write("{0}: {1}", memberInfo.Name, FormatValue(value));
						} else {
							if (isList) {
								Write("{0}: List<{1}>", memberInfo.Name, type.GetGenericArguments()[0]);
							} else {
								var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
								Write("{0}:", memberInfo.Name);
								var alreadyTouched = !isEnumerable && AlreadyTouched(value);
								_level++;
								if (!alreadyTouched) {
									if (value == null) {
										if (type == typeof(Byte[])) value = new Byte[0];
										else value = Activator.CreateInstance(type);
									}
									DumpElement(value);
								} else
									Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
								_level--;
							}
						}

					}
				}

				if (!typeof(IEnumerable).IsAssignableFrom(objectType) && !isList) {
					_level--;
				}
			}

			return _stringBuilder.ToString();
		}

		private bool AlreadyTouched(object value) {
			if (value == null)
				return false;

			var hash = value.GetHashCode();
			for (var i = 0; i < _hashListOfFoundElements.Count; i++) {
				if (_hashListOfFoundElements[i] == hash)
					return true;
			}
			return false;
		}

		private void Write(string value, params object[] args) {
			var space = new string(' ', _level * _indentSize);

			if (args != null)
				value = string.Format(value, args);

			_stringBuilder.AppendLine(space + value);
		}

		private string FormatValue(object o) {
			if (o == null)
				return ("String");
			return (o.GetType().Name);
		}
	}
}
