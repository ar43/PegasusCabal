using LibPegasus.Enums;
using LibPegasus.JSON;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;

namespace MasterServer.Sync
{
	public class SyncManager
	{
		ConcurrentDictionary<int, bool[]> _charSync;

		public SyncManager()
		{
			_charSync = new ConcurrentDictionary<int, bool[]>();
		}

		private bool[] AddNew(int id)
		{
			bool[] data = new bool[(Int32)SyncFlags.NUM_FLAGS];
			for(int i = 0; i < data.Length; i++)
				data[i] = true;
			var success =_charSync.TryAdd(id, data);
			if(!success)
			{
				throw new Exception("hmm?");
			}
			return data;
		}

		private bool[] GetStatus(int id)
		{
			if(_charSync.TryGetValue(id, out var result))
			{
				return result;
			}
			else
			{
				return AddNew(id);
			}
		}

		public bool IsSynced(int id)
		{
			var charSync = GetStatus(id);
			foreach(var charSyncEntry in charSync)
			{
				if (charSyncEntry == false)
					return false;
			}

			for (int i = 0; i < charSync.Length; i++)
				charSync[i] = false;
			return true;
		}

		public void SyncSection(int id, SyncFlags flag)
		{
			if (_charSync.TryGetValue(id, out var result))
			{
				result[(Int32)flag] = true;
			}
            else
            {
				throw new Exception("Id not in the dictionary");
            }
        }


	}
}
