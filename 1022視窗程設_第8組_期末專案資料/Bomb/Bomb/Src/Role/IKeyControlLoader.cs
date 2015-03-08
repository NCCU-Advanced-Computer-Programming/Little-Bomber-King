using Bomb.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bomb.Src.Role
{
    public interface IKeyControlLoader
    {
         IDictionary<Key, WalkDirection> getWalkKeyBinding();
         Key getLayBombKey();

    }
}
