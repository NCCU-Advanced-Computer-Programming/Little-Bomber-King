﻿using Bomb.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bomb.Src.Role
{
    public class KeyControl
    {
        public IDictionary<Key, WalkDirection> walkKeyBinding { get; set; }
        public Key layBombKey { get; set; }
        public IDictionary<Key, bool> keyState { get; set; }
        
        public KeyControl(IKeyControlLoader keyControlLoader)
        {
            walkKeyBinding = keyControlLoader.getWalkKeyBinding();
            layBombKey = keyControlLoader.getLayBombKey();
            keyState = new Dictionary<Key, bool>();

            foreach (KeyValuePair<Key, WalkDirection> entry in walkKeyBinding)
            {
                keyState[entry.Key] = false;
            }
            keyState[layBombKey] = false;
            

        }

    }
}
