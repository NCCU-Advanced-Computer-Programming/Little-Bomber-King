using Bomb.Src.Tile;
using Bomb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bomb.Tile
{
    class TileSet
    {
        public int firstgid { get; set; }
        public string image { get; set; }
        public int imageheight { get; set; }
        public int imagewidth { get; set; }
        public int margin { get; set; }
        public string name { get; set; }
        public Dictionary<string, object> properties { get; set; }
        public int spacing { get; set; }
        public int tileheight { get; set; }
        public int tilewidth { get; set; }
        public Dictionary<string, TileProperty> tileproperties { get; set; }

        public class BitmapListLoader : TileImgSrcsLoader
        {
            public BitmapListLoader(TileSet tileSet)
                : base(new Uri("Images/Tiles/" + tileSet.image, UriKind.Relative), tileSet.tilewidth, tileSet.tileheight, tileSet.margin)
            {
 
            }
            public ICollection<ImageSource> getBitmapImages()
            {
                return base.getImageSources();

            }
        }

    }
}
