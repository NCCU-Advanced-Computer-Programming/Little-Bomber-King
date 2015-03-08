using Bomb.Scene;
using Bomb.Src.Role;
using Bomb.Src.Utils;
using Bomb.Tile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bomb.Src.Game
{
    class GameManager
    {
        public static GameManager currGameMgr { get; set; }
        public bool isGaming { get;private set; }
        public GameScene scene { get;private set; }
        public List<IPlayerLoader> playerLoaders { get; set; }
        private PlayerSelector playerSelector { get; set; }
        
        public IList<Player> players 
        {
            get
            {
                return scene.roleLayer.players;
            }
        }
        public GameManager()
        {
            currGameMgr = this;
            this.isGaming = false;
            initPlayerLoaders();
            this.playerSelector = new PlayerSelector(playerLoaders);
            playerSelector.onPlayerSelectComplete += playerSelector_onPlayerSelectComplete;

            
        }
        public void showMainMenu()
        {
            Utility.MW.btnSelectRole.Click += btnSelectRole_Click;
            Utility.MW.btnExit.Click += btnExit_Click;
            Utility.MW.grdMainMenu.Visibility = System.Windows.Visibility.Visible;
        }

        void btnExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void btnSelectRole_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Utility.MW.grdMainMenu.Visibility = System.Windows.Visibility.Collapsed;
            Utility.MW.mdaSe.Stop();
            Utility.MW.mdaSe.Source = new Uri("Musics/magic.mp3", UriKind.Relative);
            Utility.MW.mdaSe.Play();
            selectPlayer();
        }
        private void playerSelector_onPlayerSelectComplete(object sender, PlayerSelectCompleteEventArgs e)
        {
            Utility.MW.grdSelectPlayer.Visibility = System.Windows.Visibility.Collapsed;
            
            loadScene();
            loadPlayer(e);
            startGame();
        }
        private void loadScene()
        {
            TileMap.JsonTileMapParser mapParser = new TileMap.JsonTileMapParser(new Uri("/Scene/Scene1.json", UriKind.Relative));
            TileMap tileMap = mapParser.parse();
            TileSceneLoader sceneLoader = new TileSceneLoader(tileMap);
            this.scene = new GameScene(sceneLoader);
            scene.renderOn(Utility.MW.cvsScene);
            Utility.MW.grdGame.Visibility = System.Windows.Visibility.Visible;

            Utility.MW.mdaBgm.Stop();
            Utility.MW.mdaBgm.Source = new Uri("Musics/bgm1.mp3", UriKind.Relative);

            Utility.MW.mdaBgm.Play();
        }
        private void initPlayerLoaders()
        {
            playerLoaders = new List<IPlayerLoader>();
            DirectoryInfo roleDir = new DirectoryInfo("Role");
            FileInfo[] jsonFiles = roleDir.GetFiles("*.json");
            foreach (FileInfo file in jsonFiles)
            {
                string uriStr = roleDir.Name+"/"+file.FullName.Substring(roleDir.FullName.Length+1);// +1 is for "\\"
                playerLoaders.Add(new JsonPlayerLoader(new Uri(uriStr, UriKind.Relative)));
            }


        }
        public void selectPlayer()
        {
            
            Utility.MW.grdSelectPlayer.Visibility = System.Windows.Visibility.Visible;
        }
        private void loadPlayer(PlayerSelectCompleteEventArgs e)
        {
            for (int i = 0; i < e.selectedLoaders.Length; i++)
            {
                Player player = new Player(e.selectedLoaders[i], e.devices[i]);
                scene.roleLayer.addPlayer(player);
            }

        }
        public void startGame()
        {
            isGaming = true;
        }
    }
}
