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
				Opcode.CSC_CONNECT2SVR => new REQ_Connect2Svr(data),
				Opcode.CSC_CHARGEINFO => new REQ_ChargeInfo(data),
				Opcode.CSC_GETMYCHARTR => new REQ_GetMyChartr(data),
				Opcode.CSC_GETSVRTIME => new REQ_GetSvrTime(data),
				Opcode.CSC_SERVERENV => new REQ_ServerEnv(data),
				Opcode.CSC_VERIFYLINKS => new REQ_VerifyLinks(data),
				Opcode.CSC_NEWMYCHARTR => new REQ_NewMyChartr(data),
				Opcode.CSC_SUBPASSWORDCHECKREQUEST => new REQ_SubPasswordCheckRequest(data),
				Opcode.CSC_SUBPASSWORDSET => new REQ_SubPasswordSet(data),
				Opcode.CSC_SUBPASSWORDCHECK => new REQ_SubPasswordCheck(data),
				Opcode.CSC_INITIALIZED => new REQ_Initialized(data),
				Opcode.CSC_QUERYCASHITEM => new REQ_QueryCashItem(data),
				Opcode.CSC_CHANGESTYLE => new REQ_ChangeStyle(data),
				Opcode.CSC_UPDATEHELPINFO => new REQ_UpdateHelpInfo(data),
				Opcode.CSC_HEARTBEAT => new RSP_Heartbeat(data),
				Opcode.REQ_MOVEBEGINED => new REQ_MoveBegined(data),
				Opcode.REQ_MOVEENDED00 => new REQ_MoveEnded00(data),
				Opcode.REQ_MOVETILEPOS => new REQ_MoveTilePos(data),
				Opcode.REQ_MOVECHANGED => new REQ_MoveChanged(data),
				Opcode.REQ_MOVEWAYPNTS => new REQ_MoveWayPnts(data),
				Opcode.CSC_WARPCOMMAND => new REQ_WarpCommand(data),
				Opcode.CSC_BACKTOCHARLOBBY => new REQ_BackToCharLobby(data),
				Opcode.CSC_UNINITIALZE => new REQ_Uninitalze(data),
				Opcode.CSC_NPCSHOPPOOLIDLIST => new REQ_NpcShopPoolIdList(data),
				Opcode.REQ_NPCSHOPSYNC => new REQ_NpcShopSync(data),
				Opcode.CSC_NPCSHOPPOOL => new REQ_NpcShopPool(data),
				Opcode.CSC_ITEMMOVE => new REQ_ItemMove(data),
				Opcode.CSC_ITEMSWAP => new REQ_ItemSwap(data),
				Opcode.CSC_SKILLTOMOBS => new REQ_SkillToMobs(data),
				Opcode.CSC_SKILLTOUSER => new REQ_SkillToUser(data),
				Opcode.CSC_QUESTOPNEVT => new REQ_QuestOpnEvt(data),
				Opcode.CSC_QUESTNPCACTIN => new REQ_QuestNPCActin(data),
				_ => throw new NotImplementedException($"unimplemented opcode {opcode}"),
			}; ;
		}

		public Queue<Action<Client>>? ReceiveAll(bool isAuthenticated)
		{
			Queue<Action<Client>>? actions = null;

			if (_decryptedInboundPackets.Count > 0)
			{
				var upcomingOpcode = _decryptedInboundPackets.Peek().Item1;
				if (upcomingOpcode != (UInt16)Opcode.CSC_CONNECT2SVR && !isAuthenticated)
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
				Log.Debug($"Processing opcode {opcodeNum} ({packet.GetType().Name})");

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
					//continue;
					throw new Exception("fix the opcode");
				}
			}
			return actions;
		}
	}
}
