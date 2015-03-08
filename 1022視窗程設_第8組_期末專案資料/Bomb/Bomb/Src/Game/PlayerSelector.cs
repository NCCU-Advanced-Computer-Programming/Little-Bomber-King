using Bomb.Scene;
using Bomb.Src.Role;
using Bomb.Src.Utils;
using Bomb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
namespace Bomb.Src.Game
{
    class PlayerSelector
    {
        private object dis = null;
        private static int MAX_PLAYERS = 4;
        private IList<long> devices { get; set; }
        private IList<int> selectedDummyPlayerIdx;
        private IList<int> lockSelectedDummyPlayerIdx;
        private IList<JsonPlayerLoader> playerLoaders { get; set; }
        private IList<ImgSrcs> faces { get; set; }
        public event EventHandler<PlayerSelectCompleteEventArgs> onPlayerSelectComplete;
        public PlayerSelector(IList<IPlayerLoader> loaders)
        {
           
          this.selectedDummyPlayerIdx = new List<int>();
            this.faces = new List<ImgSrcs>();
            this.lockSelectedDummyPlayerIdx = new List<int>();

            for (int i = 0; i < loaders.Count; i++)
            {
                lockSelectedDummyPlayerIdx.Add(0);
            }
            this.playerLoaders = new List<JsonPlayerLoader>();
            foreach (JsonPlayerLoader playerLoader in loaders)
            {
                playerLoaders.Add(playerLoader as JsonPlayerLoader);
                JsonPlayerLoader.DummyPlayer dummyPlayer = playerLoader.getDummyPlayer();
                Uri uri = new Uri("Images/Tiles/" + dummyPlayer.faceImgSrc, UriKind.Relative);
                ImgSrcs face = new ImgSrcs(new TileImgSrcsLoader(uri, 96, 96, 0));
                faces.Add(face);

            }
            Utility.MW.onRawKeyUp += MW_PreviewKeyUp;
            this.devices = new List<long>();
            Utility.MW.btnStartGame.Click += btnStartGame_Click;

        }

        void btnStartGame_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            completeSelectPlayer();
        }
       
        void MW_PreviewKeyUp(object sender, RawKeyInputEventArg e)
        {
            Utility.MW.mdaSe.Stop();
            Utility.MW.mdaSe.Source = new Uri("Musics/ok.mp3",UriKind.Relative);
            Utility.MW.mdaSe.Play();
            if (devices.Contains(e.DeviceHnd))
            {
                
                
                int slotIdx = devices.IndexOf(e.DeviceHnd);
                if (lockSelectedDummyPlayerIdx[slotIdx] != -1)
                {
                    switch (e.Key)
                    {
                        case Key.Right:
                            selectedDummyPlayerIdx[slotIdx] = (selectedDummyPlayerIdx[slotIdx] + 1) % playerLoaders.Count;
                            if (slotIdx == 0)
                            {
                                Utility.MW.p1_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p1_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p1_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p1_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp/100*90;
                                Utility.MW.p1_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p1_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p1_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p1_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 1)
                            {
                                Utility.MW.p2_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p2_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p2_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p2_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p2_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p2_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p2_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p2_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 2)
                            {
                                Utility.MW.p3_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p3_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p3_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p3_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p3_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p3_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p3_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p3_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 3)
                            {
                                Utility.MW.p4_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p4_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p4_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p4_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p4_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p4_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p4_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p4_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            break;
                        case Key.Left:
                            if (selectedDummyPlayerIdx[slotIdx] == 0)
                            {
                                selectedDummyPlayerIdx[slotIdx] = playerLoaders.Count-1; 
                            }
                            else
                            {
                                selectedDummyPlayerIdx[slotIdx] = (selectedDummyPlayerIdx[slotIdx] - 1) % playerLoaders.Count;
                            }
                            
                            if (slotIdx == 0)
                            {
                                Utility.MW.p1_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p1_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p1_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p1_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p1_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p1_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p1_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p1_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 1)
                            {
                                Utility.MW.p2_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p2_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p2_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p2_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p2_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p2_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p2_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p2_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 2)
                            {
                                Utility.MW.p3_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p3_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p3_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p3_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p3_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p3_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p3_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p3_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            else if (slotIdx == 3)
                            {
                                Utility.MW.p4_Img.Source = faces[selectedDummyPlayerIdx[slotIdx]][0];
                                Utility.MW.p4_Name.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().playerName;
                                Utility.MW.p4_Hp.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                //Utility.MW.p4_Hp.Value = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxHp / 100 * 90;
                                Utility.MW.p4_Bomb.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().maxBomb;
                                Utility.MW.p4_Atk.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().atk;
                                Utility.MW.p4_Power.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().power;
                                Utility.MW.p4_Speed.Content = playerLoaders[selectedDummyPlayerIdx[slotIdx]].getDummyPlayer().speed;
                            }
                            break;
                        case Key.Space:
                            lockSelectedDummyPlayerIdx[slotIdx] = -1;
                            break;

                    }
                }
                else
                {
                    switch (e.Key)
                    {
                        case Key.Space:
                            lockSelectedDummyPlayerIdx[slotIdx] = 1;
                            break;
                    }
                }
            }
            else
            {
                if (devices.Count < MAX_PLAYERS)
                {
                    devices.Add(e.DeviceHnd);
                    selectedDummyPlayerIdx.Add(0);
                }

            }
        }


        public void completeSelectPlayer()
        {
            Utility.MW.onRawKeyUp -= MW_PreviewKeyUp;
            Utility.MW.mdaSe.Stop();
            Utility.MW.mdaSe.Source = new Uri("Musics/magic.mp3", UriKind.Relative);

            Utility.MW.mdaSe.Play();
            IList<IPlayerLoader> selectedPlayerLoader = new List<IPlayerLoader>();
            foreach (int pDummyPlayerIdx in selectedDummyPlayerIdx)
            {
                selectedPlayerLoader.Add(playerLoaders[pDummyPlayerIdx]);
            }



            onPlayerSelectComplete.Invoke(this, new PlayerSelectCompleteEventArgs(devices.ToArray(), selectedPlayerLoader.ToArray()));

        }

    }
}
