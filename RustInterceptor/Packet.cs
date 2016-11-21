using System;
using System.IO;
using Facepunch.Network.Raknet;
using Newtonsoft.Json;

namespace Rust_Interceptor {
	[JsonConverter(typeof(Serializer.PacketConverter))]
	public class Packet : Network.Read {
		#region ENUM

		public enum RakNet : byte {
			CONNECTED_PING,
			UNCONNECTED_PING,
			UNCONNECTED_PING_OPEN_CONNECTIONS,
			CONNECTED_PONG,
			DETECT_LOST_CONNECTIONS,
			OPEN_CONNECTION_REQUEST_1,
			OPEN_CONNECTION_REPLY_1,
			OPEN_CONNECTION_REQUEST_2,
			OPEN_CONNECTION_REPLY_2,
			CONNECTION_REQUEST,
			REMOTE_SYSTEM_REQUIRES_PUBLIC_KEY,
			OUR_SYSTEM_REQUIRES_SECURITY,
			PUBLIC_KEY_MISMATCH,
			OUT_OF_BAND_INTERNAL,
			SND_RECEIPT_ACKED,
			SND_RECEIPT_LOSS,
			CONNECTION_REQUEST_ACCEPTED,
			CONNECTION_ATTEMPT_FAILED,
			ALREADY_CONNECTED,
			NEW_INCOMING_CONNECTION,
			NO_FREE_INCOMING_CONNECTIONS,
			DISCONNECTION_NOTIFICATION,
			CONNECTION_LOST,
			CONNECTION_BANNED,
			INVALID_PASSWORD,
			INCOMPATIBLE_PROTOCOL_VERSION,
			IP_RECENTLY_CONNECTED,
			TIMESTAMP,
			UNCONNECTED_PONG,
			ADVERTISE_SYSTEM,
			DOWNLOAD_PROGRESS,
			REMOTE_DISCONNECTION_NOTIFICATION,
			REMOTE_CONNECTION_LOST,
			REMOTE_NEW_INCOMING_CONNECTION,
			FILE_LIST_TRANSFER_HEADER,
			FILE_LIST_TRANSFER_FILE,
			FILE_LIST_REFERENCE_PUSH_ACK,
			DDT_DOWNLOAD_REQUEST,
			TRANSPORT_STRING,
			REPLICA_MANAGER_CONSTRUCTION,
			REPLICA_MANAGER_SCOPE_CHANGE,
			REPLICA_MANAGER_SERIALIZE,
			REPLICA_MANAGER_DOWNLOAD_STARTED,
			REPLICA_MANAGER_DOWNLOAD_COMPLETE,
			RAKVOICE_OPEN_CHANNEL_REQUEST,
			RAKVOICE_OPEN_CHANNEL_REPLY,
			RAKVOICE_CLOSE_CHANNEL,
			RAKVOICE_DATA,
			AUTOPATCHER_GET_CHANGELIST_SINCE_DATE,
			AUTOPATCHER_CREATION_LIST,
			AUTOPATCHER_DELETION_LIST,
			AUTOPATCHER_GET_PATCH,
			AUTOPATCHER_PATCH_LIST,
			AUTOPATCHER_REPOSITORY_FATAL_ERROR,
			AUTOPATCHER_CANNOT_DOWNLOAD_ORIGINAL_UNMODIFIED_FILES,
			AUTOPATCHER_FINISHED_INTERNAL,
			AUTOPATCHER_FINISHED,
			AUTOPATCHER_RESTART_APPLICATION,
			NAT_PUNCHTHROUGH_REQUEST,
			NAT_CONNECT_AT_TIME,
			NAT_GET_MOST_RECENT_PORT,
			NAT_CLIENT_READY,
			NAT_TARGET_NOT_CONNECTED,
			NAT_TARGET_UNRESPONSIVE,
			NAT_CONNECTION_TO_TARGET_LOST,
			NAT_ALREADY_IN_PROGRESS,
			NAT_PUNCHTHROUGH_FAILED,
			NAT_PUNCHTHROUGH_SUCCEEDED,
			READY_EVENT_SET,
			READY_EVENT_UNSET,
			READY_EVENT_ALL_SET,
			READY_EVENT_QUERY,
			LOBBY_GENERAL,
			RPC_REMOTE_ERROR,
			RPC_PLUGIN,
			FILE_LIST_REFERENCE_PUSH,
			READY_EVENT_FORCE_ALL_SET,
			ROOMS_EXECUTE_FUNC,
			ROOMS_LOGON_STATUS,
			ROOMS_HANDLE_CHANGE,
			LOBBY2_SEND_MESSAGE,
			LOBBY2_SERVER_ERROR,
			FCM2_NEW_HOST,
			FCM2_REQUEST_FCMGUID,
			FCM2_RESPOND_CONNECTION_COUNT,
			FCM2_INFORM_FCMGUID,
			FCM2_UPDATE_MIN_TOTAL_CONNECTION_COUNT,
			FCM2_VERIFIED_JOIN_START,
			FCM2_VERIFIED_JOIN_CAPABLE,
			FCM2_VERIFIED_JOIN_FAILED,
			FCM2_VERIFIED_JOIN_ACCEPTED,
			FCM2_VERIFIED_JOIN_REJECTED,
			UDP_PROXY_GENERAL,
			SQLite3_EXEC,
			SQLite3_UNKNOWN_DB,
			SQLLITE_LOGGER,
			NAT_TYPE_DETECTION_REQUEST,
			NAT_TYPE_DETECTION_RESULT,
			ROUTER_2_INTERNAL,
			ROUTER_2_FORWARDING_NO_PATH,
			ROUTER_2_FORWARDING_ESTABLISHED,
			ROUTER_2_REROUTED,
			TEAM_BALANCER_INTERNAL,
			TEAM_BALANCER_REQUESTED_TEAM_FULL,
			TEAM_BALANCER_REQUESTED_TEAM_LOCKED,
			TEAM_BALANCER_TEAM_REQUESTED_CANCELLED,
			TEAM_BALANCER_TEAM_ASSIGNED,
			LIGHTSPEED_INTEGRATION,
			XBOX_LOBBY,
			TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_SUCCESS,
			TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_SUCCESS,
			TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_FAILURE,
			TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_FAILURE,
			TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_TIMEOUT,
			TWO_WAY_AUTHENTICATION_NEGOTIATION,
			CLOUD_POST_REQUEST,
			CLOUD_RELEASE_REQUEST,
			CLOUD_GET_REQUEST,
			CLOUD_GET_RESPONSE,
			CLOUD_UNSUBSCRIBE_REQUEST,
			CLOUD_SERVER_TO_SERVER_COMMAND,
			CLOUD_SUBSCRIPTION_NOTIFICATION,
			LIB_VOICE,
			RELAY_PLUGIN,
			NAT_REQUEST_BOUND_ADDRESSES,
			NAT_RESPOND_BOUND_ADDRESSES,
			FCM2_UPDATE_USER_CONTEXT,
			RESERVED_3,
			RESERVED_4,
			RESERVED_5,
			RESERVED_6,
			RESERVED_7,
			RESERVED_8,
			RESERVED_9,
			USER_PACKET_ENUM
		}

		public enum Rust : byte {
			Welcome = 1,
			Auth = 2,
			Approved = 3,
			Ready = 4,
			Entities = 5,
			EntityDestroy = 6,
			GroupChange = 7,
			GroupDestroy = 8,
			RPCMessage = 9,
			EntityPosition = 10,
			ConsoleMessage = 11,
			ConsoleCommand = 12,
			Effect = 13,
			DisconnectReason = 14,
			Tick = 15,
			Message = 16,
			RequestUserInformation = 17,
			GiveUserInformation = 18,
			GroupEnter = 19,
			GroupLeave = 20,
			Last = 20
		}

		public enum PacketType : byte {
			NONE = 255,
			RAKNET = 0,
			RUST = 140
		}
		#endregion
		public MemoryStream baseStream = new MemoryStream();
		public ulong incomingGUID;
		public int incomingLength;

		public byte packetID;

		public Rust rustID {
			get { return (Rust)packetID; }
		}

		public PacketType type = PacketType.NONE;
		public long delay = 0;


		internal bool Send(RakNetPeer peer, ulong guid) {
			peer.SendStart();
			peer.WriteBytes(baseStream.ToArray(), 0, incomingLength);
			peer.SendTo(guid, RakNetPeer.Priority.Medium, RakNetPeer.SendMethod.Reliable, 0);
			return true;
		}

		public string GetName() {
			return type == PacketType.RAKNET ? Enum.GetName(typeof(RakNet), packetID) : Enum.GetName(typeof(Rust), packetID);
		}

		public string GetPacketTypeName() {
			return type == PacketType.RAKNET ? "RAKNET" : "RUST";
		}

		public override string ToString() {
			return string.Format("PacketOrigin: {0}\nPacketType: {1}\nPacketName: {2}\nPacketID: {3}\nPacketLength: {4}", GetOrigin(), GetPacketTypeName(), GetName(), packetID, incomingLength);
		}

		internal bool Receive(RakNetPeer peer) {
			incomingLength = peer.incomingBytes;
			incomingGUID = peer.incomingGUID;
			Position = 0;
			MemoryStream mem = peer.ReadBytes(incomingLength);
			mem.Position = 0;
			mem.WriteTo(baseStream);
			SetLength(incomingLength);
			Position = 0;
			packetID = UInt8();
			type = packetID < 140 ? PacketType.RAKNET : PacketType.RUST;
			packetID -= (byte)type;
			return true;
		}

		internal void Clear() {
			Position = 0;
			SetLength(0);
			packetID = 0;
			incomingLength = 0;
			type = PacketType.NONE;
		}

		public static string GetOrigin(ulong guid) {
			return string.Format("{0}", guid == RustInterceptor.clientGUID ? "Client" : "Server");
		}

		public static string GetOrigin(Packet packet) {
			return GetOrigin(packet.incomingGUID);
		}

		public string GetOrigin() {
			return GetOrigin(incomingGUID);
		}

		internal Packet Clone() {
			var ret = new Packet();
			ret.incomingGUID = incomingGUID;
			ret.incomingLength = incomingLength;
			ret.packetID = packetID;
			ret.type = type;
			ret.baseStream.Position = 0;
			ret.baseStream.Write(baseStream.ToArray(), 0, incomingLength);
			ret.Position = 1;
			return ret;
		}

		/* Packet Stream Reader */
		public override long Position {
			get {
				return baseStream.Position;
			}
			set { baseStream.Position = value; }
		}

		public override long Length {
			get {
				return incomingLength;
			}
		}

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

		public override int length {
			get {
				return (int)baseStream.Length;
			}
		}

		public override int position {
			get { return (int)baseStream.Position; }
		}

		public override int unread {
			get { return (int)(incomingLength - baseStream.Position); }
		}

		public override void Write(byte[] buffer, int offset, int count) {
			baseStream.Write(buffer, offset, count);
		}

		public void WriteTo(Stream stream) {
			var packet = (Packet)stream;
			packet.Clear();
			baseStream.WriteTo(packet);
		}

		public override void Flush() {
			baseStream.Flush();
		}

		public override int ReadByte() {
			if (baseStream.Position == Length)
				return -1;
			return (int)baseStream.ReadByte();
		}

		public override long Seek(long offset, SeekOrigin origin) {
			return baseStream.Seek(offset, origin);
		}

		public override void SetLength(long value) {
			baseStream.SetLength(value);
		}

		public override byte UInt8() {
			return (byte)ReadByte();
		}

		public override bool Bit() {
			return UInt8() > 0U;
		}

		public override sbyte Int8() {
			return (sbyte)UInt8();
		}

		public override long Int64() {
			return BitConverter.ToInt64(Read(8), 0);
		}

		public override int Int32() {
			return BitConverter.ToInt32(Read(4), 0);
		}

		public override short Int16() {
			return BitConverter.ToInt16(Read(2), 0);
		}

		public override ulong UInt64() {
			return BitConverter.ToUInt64(Read(8), 0);
		}

		public override uint UInt32() {
			return BitConverter.ToUInt32(Read(4), 0);
		}

		public override ushort UInt16() {
			return BitConverter.ToUInt16(Read(2), 0);
		}

		public override float Float(bool unused = false) {
			return BitConverter.ToSingle(Read(4), 0);
		}

		public override double Double() {
			return BitConverter.ToDouble(Read(8), 0);
		}

		public byte[] Read(int size) {
			byte[] data = new byte[size];
			baseStream.Read(data, 0, size);
			if (!BitConverter.IsLittleEndian && data.Length > 1) Array.Reverse(data);
			return data;
		}

		public override int Read(byte[] buffer, int offset, int count) {
			return baseStream.Read(buffer, offset, count);
		}

		public new byte[] BytesWithSize() {
			uint num = UInt32();
			if (num == 0)
				return null;
			if (num > 10485760U)
				return null;
			byte[] buffer = new byte[num];
			if (baseStream.Read(buffer, 0, (int)num) != num)
				return (byte[])null;
			return buffer;
		}

	}
}