using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bomb.Src.Utils
{
    public class RawKeyInputEventArg : EventArgs
    {
        public long DeviceHnd;
        public Key Key;
        public RawKeyInputEventArg(RawStuff.InputDevice.KeyControlEventArgs rawE)
        {
            this.DeviceHnd = rawE.Keyboard.deviceHandle.ToInt64();
            try
            {
                this.Key = (Key)Enum.Parse(typeof(Key), rawE.Keyboard.vKey);
            }
            catch (Exception e)
            {
 
            }
            

            
        }
    }
}
