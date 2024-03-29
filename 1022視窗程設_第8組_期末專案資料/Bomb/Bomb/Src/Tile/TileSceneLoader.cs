﻿using Bomb.Scene;
using Bomb.Src.Scene;
using Bomb.Src.Tile;
using Bomb.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Bomb.Tile
{
    class TileSceneLoader : ISceneLoader
    {
        private TileMap tileMap { get; set; }
        private ImgSrcs tileImgSrcs { get; set; }
        private FloorLayer floorLayer { get;  set; }
        private ObstacleLayer obstacleLayer { get;  set; }
        private RoleLayer roleLayer { get;  set; }
        private ShelterLayer shelterLayer { get;  set; }
        private BoxLayer boxLayer { get;  set; }
        private BombLayer bombLayer { get; set; }
        private Int32Point[] startPos { get; set; }
        private Dictionary<int, TileProperty> tileProp { get; set; }
        public TileSceneLoader(TileMap tileMap)
        {
            this.tileMap = tileMap;
            this.tileProp = new Dictionary<int, TileProperty>();
            foreach (TileSet tileSet in tileMap.tilesets)
            {
                if (tileSet.tileproperties != null)
                {
                    foreach (KeyValuePair<string, TileProperty> entry in tileSet.tileproperties)
                    {
                        tileProp[int.Parse(entry.Key)] = entry.Value;
                    }
                }
            }

        }
        public void loadFloorLayer(TileLayer tileLayer)
        {
            floorLayer = new FloorLayer();
            for (int i = 0; i < tileMap.height; i++)
            {
                floorLayer.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(tileMap.tileheight) });
            }
            for (int i = 0; i < tileMap.width; i++)
            {
                floorLayer.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(tileMap.tilewidth) });
            }
       
            floorLayer.Width = tileMap.width * tileMap.tilewidth;
            floorLayer.Height = tileMap.height * tileMap.tileheight;
            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                
                if (tileLayer.data[i] != 0)
                {
                    int x = i % tileMap.width;
                    int y = i / tileMap.height;

                    Image img = new Image();
                    img.Source = tileImgSrcs[tileLayer.data[i]-1];
                    img.SetValue(Grid.ColumnProperty, x);
                    img.SetValue(Grid.RowProperty, y);

                    floorLayer.Children.Add(img);
                }
            }
        }
        public void loadObstacleLayer(TileLayer tileLayer)
        {
            obstacleLayer = new ObstacleLayer(tileMap.height,tileMap.width,tileMap.tileheight,tileMap.tilewidth);
           
            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                if (tileLayer.data[i] != 0)
                {
                    int x = i % tileMap.width;
                    int y = i / tileMap.height;

                    Obstacle img = new Obstacle();
                    img.Source = tileImgSrcs[tileLayer.data[i] - 1];
                    obstacleLayer.addObstacle(img,y,x);
                }

            }
        }
        public void loadShelterLayer(TileLayer tileLayer)
        {
            shelterLayer = new ShelterLayer(tileMap.height, tileMap.width, tileMap.tileheight, tileMap.tilewidth);
            

            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                if (tileLayer.data[i] != 0)
                {
                    int x = i % tileMap.width;
                    int y = i / tileMap.height;

                    bool canDestroy = tileProp.ContainsKey(tileLayer.data[i]-1) && tileProp[tileLayer.data[i]-1].canDestroy != null;
                    bool canKick = tileProp.ContainsKey(tileLayer.data[i]-1) && tileProp[tileLayer.data[i]-1].canKick != null;
                    Shelter shelter = new Shelter(canDestroy,canKick);
                    shelter.Source = tileImgSrcs[tileLayer.data[i] - 1];
                    shelterLayer.addShelter(shelter,y,x);
                }

            }
        }
        public void loadBoxLayer(TileLayer tileLayer)
        {
            boxLayer = new BoxLayer(tileMap.height, tileMap.width, tileMap.tileheight, tileMap.tilewidth);
          
            for (int i = 0; i < tileLayer.data.Length; i++)
            {
                if (tileLayer.data[i] != 0)
                {
                    int x = i % tileMap.width;
                    int y = i / tileMap.height;

                    bool canDestroy = tileProp.ContainsKey(tileLayer.data[i]-1) && tileProp[tileLayer.data[i]-1].canDestroy!=null;
                    bool canKick = tileProp.ContainsKey(tileLayer.data[i]-1) &&  tileProp[tileLayer.data[i]-1].canKick != null;
                    

                    Box box = new Box(canDestroy,canKick);
                    box.Source = tileImgSrcs[tileLayer.data[i] - 1];
                    boxLayer.addBox(box, y, x);
                }

            }
        }
        public void loadScene()
        {
            tileImgSrcs = new ImgSrcs();
            foreach (TileSet tileSet in tileMap.tilesets)
            {
                TileSet.BitmapListLoader loader = new TileSet.BitmapListLoader(tileSet);
                tileImgSrcs.AddRange(loader.getBitmapImages());
            }


            foreach (TileLayer tileLayer in tileMap.layers)
            {
                switch (tileLayer.name)
                {
                    case "Floor":
                        loadFloorLayer(tileLayer);
                        break;
                    case "Obstacle":
                        loadObstacleLayer(tileLayer);
                        break;
                    case "Shelter":
                        loadShelterLayer(tileLayer);
                        break;
                    case "Box":
                        loadBoxLayer(tileLayer);
                        break;
                    default:
                        break;
                }
            }
        }

        public FloorLayer getFloorLayer()
        {
            return floorLayer;
        }

        public ObstacleLayer getObstacleLayer()
        {
            return obstacleLayer;
        }

        public ShelterLayer getShelterLayer()
        {
            return shelterLayer;
        }

        public BoxLayer getBoxLayer()
        {
            return boxLayer;
        }


        public System.Windows.Media.Color getBackgroundColor()
        {
            return (Color)ColorConverter.ConvertFromString(tileMap.backgroundcolor); ;
        }

        public Int32Point[] getStartPoint()
        {
            return tileMap.startPos;
        }





        public RoleLayer getRoleLayer()
        {
            roleLayer = new RoleLayer(tileMap.startPos);
            return roleLayer;
        }


        public int getTileWidth()
        {
            return tileMap.tilewidth;
        }

        public int getTileHeight()
        {
            return tileMap.tileheight;
        }


        public int getWidth()
        {
            return tileMap.width;
        }

        public int getHeight()
        {
            return tileMap.height;
        }


        public BombLayer getBombLayer()
        {
            bombLayer = new BombLayer(tileMap.height,tileMap.width);
            return bombLayer;
        }
    }
}
