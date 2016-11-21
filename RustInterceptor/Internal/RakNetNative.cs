using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Facepunch.Network.Raknet {
	[SuppressUnmanagedCodeSecurity]
	public class RakNetNative {
		public enum Metrics {
			USER_MESSAGE_BYTES_PUSHED,
			USER_MESSAGE_BYTES_SENT,
			USER_MESSAGE_BYTES_RESENT,
			USER_MESSAGE_BYTES_RECEIVED_PROCESSED,
			USER_MESSAGE_BYTES_RECEIVED_IGNORED,
			ACTUAL_BYTES_SENT,
			ACTUAL_BYTES_RECEIVED,
			RNS_PER_SECOND_METRICS_COUNT
		}

		public enum PacketPriority {
			IMMEDIATE_PRIORITY,
			HIGH_PRIORITY,
			MEDIUM_PRIORITY,
			LOW_PRIORITY,
			NUMBER_OF_PRIORITIES
		}

		[DllImport("RakNet")]
		public static extern IntPtr NET_Create();

		[DllImport("RakNet")]
		public static extern void NET_Close(IntPtr nw);

		[DllImport("RakNet")]
		public static extern int NET_StartClient(IntPtr nw, string hostName, int port, int retries, int retryDelay,
			int timeout);

		[DllImport("RakNet")]
		public static extern int NET_StartServer(IntPtr nw, string ip, int port, int maxConnections);

		[DllImport("RakNet")]
		public static extern IntPtr NET_LastStartupError(IntPtr nw);

		[DllImport("RakNet")]
		public static extern bool NET_Receive(IntPtr nw);

		[DllImport("RakNet")]
		public static extern ulong NETRCV_GUID(IntPtr nw);

		[DllImport("RakNet")]
		public static extern uint NETRCV_Address(IntPtr nw);

		[DllImport("RakNet")]
		public static extern uint NETRCV_Port(IntPtr nw);

		[DllImport("RakNet")]
		public static extern int NETRCV_LengthBits(IntPtr nw);

		[DllImport("RakNet")]
		public static extern int NETRCV_UnreadBits(IntPtr nw);

		[DllImport("RakNet")]
		public static extern unsafe bool NETRCV_ReadBytes(IntPtr nw, byte* data, int length);

		[DllImport("RakNet")]
		public static extern void NETRCV_SetReadPointer(IntPtr nw, int bitsOffset);

		[DllImport("RakNet")]
		public static extern void NETSND_Start(IntPtr nw);

		[DllImport("RakNet")]
		public static extern unsafe void NETSND_WriteBytes(IntPtr nw, byte* data, int length);

		[DllImport("RakNet")]
		public static extern uint NETSND_Size(IntPtr nw);

		[DllImport("RakNet")]
		public static extern uint NETSND_Broadcast(IntPtr nw, int priority, int reliability, int channel);

		[DllImport("RakNet")]
		public static extern uint NETSND_Send(IntPtr nw, ulong connectionID, int priority, int reliability, int channel);

		[DllImport("RakNet")]
		public static extern void NET_CloseConnection(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern IntPtr NET_GetAddress(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern IntPtr NET_GetStatisticsString(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern bool NET_GetStatistics(IntPtr nw, ulong connectionID, ref RaknetStats data, int dataLength);

		[DllImport("RakNet")]
		public static extern int NET_GetAveragePing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern int NET_GetLastPing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern int NET_GetLowestPing(IntPtr nw, ulong connectionID);

		[DllImport("RakNet")]
		public static extern unsafe void NET_SendMessage(IntPtr nw, byte* data, int length, uint adr, ushort port);

		[DllImport("RakNet")]
		public static extern void NETSND_WriteCompressedInt32(IntPtr nw, int val);

		[DllImport("RakNet")]
		public static extern void NETSND_WriteCompressedInt64(IntPtr nw, long val);

		[DllImport("RakNet")]
		public static extern void NETSND_WriteCompressedFloat(IntPtr nw, float val);

		[DllImport("RakNet")]
		public static extern int NETSND_ReadCompressedInt32(IntPtr nw);

		[DllImport("RakNet")]
		public static extern long NETSND_ReadCompressedInt64(IntPtr nw);

		[DllImport("RakNet")]
		public static extern float NETSND_ReadCompressedFloat(IntPtr nw);

		[DllImport("RakNet")]
		public static extern unsafe int NETRCV_ReadCompressedBytes(IntPtr nw, int size, byte* receiveBuffer, int expectedSize);

		[DllImport("RakNet")]
		public static extern unsafe int NETSND_WriteCompressedBytes(IntPtr nw, byte* data, int size);

		public struct RaknetStats {
			public unsafe fixed ulong valueOverLastSecond[7];
			public unsafe fixed ulong runningTotal[7];
			public ulong connectionStartTime;
			public byte isLimitedByCongestionControl;
			public ulong BPSLimitByCongestionControl;
			public byte isLimitedByOutgoingBandwidthLimit;
			public ulong BPSLimitByOutgoingBandwidthLimit;
			public unsafe fixed uint messageInSendBuffer[4];
			public unsafe fixed double bytesInSendBuffer[4];
			public uint messagesInResendBuffer;
			public ulong bytesInResendBuffer;
			public float packetlossLastSecond;
			public float packetlossTotal;
		}
	}
}