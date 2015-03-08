using Bomb.Src.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bomb.Src.Role
{
    class JsonKeyControlLoader : IKeyControlLoader
    {
        
        private DummyKeyControl dummyKeyControl { get;set; }
        public JsonKeyControlLoader(JsonReader reader)
        {
            dummyKeyControl = new DummyKeyControl();
            JsonMapper jsonMapper = new JsonMapper();
            while (reader.Read())
            {
                if (reader.Token == JsonToken.ObjectEnd)
                {
                    break;
                }
                if (reader.Token == JsonToken.PropertyName)
                {
                    switch (reader.Value as string)
                    {
                        case "walkKeyBinding":
                            dummyKeyControl.walkKeyBinding = jsonMapper.ToObject<Dictionary<string, string>>(reader);
                            break;
                        case "layBombKey":
                            dummyKeyControl.layBombKey = jsonMapper.ToObject<string>(reader);
                            break;
                    }
                }
            }
            
            
        }


        public class DummyKeyControl
        {
            public Dictionary<string, string> walkKeyBinding { get; set; }
            public string layBombKey { get; set; }
        }

        public IDictionary<System.Windows.Input.Key, Utils.WalkDirection> getWalkKeyBinding()
        {
            Dictionary<Key,WalkDirection> retVal = new Dictionary<Key, WalkDirection>();
            foreach (KeyValuePair<string, string> entry in dummyKeyControl.walkKeyBinding)
            {
                retVal[(Key)Enum.Parse(typeof(Key), entry.Key)] = (WalkDirection)Enum.Parse(typeof(WalkDirection), entry.Value);
            }
            return retVal;

        }

        public System.Windows.Input.Key getLayBombKey()
        {
            return (Key)Enum.Parse(typeof(Key), dummyKeyControl.layBombKey);
        }
    }
}
