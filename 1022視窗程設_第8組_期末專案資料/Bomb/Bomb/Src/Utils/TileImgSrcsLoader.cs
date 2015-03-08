using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bomb.Utils
{
    class TileImgSrcsLoader : IImgSrcsLoader
    {
        private BitmapImage img { get; set; }
        private int tileWidth { get; set; }
        private int tileHeight { get; set; }
       
        private int margin { get; set; }

        public TileImgSrcsLoader(Uri uri, int tileWidth, int tileHeight, int margin)
        {
            this.img = new BitmapImage();
            img.BeginInit();
            img.UriSource = uri;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();

            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.margin = margin;

        }

        public ICollection<ImageSource> getImageSources()
        {
           
            int row = (int)(img.PixelHeight-2*margin)/tileHeight;
            int col = (int)(img.PixelWidth - 2 * margin) / tileWidth;
            int size = row * col;
            ImageSource[] retVal = new ImageSource[size];
            

            int count = 0;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    retVal[count++] = new CroppedBitmap(img, new Int32Rect(margin + j * tileWidth, margin + i * tileHeight, tileWidth, tileHeight));
                }
            }
            return retVal;
        }

        
    }
}
