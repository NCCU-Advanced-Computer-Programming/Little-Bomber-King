using Bomb.Src.Utils;
using Bomb.Src.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bomb.Src.Scene
{
    class BombLayer : Canvas
    {
        public IList<WBomb>[,] bombs { get;private set; }
        
        public BombLayer(int rows,int column) : base()
        {
            this.Opacity = 0.7;
            this.bombs = new List<WBomb>[rows,column];
            for(int i=0;i<bombs.GetLength(0);i++)
            {
                for (int j = 0; j < bombs.GetLength(1); j++)
                {
                    bombs[i, j] = new List<WBomb>();
                }
            }
        }
        public void addBomb(WBomb bomb)
        {
            
            bombs[bomb.laidGridX, bomb.laidGridY].Add(bomb);
            Children.Add(bomb);
            foreach (WalkDirection dir in Enum.GetValues(typeof(WalkDirection)))
            {
                Children.Add(bomb.detonateRects[dir]);
            }
            
        }
        public void removeBomb(WBomb bomb)
        {
            bombs[bomb.laidGridX, bomb.laidGridY].Remove(bomb);
            Children.Remove(bomb);
            foreach (WalkDirection dir in Enum.GetValues(typeof(WalkDirection)))
            {
                Children.Remove(bomb.detonateRects[dir]);
            }
        }
        
    }
}
