using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Bomb.Src.Scene
{
    class Shelter : Image
    {
        public bool canDestroy { get; set; }
        public bool canKick { get; set; }
        public Shelter(bool canDestroy,bool canKick)
            : base()
        {
            this.canDestroy = canDestroy;
            this.canKick = canKick;
        }
    }
}
