using LibPegasus.Packets;
using Nito.Collections;
using Serilog;
using WorldServer.Enums;
using WorldServer.Logic;
using WorldServer.Packets.C2S;

namespace WorldServer.Packets
{
	internal class PacketManager
	{
		private Queue<Tuple<UInt16, Queue<byte>>> _decryptedInboundPackets = new();
		private Queue<Deque<byte>> _decryptedOutboundPackets = new();
		public DanglingPacket? DanglingPacket = null;
		// Start is called before the first frame update
		public PacketManager()
		{
		}

		public void EnqueuePacket(UInt16 opcode, Queue<byte> packet)
		{
			_decryptedInboundPackets.Enqueue(new Tuple<UInt16, Queue<byte>>(opcode, packet));
		}

		public Deque<byte> GetOutboundPacket()
		{
			return _decryptedOutboundPackets.Dequeue();
		}

		public bool OutputQueued()
		{
			return _decryptedOutboundPackets.Count > 0;
		}

		public void Send(PacketS2C packet)
		{
			var p = packet.Send();
			_decryptedOutboundPackets.Enqueue(p);
		}

		private PacketC2S<Client> GetPacket(Opcode opcode, Queue<byte> data)
		{
			return opcode switch
			{
				Opcode.CONNECT2SVR => new REQ_Connect2Svr(data),
				Opcode.CHARGEINFO => new REQ_ChargeInfo(data),
				Opcode.GETMYCHARTR => new REQ_GetMyChartr(data),
				Opcode.GETSVRTIME => new REQ_GetSvrTime(data),
				Opcode.SERVERENV => new REQ_ServerEnv(data),
				Opcode.VERIFYLINKS => new REQ_VerifyLinks(data),
				Opcode.NEWMYCHARTR => new REQ_NewMyChartr(data),
				_ => throw new NotImplementedException($"unimplemented opcode {opcode}"),
			}; ;
		}

		public Queue<Action<Client>>? ReceiveAll(bool isAuthenticated)
		{
			Queue<Action<Client>>? actions = null;

			if (_decryptedInboundPackets.Count > 0)
			{
				var upcomingOpcode = _decryptedInboundPackets.Peek().Item1;
				if (upcomingOpcode != (UInt16)Opcode.CONNECT2SVR && !isAuthenticated)
				{
					//TODO: User tried to do things while not logged in
					return null;
				}
			}

			while (_decryptedInboundPackets.Count > 0)
			{
				if (actions == null)
					actions = new Queue<Action<Client>>();

				var packetInfo = _decryptedInboundPackets.Dequeue();
				var opcodeNum = packetInfo.Item1;
				var dataQueue = packetInfo.Item2;

				bool opcodeDefined = Enum.IsDefined(typeof(Opcode), opcodeNum);
				if (!opcodeDefined)
				{
					Log.Warning($"Received undefined opcode {opcodeNum}(len={dataQueue.Count})");
					continue;
				}

				var packet = GetPacket((Opcode)opcodeNum, dataQueue);
				Log.Debug($"Processing opcode {opcodeNum}");

				bool verifyHeader = packet.ReadHeader();
				if (!verifyHeader)
				{
					Log.Warning($"Header for opcode {opcodeNum} is invalid");
					continue;
				}

				bool ok = packet.ReadPayload(actions);
				if (!ok)
				{
					Log.Warning($"Invalid payload data during opcode {opcodeNum}");
					continue;
				}

				bool verifyReceived = packet.Verify();
				if (!verifyReceived)
				{
					Log.Warning($"Data of opcode {opcodeNum} was not fully read");
					continue;
				}
			}
			return actions;
		}
	}
}
