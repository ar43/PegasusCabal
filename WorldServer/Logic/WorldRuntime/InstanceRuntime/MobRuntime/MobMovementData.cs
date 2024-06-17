using LibPegasus.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldServer.Logic.CharData;

namespace WorldServer.Logic.WorldRuntime.InstanceRuntime.MobRuntime
{
	internal class MobMovementData
	{
		public static readonly int MAX_SQR_MOVE_DIFF = 256;
		public bool IsMoving { get; private set; }
		public int StartX { get; private set; }
		public int StartY { get; private set; }
		public int EndX { get; private set; }
		public int EndY { get; private set; }
		public Int32 StartTime { get; private set; }

		public int X { get; private set; }
		public int Y { get; private set; }

		public int CellX { get; private set; }
		public int CellY { get; private set; }

		List<Waypoint> _waypoints = new List<Waypoint>();

		public float Distance { get; private set; }
		public float Base { get; private set; }
		public float Sin { get; private set; }
		public float Cos { get; private set; }
		public bool IsDeadReckoning { get; private set; }
		public int LastDeadReckoning;
		public int CurrentWaypoint { get; private set; } // thinking

		public float MoveSpeed { get; set; }

		public MobMovementData(UInt16 startX, UInt16 startY, float moveSpeed)
		{
			IsMoving = false;
			StartX = startX;
			StartY = startY;
			EndX = startX;
			EndY = startY;
			X = startX;
			Y = startY;
			CellX = X / 16;
			CellY = Y / 16;
			StartTime = 0;
			IsDeadReckoning = false;
			Distance = 0;
			Base = 0;
			Sin = 0;
			Cos = 0;
			MoveSpeed = moveSpeed;
			LastDeadReckoning = 0;
			CurrentWaypoint = 0;
		}

		public void UpdateCellPos()
		{
			CellX = X / 16;
			CellY = Y / 16;
		}

		private void StartDeadReckoning()
		{
			var waypoint = _waypoints[0];
			var waypoint2 = _waypoints[1];

			var dx = waypoint2.X - waypoint.X;
			var dy = waypoint2.Y - waypoint.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx >= DistanceCache.TABLE_LENGTH)
			{
				Serilog.Log.Warning("StartDeadReckoning: adx >= DistanceCache.TABLE_LENGTH");
				adx = DistanceCache.TABLE_LENGTH - 1;
			}

			if (ady >= DistanceCache.TABLE_LENGTH)
			{
				Serilog.Log.Warning("StartDeadReckoning: ady >= DistanceCache.TABLE_LENGTH");
				ady = DistanceCache.TABLE_LENGTH - 1;
			}

			Distance = DistanceCache.Get.GetDistance(adx, ady);
			Base = 0.0f;
			CurrentWaypoint = 0;
			Sin = (float)dy / Distance;
			Cos = (float)dx / Distance;

			IsMoving = true;
			IsDeadReckoning = true;
			StartTime = Environment.TickCount;
			LastDeadReckoning = 0;
		}

		private void ChangeDeadReckoning()
		{
			var waypoint = _waypoints[0];
			var waypoint2 = _waypoints[1];

			var dx = waypoint2.X - waypoint.X;
			var dy = waypoint2.Y - waypoint.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx >= DistanceCache.TABLE_LENGTH)
			{
				Serilog.Log.Error("ChangeDeadReckoning: adx >= DistanceCache.TABLE_LENGTH");
				throw new Exception("ChangeDeadReckoning");
			}

			if (ady >= DistanceCache.TABLE_LENGTH)
			{
				Serilog.Log.Error("ChangeDeadReckoning: ady >= DistanceCache.TABLE_LENGTH");
				throw new Exception("ChangeDeadReckoning");
			}

			Base += Distance;
			Distance = DistanceCache.Get.GetDistance(adx, ady);
			CurrentWaypoint = 0;
			Sin = (float)dy / Distance;
			Cos = (float)dx / Distance;

			IsDeadReckoning = true;
			LastDeadReckoning = 0;
		}

		private void AdjustTargetPoint(int validOffset)
		{
			Debug.Assert(validOffset >= 0);
			int iN,
							iDx,
							iDy,
							iAdx,
							iAdy,
							iPos,
							iOffX,
							iOffY;

			iN = _waypoints.Count - 2;
			var pWayPoint = _waypoints[iN];
			var pWayPoint2 = _waypoints[iN+1];

			iDx = pWayPoint2.X - pWayPoint.X;
			iDy = pWayPoint2.Y - pWayPoint.Y;
			iAdx = (iDx >= 0) ? iDx : -iDx;
			iAdy = (iDy >= 0) ? iDy : -iDy;

			if (iAdx >= iAdy)
			{
				iPos = iAdx - validOffset;
				if (iPos < 0)
				{
					_waypoints.RemoveAt(iN+1);
					Debug.Assert(_waypoints.Count > 0);

					EndX = pWayPoint.X;
					EndY = pWayPoint.Y;

					if (StartX == EndX &&
						StartY == EndY)
					{
						Serilog.Log.Error(("iPos < 0"));
					}
					return;
				}

				iOffX = (iPos < 0) ? -iPos : iPos;
				
				iOffY = (iAdy * iOffX + (iAdx >> 1)) / iAdx;

				if (iDx < 0)
					iOffX = -iOffX;
				if (iDy < 0)
					iOffY = -iOffY;

				pWayPoint2.X = pWayPoint.X + iOffX;
				pWayPoint2.Y = pWayPoint.Y + iOffY;

				EndX = pWayPoint2.X;
				EndY = pWayPoint2.Y;
			}
			else
			{
				iPos = iAdy - validOffset;
				if (iPos < 0)
				{
					_waypoints.RemoveAt(iN + 1);
					Debug.Assert(_waypoints.Count > 0);

					EndX = pWayPoint.X;
					EndY = pWayPoint.Y;
					return;
				}

				iOffY = (iPos < 0) ? -iPos : iPos;
				
				iOffX = (iAdx * iOffY + (iAdy >> 1)) / iAdy;

				if (iDx < 0)
					iOffX = -iOffX;
				if (iDy < 0)
					iOffY = -iOffY;

				pWayPoint2.X = pWayPoint.X + iOffX;
				pWayPoint2.Y = pWayPoint.Y + iOffY;

				EndX = pWayPoint2.X;
				EndY = pWayPoint2.X;
			}
		}

		public bool Begin(UInt16 startX, UInt16 startY, UInt16 endX, UInt16 endY, UInt16 pntX, UInt16 pntY)
		{
			var dxBegin = StartX - startX;
			var dyBegin = StartY - startY;
			var dxCurrent = X - startX;
			var dyCurrent = Y - startY;

			if (dxBegin * dxBegin > MAX_SQR_MOVE_DIFF ||
			   dyBegin * dyBegin > MAX_SQR_MOVE_DIFF ||
			   dxCurrent * dxCurrent > MAX_SQR_MOVE_DIFF ||
			   dyCurrent * dxCurrent > MAX_SQR_MOVE_DIFF)
			{
				Serilog.Log.Warning("Movement begin: MAX_SQR_MOVE_DIFF problem");
				return false;
			}

			if (startX == endX && startY == endY)
			{
				Serilog.Log.Warning("startX == endX && startY == endY");
				return false;
			}

			StartX -= dxBegin;
			StartY -= dyBegin;
			X -= dxCurrent;
			Y -= dyCurrent;

			var cellDx = pntX / 16 - CellX;
			var cellDy = pntY / 16 - CellY;

			if (cellDx > 1 || cellDy > 1 || cellDx < -1 || cellDy < -1)
			{
				Serilog.Log.Warning($"Mob Movement begin: TileDiff problem {cellDx} {cellDy}");
				return false;
			}

			_waypoints.Add(new Waypoint(startX, startY));
			_waypoints.Add(new Waypoint(pntX, pntY));

			StartDeadReckoning();

			EndX = endX;
			EndY = endY;


			return true;
		}

		public void DeadReckoning()
		{
			var tickCount = Environment.TickCount;

			int dx, dy, adx, ady;
			var elapsedTime = tickCount - StartTime;

			var len = MoveSpeed * (float)(elapsedTime / 1000.0f);

			var lenRest = len - Base;

			if (lenRest < 0) //what does this do
				return;

			var waypoint = _waypoints[CurrentWaypoint];
			var waypoint2 = _waypoints[CurrentWaypoint + 1];

			while (lenRest >= Distance)
			{
				Base += Distance;

				if (CurrentWaypoint + 2 >= _waypoints.Count)
				{
					IsDeadReckoning = false;
					X = waypoint2.X;
					Y = waypoint2.Y;
					return;
				}

				CurrentWaypoint++;
				waypoint = _waypoints[CurrentWaypoint];
				waypoint2 = _waypoints[CurrentWaypoint + 1];

				dx = waypoint2.X - waypoint.X;
				dy = waypoint2.Y - waypoint.Y;

				adx = Math.Abs(dx);
				ady = Math.Abs(dy);

				if (adx >= DistanceCache.TABLE_LENGTH)
				{
					Serilog.Log.Warning("StartDeadReckoning: adx >= DistanceCache.TABLE_LENGTH");
					adx = DistanceCache.TABLE_LENGTH - 1;
				}

				if (ady >= DistanceCache.TABLE_LENGTH)
				{
					Serilog.Log.Warning("StartDeadReckoning: ady >= DistanceCache.TABLE_LENGTH");
					ady = DistanceCache.TABLE_LENGTH - 1;
				}

				lenRest -= Distance;
				Distance = DistanceCache.Get.GetDistance(adx, ady);
				Sin = (float)dy / Distance;
				Cos = (float)dx / Distance;
			}

			dx = waypoint2.X - waypoint.X;
			dy = waypoint2.Y - waypoint.Y;

			adx = Math.Abs(dx);
			ady = Math.Abs(dy);

			if (adx >= ady)
			{
				var pos = (int)(lenRest * Cos);

				var offX = pos < 0 ? -pos : pos;
				var offY = (ady * offX + (adx >> 1)) / adx;

				if (dx < 0)
					offX = -offX;
				if (dy < 0)
					offY = -offY;

				X = waypoint.X + offX;
				Y = waypoint.Y + offY;
			}
			else
			{
				var pos = (int)(lenRest * Sin);

				var offY = pos < 0 ? -pos : pos;
				var offX = (adx * offY + (ady >> 1)) / ady;

				if (dx < 0)
					offX = -offX;
				if (dy < 0)
					offY = -offY;

				X = waypoint.X + offX;
				Y = waypoint.Y + offY;
			}
		}

		public bool Change(UInt16 startX, UInt16 startY, UInt16 endX, UInt16 endY, UInt16 pntX, UInt16 pntY)
		{
			var waypoint = _waypoints[0];
			var waypoint2 = _waypoints[1];

			var dx = waypoint2.X - waypoint.X;
			var dy = waypoint2.Y - waypoint.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			int ox, oy, dxBegin, dyBegin;

			if (adx > ady)
			{
				if (adx == 0)
					adx = 1;

				ox = (dx > 0) ? (startX - waypoint.X) : (waypoint.X - startX);
				oy = (ady * ox + (adx >> 1)) / adx;

				if (dy < 0)
					oy = -oy;

				dxBegin = waypoint.X + ox * dx / adx - startX;
				dyBegin = waypoint.Y + oy - startY;
			}
			else
			{
				oy = (dy > 0) ? (startY - waypoint.Y) : (waypoint.Y - startY);
				ox = (adx * oy + (ady >> 1)) / ady;

				if (dx < 0)
					ox = -ox;

				dxBegin = waypoint.X + ox - startX;
				dyBegin = waypoint.Y + oy * dy / ady - startY;
			}

			if (dyBegin * dyBegin > MAX_SQR_MOVE_DIFF || dxBegin * dxBegin > MAX_SQR_MOVE_DIFF)
			{
				Serilog.Log.Warning($"Movement change: MAX_SQR_MOVE_DIFF problem {dxBegin} {dyBegin}");
				return false;
			}

			var newCellX = pntX / 16;
			var newCellY = pntY / 16;

			var cellDx = newCellX - CellX;
			var cellDy = newCellY - CellY;

			if (cellDx < -1 || cellDy < -1 || cellDx > 1 || cellDy > 1)
			{
				return false;
			}

			_waypoints.Clear();
			_waypoints.Add(new Waypoint(startX, startY));
			_waypoints.Add(new Waypoint(pntX, pntY));

			StartDeadReckoning();

			StartX = startX;
			StartY = startY;
			X = startX;
			Y = startY;
			EndX = endX;
			EndY = endY;

			return true;
		}

		public void SetPosition(int x, int y)
		{
			StartX = x;
			StartY = y;
			X = x;
			Y = y;
			EndX = x;
			EndY = y;
		}

		public void ForceEndMovement()
		{
			StartX = X;
			StartY = Y;
			EndX = X;
			EndY = Y;

			StopDeadReckoning();
		}

		public bool End(UInt16 x, UInt16 y)
		{
			var waypoint = _waypoints[0];
			var waypoint2 = _waypoints[1];

			var dx = waypoint2.X - waypoint.X;
			var dy = waypoint2.Y - waypoint.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			int ox, oy, dxBegin, dyBegin;

			if (adx > ady)
			{
				if (adx == 0)
					adx = 1;

				ox = (dx > 0) ? (x - waypoint.X) : (waypoint.X - x);
				oy = (ady * ox + (adx >> 1)) / adx;

				if (dy < 0)
					oy = -oy;

				dxBegin = waypoint.X + ox * dx / adx - x;
				dyBegin = waypoint.Y + oy - y;
			}
			else
			{
				oy = (dy > 0) ? (y - waypoint.Y) : (waypoint.Y - y);
				ox = (adx * oy + (ady >> 1)) / ady;

				if (dx < 0)
					ox = -ox;

				dxBegin = waypoint.X + ox - x;
				dyBegin = waypoint.Y + oy * dy / ady - y;
			}

			if (dyBegin * dyBegin > MAX_SQR_MOVE_DIFF || dxBegin * dxBegin > MAX_SQR_MOVE_DIFF)
			{
				Serilog.Log.Warning($"Movement end: MAX_SQR_MOVE_DIFF problem {dxBegin} {dyBegin}");
				return false;
			}

			StartX = x;
			StartY = y;
			X = x;
			Y = y;
			EndX = x;
			EndY = y;

			StopDeadReckoning();
			return true;
		}

		public bool CellMove(UInt16 x, UInt16 y, bool isGm)
		{
			if (x >= MAX_SQR_MOVE_DIFF || y >= MAX_SQR_MOVE_DIFF)
			{
				return false;
			}

			var waypoint = _waypoints[0];
			var waypoint2 = _waypoints[1];

			var dx = waypoint2.X - waypoint.X;
			var dy = waypoint2.Y - waypoint.Y;

			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			int ox, oy, dxBegin, dyBegin;

			if (adx > ady)
			{
				if (adx == 0)
					adx = 1;

				ox = (dx > 0) ? (x - waypoint.X) : (waypoint.X - x);
				oy = (ady * ox + (adx >> 1)) / adx;

				if (dy < 0)
					oy = -oy;

				dxBegin = waypoint.X + ox * dx / adx - x;
				dyBegin = waypoint.Y + oy - y;
			}
			else
			{
				oy = (dy > 0) ? (y - waypoint.Y) : (waypoint.Y - y);
				ox = (adx * oy + (ady >> 1)) / ady;

				if (dx < 0)
					ox = -ox;

				dxBegin = waypoint.X + ox - x;
				dyBegin = waypoint.Y + oy * dy / ady - y;
			}

			if (dyBegin * dyBegin > MAX_SQR_MOVE_DIFF || dxBegin * dxBegin > MAX_SQR_MOVE_DIFF)
			{
				Serilog.Log.Warning($"VerifyTileMove: MAX_SQR_MOVE_DIFF problem {dxBegin} {dyBegin}");
				return false;
			}

			var newCellX = x / 16;
			var newCellY = y / 16;

			var cellDx = newCellX - CellX;
			var cellDy = newCellY - CellY;

			if (cellDx == 0 && cellDy == 0)
			{
				return false;
			}

			if (cellDx < -1 || cellDy < -1 || cellDx > 1 || cellDy > 1)
			{
				return false;
			}

			X = x;
			Y = y;

			return true;
		}

		public bool SwitchWaypoint(UInt16 startX, UInt16 startY, UInt16 pntX, UInt16 pntY)
		{
			_waypoints.Clear();
			_waypoints.Add(new Waypoint(startX, startY));
			_waypoints.Add(new Waypoint(pntX, pntY));

			var dx = pntX - startX;
			var dy = pntY - startY;
			var adx = Math.Abs(dx);
			var ady = Math.Abs(dy);

			if (adx >= DistanceCache.TABLE_LENGTH || ady >= DistanceCache.TABLE_LENGTH)
			{
				return false;
			}

			ChangeDeadReckoning();

			X = startX;
			Y = startY;

			return true;
		}

		private void StopDeadReckoning()
		{
			IsMoving = false;
			IsDeadReckoning = false;
			_waypoints.Clear();
		}
	}
}
