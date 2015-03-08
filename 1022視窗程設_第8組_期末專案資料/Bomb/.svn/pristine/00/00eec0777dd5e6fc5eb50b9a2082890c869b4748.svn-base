using Bomb.Src;
using Bomb.Src.Role;
using Bomb.Src.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Bomb.Scene
{
    partial class GameScene
    {
        public Color backgroundColor { get;protected set; }
        public FloorLayer floorLayer {  get; protected set; }
        public ObstacleLayer obstacleLayer { get; protected set; }
        public BoxLayer boxLayer { get; protected set; }
        public RoleLayer roleLayer { get; protected set; }
        public BombLayer bombLayer { get; protected set; }
        public ShelterLayer shelterLayer { get; protected set; }
        
        public int tileWidth { get; private set; }
        public int tileHeight { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        public GameScene(ISceneLoader sceneLoader)
        {
            sceneLoader.loadScene();
            this.floorLayer = sceneLoader.getFloorLayer();
            this.obstacleLayer = sceneLoader.getObstacleLayer();
            this.roleLayer = sceneLoader.getRoleLayer();
            this.shelterLayer = sceneLoader.getShelterLayer();
            this.boxLayer = sceneLoader.getBoxLayer();
            this.bombLayer = sceneLoader.getBombLayer();
            this.backgroundColor = sceneLoader.getBackgroundColor();
            this.tileWidth = sceneLoader.getTileWidth();
            this.tileHeight = sceneLoader.getTileHeight() ;
            this.width = sceneLoader.getWidth();
            this.height = sceneLoader.getHeight();
        }
        public void renderOn(Panel panel)
        {
           panel.Background = new SolidColorBrush(backgroundColor);
           panel.Children.Add(floorLayer);
           panel.Children.Add(obstacleLayer);
           panel.Children.Add(boxLayer);
           panel.Children.Add(roleLayer);
           panel.Children.Add(bombLayer);
           panel.Children.Add(shelterLayer);
           
        }
        
        


        

    }
}
