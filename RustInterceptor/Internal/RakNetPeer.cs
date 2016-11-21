using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Facepunch.Network.Raknet {
	[SuppressUnmanagedCodeSecurity]
	internal class RakNetPeer {
		public enum PacketReliability {
			UNRELIABLE,
			UNRELIABLE_SEQUENCED,
			RELIABLE,
			RELIABLE_ORDERED,
			RELIABLE_SEQUENCED,
			UNRELIABLE_WITH_ACK_RECEIPT,
			RELIABLE_WITH_ACK_RECEIPT,
			RELIABLE_ORDERED_WITH_ACK_RECEIPT
		}

		public enum Priority {
			Immediate,
			High,
			Medium,
			Low
		}

		public enum SendMethod {
			Reliable,
			ReliableUnordered,
			ReliableSequenced,
			Unreliable,
			UnreliableSequenced
		}

		public enum StatTypeLong {
			BytesSent,
			BytesSent_LastSecond,
			BytesReceived,
			BytesReceived_LastSecond,
			MessagesInSendBuffer,
			BytesInSendBuffer,
			MessagesInResendBuffer,
			BytesInResendBuffer,
			PacketLossAverage,
			PacketLossLastSecond,
			ThrottleBytes
		}

		private static readonly byte[] ReadBuffer = new byte[1024];
		private static readonly MemoryStream memoryStream = new MemoryStream();
		private static readonly byte[] ByteBuffer = new byte[512];
		private IntPtr ptr;

		public ulong incomingGUID {
			get {
				Check();
				return RakNetNative.NETRCV_GUID(ptr);
			}
		}

		public uint incomingAddressInt {
			get {
				Check();
				return RakNetNative.NETRCV_Address(ptr);
			}
		}

		public uint incomingPort {
			get {
				Check();
				return RakNetNative.NETRCV_Port(ptr);
			}
		}

		public string incomingAddress {
			get {
				Check();
				return GetAddress(incomingGUID);
			}
		}

		public int incomingBits {
			get {
				Check();
				return RakNetNative.NETRCV_LengthBits(ptr);
			}
		}

		public int incomingBitsUnread {
			get {
				Check();
				return RakNetNative.NETRCV_UnreadBits(ptr);
			}
		}

		public int incomingBytes {
			get {
				Check();
				return incomingBits / 8;
			}
		}

		public int incomingBytesUnread {
			get {
				Check();
				return incomingBitsUnread / 8;
			}
		}

		public static RakNetPeer CreateServer(string ip, int port, int maxConnections) {
			var peer = new RakNetPeer();
			peer.ptr = RakNetNative.NET_Create();
			if (RakNetNative.NET_StartServer(peer.ptr, ip, port, maxConnections) == 0)
				return peer;
			peer.Close();
			var str = StringFromPointer(RakNetNative.NET_LastStartupError(peer.ptr));
			Console.WriteLine((object)("Couldn't create server on port " + port + " (" + str + ")"));
			return null;
		}

		public static RakNetPeer CreateConnection(string hostname, int port, int retries, int retryDelay, int timeout) {
			var peer = new RakNetPeer();
			peer.ptr = RakNetNative.NET_Create();
			if (RakNetNative.NET_StartClient(peer.ptr, hostname, port, retries, retryDelay, timeout) == 0)
				return peer;
			var str = StringFromPointer(RakNetNative.NET_LastStartupError(peer.ptr));
			Console.WriteLine((object)("Couldn't connect to server " + hostname + ":" + port + " (" + str + ")"));
			peer.Close();
			return null;
		}

		public void Close() {
			if (!(ptr != IntPtr.Zero))
				return;
			RakNetNative.NET_Close(ptr);
			ptr = IntPtr.Zero;
		}

		public bool Receive() {
			if (ptr == IntPtr.Zero)
				return false;
			return RakNetNative.NET_Receive(ptr);
		}

		public void SetReadPos(int bitsOffset) {
			RakNetNative.NETRCV_SetReadPointer(ptr, bitsOffset);
		}

		public unsafe int Read(byte[] buffer, int offset, int length) {
			if ((uint)offset > 0U)
				throw new NotImplementedException("Offset != 0");
			fixed (byte* data = buffer) {
				if (!RakNetNative.NETRCV_ReadBytes(ptr, data, length)) {
					Console.WriteLine("NETRCV_ReadBytes returned false");
					return 0;
				}
				return length;
			}
		}

		public byte ReadUInt8() {
			return ReadByte();
		}

		public bool ReadBit() {
			return ReadUInt8() > 0U;
		}

		public sbyte ReadInt8() {
			return (sbyte)ReadUInt8();
		}

		public unsafe long ReadInt64() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(long*)Read(8, data);
			}
		}

		public unsafe int ReadInt32() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(int*)Read(4, data);
			}
		}

		public unsafe short ReadInt16() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(short*)Read(2, data);
			}
		}

		public unsafe ulong ReadUInt64() {
			fixed (byte* data = &ReadBuffer[0]) {
				return (ulong)*(long*)Read(8, data);
			}
		}

		public unsafe uint ReadUInt32() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(uint*)Read(4, data);
			}
		}

		public unsafe ushort ReadUInt16() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(ushort*)Read(2, data);
			}
		}

		public unsafe float ReadFloat(bool compressed) {
			if (compressed)
				return RakNetNative.NETSND_ReadCompressedFloat(ptr);
			fixed (byte* data = &ReadBuffer[0]) {
				return *(float*)Read(4, data);
			}
		}

		public unsafe double ReadDouble() {
			fixed (byte* data = &ReadBuffer[0]) {
				return *(double*)Read(8, data);
			}
		}

		public unsafe byte ReadByte() {
			Check();
			fixed (byte* data = ByteBuffer) {
				if (!RakNetNative.NETRCV_ReadBytes(ptr, data, 1)) {
					Console.WriteLine((object)"NETRCV_ReadBytes returned false");
					return 0;
				}
				return ByteBuffer[0];
			}
		}

		public unsafe MemoryStream ReadBytes(int length) {
			Check();
			if (length == -1)
				length = incomingBytesUnread;
			if (memoryStream.Capacity < length)
				memoryStream.Capacity = length + 32;
			fixed (byte* data = memoryStream.GetBuffer()) {
				memoryStream.SetLength(memoryStream.Capacity);
				if (!RakNetNative.NETRCV_ReadBytes(ptr, data, length)) {
					Console.WriteLine((object)"NETRCV_ReadBytes returned false");
					return null;
				}
				memoryStream.SetLength(length);
				return memoryStream;
			}
		}

		private unsafe byte* Read(int size, byte* data) {
			Check();
			if (size > ReadBuffer.Length)
				throw new Exception("Size > ReadBuffer.Length");
			if (RakNetNative.NETRCV_ReadBytes(ptr, data, size))
				return data;
			Console.WriteLine((object)"NETRCV_ReadBytes returned false");
			return null;
		}

		public void SendStart() {
			Check();
			RakNetNative.NETSND_Start(ptr);
		}

		public void WriteBool(bool val) {
			WriteUInt8(val ? (byte)1 : (byte)0);
		}

		public unsafe void WriteUInt16(ushort val) {
			Write((byte*)&val, 2);
		}

		public unsafe void WriteUInt32(uint val) {
			Write((byte*)&val, 4);
		}

		public unsafe void WriteUInt64(ulong val) {
			Write((byte*)&val, 8);
		}

		public unsafe void WriteInt8(sbyte val) {
			Write((byte*)&val, 1);
		}

		public unsafe void WriteInt16(short val) {
			Write((byte*)&val, 2);
		}

		public unsafe void WriteInt32(int val) {
			Write((byte*)&val, 4);
		}

		public unsafe void WriteInt64(long val) {
			Write((byte*)&val, 8);
		}

		public unsafe void WriteFloat(float val, bool compressed) {
			if (compressed)
				RakNetNative.NETSND_WriteCompressedFloat(ptr, val);
			else
				Write((byte*)&val, 4);
		}

		public unsafe void WriteDouble(double val) {
			Write((byte*)&val, 8);
		}

		public unsafe void WriteUInt8(byte val) {
			Write(&val, 1);
		}

		public unsafe void WriteBytes(byte[] val, int offset, int length) {
			if ((uint)offset > 0U)
				throw new NotSupportedException("offset != 0");
			fixed (byte* data = val) {
				Write(data, length);
			}
		}

		public unsafe void WriteBytes(byte[] val) {
			fixed (byte* data = val) {
				Write(data, val.Length);
			}
		}

		private unsafe void Write(byte* data, int size) {
			if ((IntPtr)data == IntPtr.Zero)
				throw new InvalidOperationException("data is NULL");
			Check();
			RakNetNative.NETSND_WriteBytes(ptr, data, size);
		}

		public uint SendBroadcast(Priority priority, SendMethod reliability, sbyte channel) {
			Check();
			return RakNetNative.NETSND_Broadcast(ptr, ToRaknetPriority(priority), ToRaknetPacketReliability(reliability), channel);
		}

		public uint SendTo(ulong guid, Priority priority, SendMethod reliability, sbyte channel) {
			Check();
			return RakNetNative.NETSND_Send(ptr, guid, ToRaknetPriority(priority), ToRaknetPacketReliability(reliability), channel);
		}

		public unsafe void SendUnconnectedMessage(byte* data, int length, uint adr, ushort port) {
			Check();
			RakNetNative.NET_SendMessage(ptr, data, length, adr, port);
		}

		public string GetAddress(ulong guid) {
			Check();
			return StringFromPointer(RakNetNative.NET_GetAddress(ptr, guid));
		}

		private static string StringFromPointer(IntPtr p) {
			if (p == IntPtr.Zero)
				return string.Empty;
			return Marshal.PtrToStringAnsi(p);
		}

		public int ToRaknetPriority(Priority priority) {
			switch (priority) {
				case Priority.Immediate:
					return 0;
				case Priority.High:
					return 1;
				case Priority.Medium:
					return 2;
				default:
					return 3;
			}
		}

		public int ToRaknetPacketReliability(SendMethod priority) {
			switch (priority) {
				case SendMethod.Reliable:
					return 3;
				case SendMethod.ReliableUnordered:
					return 2;
				case SendMethod.ReliableSequenced:
					return 4;
				case SendMethod.Unreliable:
					return 0;
				case SendMethod.UnreliableSequenced:
					return 1;
				default:
					return 3;
			}
		}

		private void Check() {
			if (ptr == IntPtr.Zero)
				throw new NullReferenceException("Peer has already shut down!");
		}
	}
}