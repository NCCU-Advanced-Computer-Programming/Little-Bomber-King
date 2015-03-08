using Bomb.Src.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bomb.Src.Game
{
    public class PlayerSelectCompleteEventArgs : EventArgs
    {
       
        public long[] devices;
        public IPlayerLoader[] selectedLoaders;
        public PlayerSelectCompleteEventArgs(long[] devices,IPlayerLoader[] dummyPlayers)
        {
            this.devices = devices;
            this.selectedLoaders = dummyPlayers;
        }
    }
}
