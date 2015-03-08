using Bomb.Src;
using Bomb.Src.Scene;
using Bomb.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bomb.Scene
{
    class RoleLayer : Canvas
    {
        public IList<Player> players { get;private set; }
        public Int32Point[] startPos { get; set; }
        public RoleLayer(Int32Point[] startPos) : base()
        {
            this.startPos = startPos;
            this.players = new List<Player>();
        }
        public void addPlayer(Player player)
        {
            
            Int32Point pos = startPos[players.Count];
            player.gridX = pos.x;
            player.gridY = pos.y;
            Children.Add(player);

            Utility.MW.stkPlayerInfoPanel.Children.Add(player.infoPanel);
            players.Add(player);
        }
        public IList<Player> playersOnGrid(int x, int y)
        {
            return playerWithinGrids(x,y,1,1);
            
        }
        public IList<Player> playersOnActualGrid(int x, int y)
        {
            return playerWithinActualGrids(x, y, 1, 1);

        }
        public IList<Player> playerWithinGrids(int x, int y, int width, int height)
        {
            IList<Player> retVal = new List<Player>();
            foreach (Player player in players)
            {
                if (player.gridX >= x && player.gridX < x+width && player.gridY >= y &&  player.gridY < y+height)
                {
                    retVal.Add(player);
                }
            }
            return retVal;
        }
        public IList<Player> playerWithinActualGrids(int x, int y, int width, int height)
        {
            IList<Player> retVal = new List<Player>();
            foreach (Player player in players)
            {
                if (player.actualGridX >= x && player.actualGridX < x + width && player.actualGridY >= y && player.actualGridY < y + height)
                {
                    retVal.Add(player);
                }
            }
            return retVal;
        }
    }
}
