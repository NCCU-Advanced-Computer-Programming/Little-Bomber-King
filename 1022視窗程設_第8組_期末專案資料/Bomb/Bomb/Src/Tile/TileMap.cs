using Bomb.Src.Scene;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;

namespace Bomb.Tile
{
    class TileMap
    {
        public string backgroundcolor { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string name { get; set; }
        public int tilewidth { get; set; }
        public int tileheight { get; set; }
        public TileLayer[] layers { get; set; }
        public TileSet[] tilesets { get; set; }
        public Dictionary<string, object> propertites { get; set; }
        public Int32Point[] startPos { get; set; }
        public class JsonTileMapParser
        {
            private Uri uri { get; set; }
            public JsonTileMapParser(Uri uri)
            {
                this.uri = uri;
            }
            public TileMap parse()
            {
                JsonMapper jsonMapper = new JsonMapper();
                Stream jsonStream = Application.GetContentStream(uri).Stream;
                string jsonStr = new StreamReader(jsonStream, Encoding.UTF8).ReadToEnd();
                return jsonMapper.ToObject<TileMap>(jsonStr);
            }


        }



    }
}
