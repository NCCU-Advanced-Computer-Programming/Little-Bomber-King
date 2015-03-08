using Bomb.Scene;
using Bomb.Utils;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bomb.Src.Role
{
    class JsonPlayerLoader : IPlayerLoader
    {
        private DummyPlayer dummyPlayer {get;set;}
        public JsonPlayerLoader(Uri uri)
        {
            JsonMapper jsonMapper = new JsonMapper();
            jsonMapper.RegisterImporter<JsonReader, KeyControl>(deserializeKeyControl);
            Stream jsonStream = Application.GetContentStream(uri).Stream;
            string jsonStr = new StreamReader(jsonStream, Encoding.UTF8).ReadToEnd();
            this.dummyPlayer = jsonMapper.ToObject<DummyPlayer>(jsonStr);

        }
        private KeyControl deserializeKeyControl(JsonReader reader)
        {
            return new KeyControl(new JsonKeyControlLoader(reader));
        }
        public string getPlayerName()
        {
            return dummyPlayer.playerName;
        }

        public ImgSrcs getWalkImgSrcs()
        {
            Uri uri = new Uri("Images/Tiles/" + dummyPlayer.walkImgSrcs, UriKind.Relative);
            return new ImgSrcs(new TileImgSrcsLoader(uri, dummyPlayer.walkImgTileWidth, dummyPlayer.walkImgTileHeight, dummyPlayer.walkImgTileMargin));
        }
        public ImgSrcs getFaceImgSrcs()
        {
            Uri uri = new Uri("Images/Tiles/" + dummyPlayer.faceImgSrc, UriKind.Relative);
            return new ImgSrcs(new TileImgSrcsLoader(uri, 96, 96, 0));
        }

        public int getMaxBomb()
        {
            return dummyPlayer.maxBomb;
        }

        public double getSpeed()
        {
            return dummyPlayer.speed;
        }

        public KeyControl getKeyControl()
        {
            return dummyPlayer.ctrl;
            
        }
        public int getPower()
        {
            return dummyPlayer.power;
        }
        public int getMaxHp()
        {
            return dummyPlayer.maxHp;
        }
        public int getAtk()
        {
            return dummyPlayer.atk;
        }

        public DummyPlayer getDummyPlayer()
        {
            return dummyPlayer;
        }
      

        public class DummyPlayer
        {
           
            public string playerName { get;private set; }
            public string walkImgSrcs { get; set; }
            public string faceImgSrc { get; set; }
            public int maxBomb { get; set;}
            public double speed{get;set;} //grid per sec
            public int power { get; set; }
            public int atk { get; set; }
            public int maxHp { get; set; }
            public int walkImgTileWidth { get; set; }
            public int walkImgTileHeight { get; set; }
            public int walkImgTileMargin { get; set; }
            public KeyControl ctrl { get; set; }
        }


       
    }
}
