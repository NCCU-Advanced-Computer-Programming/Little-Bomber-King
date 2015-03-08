using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bomb.Src.Utils
{
    class Utility
    {
        public static MainWindow MW
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
        }
    }
}
