using LibPegasus.Packets;
using Nito.Collections;
using Serilog;
using WorldServer.Enums;

namespace WorldServer
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
				_ => throw new NotImplementedException($"unimplemented opcode {opcode}"),
			}; ;
		}

		public Queue<Action<Client>>? ReceiveAll()
		{
			Queue<Action<Client>>? actions = null;

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
