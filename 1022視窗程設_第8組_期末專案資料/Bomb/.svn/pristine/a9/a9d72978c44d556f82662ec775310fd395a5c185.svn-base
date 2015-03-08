using Bomb.Scene;
using Bomb.Src.Game;
using Bomb.Src.Scene;
using Bomb.Src.Utils;
using Bomb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bomb.Src.Weapon
{
    class WBomb : Image
    {
        private static int bombIdCounter = 1;
        public static ImgSrcs bombSrc = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/Bomb.png", UriKind.Relative), 32, 32, 0));
        private static Dictionary<WalkDirection, ImgSrcs> detonateSrc;

        private static ImgSrcs coreDetonateSrc = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/CoreDetonate.png", UriKind.Relative), 40, 40, 0));
        private static TimeSpan detonateTime = TimeSpan.FromMilliseconds(2000);
        private static TimeSpan layAniDuration = TimeSpan.FromMilliseconds(500);
        private static int detonateSpeed = 5;

        

        private GameScene scene
        {
            get
            {
                return GameManager.currGameMgr.scene;
            }
        }
        private int bombId { get; set; }
        private Player owner { get; set; }
        private Storyboard layStb { get; set; }
        public Dictionary<WalkDirection, Rectangle> detonateRects { get; private set; }
        private Dictionary<WalkDirection, ImageBrush> detonateBrushes { get; set; }
        private Dictionary<WalkDirection, Storyboard> detonateStb { get; set; }
        private IList<object> currDestoryList { get; set; }
        private int detonateCompleteNum { get; set; }
        static WBomb()
        {
            WBomb.detonateSrc = new Dictionary<WalkDirection, ImgSrcs>();
            detonateSrc[WalkDirection.DOWN] = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/DownDetonate.png", UriKind.Relative), 40, 40, 0));
            detonateSrc[WalkDirection.LEFT] = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/LeftDetonate.png", UriKind.Relative), 40, 40, 0));
            detonateSrc[WalkDirection.RIGHT] = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/RightDetonate.png", UriKind.Relative), 40, 40, 0));
            detonateSrc[WalkDirection.UP] = new ImgSrcs(new TileImgSrcsLoader(new Uri("Images/Tiles/UpDetonate.png", UriKind.Relative), 40, 40, 0));

            

        }
        public WBomb(Player owner) :base()
        {
            this.bombId = bombIdCounter++;
            this.owner = owner;
            this.Source = bombSrc[0];
            this.Width = scene.tileWidth;
            this.Height = scene.tileHeight;
            this.Stretch = System.Windows.Media.Stretch.Uniform;
            this.layStb = initLayStb();
            this.timer = new DispatcherTimer();
            timer.Interval = detonateTime;
            timer.Tick += timer_Tick;

            this.currDestoryList = new List<object>();

            this.detonateBrushes = new Dictionary<WalkDirection, ImageBrush>();
            this.detonateRects = new Dictionary<WalkDirection, Rectangle>();


            Rectangle detonateDownRect = new Rectangle();
            detonateDownRect.Height = 0;
            detonateDownRect.Width = scene.tileWidth;
            ImageBrush downBrush = new ImageBrush(detonateSrc[WalkDirection.DOWN][0]);
            Utility.MW.RegisterName("downBrush" + bombId, downBrush);
            downBrush.TileMode = TileMode.Tile;
            downBrush.Stretch = Stretch.Uniform;
            downBrush.ViewportUnits = BrushMappingMode.Absolute;
            downBrush.Viewport = new Rect(0, 0, scene.tileWidth, scene.tileHeight);
            detonateDownRect.Fill = downBrush;
            detonateRects[WalkDirection.DOWN] = detonateDownRect;
            detonateBrushes[WalkDirection.DOWN] = downBrush;
            

            Rectangle detonateLeftRect = new Rectangle();
            detonateLeftRect.Height = scene.tileHeight;
            detonateLeftRect.Width = 0;
            ImageBrush leftBrush = new ImageBrush(detonateSrc[WalkDirection.LEFT][0]);
            leftBrush.TileMode = TileMode.Tile;
            leftBrush.Stretch = Stretch.Uniform;
            leftBrush.ViewportUnits = BrushMappingMode.Absolute;
            leftBrush.Viewport = new Rect(0, 0, scene.tileWidth, scene.tileHeight);
            detonateLeftRect.Fill = leftBrush;
            detonateRects[WalkDirection.LEFT] = detonateLeftRect;
            detonateBrushes[WalkDirection.LEFT] = leftBrush;

            Rectangle detonateRightRect = new Rectangle();
            detonateRightRect.Height = scene.tileHeight;
            detonateRightRect.Width = 0;
            ImageBrush rightBrush = new ImageBrush(detonateSrc[WalkDirection.RIGHT][0]);
            rightBrush.TileMode = TileMode.Tile;
            rightBrush.Stretch = Stretch.Uniform;
            rightBrush.ViewportUnits = BrushMappingMode.Absolute;
            rightBrush.Viewport = new Rect(0, 0, scene.tileWidth, scene.tileHeight);
            detonateRightRect.Fill = rightBrush;
            detonateRects[WalkDirection.RIGHT] = detonateRightRect;
            detonateBrushes[WalkDirection.RIGHT] = rightBrush;

            Rectangle detonateUpRect = new Rectangle();
            detonateUpRect.Height = 0;
            detonateUpRect.Width = scene.tileWidth;
            ImageBrush upBrush = new ImageBrush(detonateSrc[WalkDirection.UP][0]);
            upBrush.TileMode = TileMode.Tile;
            upBrush.Stretch = Stretch.Uniform;
            upBrush.ViewportUnits = BrushMappingMode.Absolute;
            upBrush.Viewport = new Rect(0, 0, scene.tileWidth, scene.tileHeight);
            detonateUpRect.Fill = upBrush;
            detonateRects[WalkDirection.UP] = detonateUpRect;
            detonateBrushes[WalkDirection.UP] = upBrush;


            this.detonateStb = new Dictionary<WalkDirection, Storyboard>();

            Storyboard downStb = new Storyboard();
            downStb.Children.Add(createDetonateStretchAni(WalkDirection.DOWN, owner.power));
            downStb.Children.Add(createDetonateViewportAni(WalkDirection.DOWN, owner.power));
            downStb.Completed += detonateStb_Completed;
            detonateStb[WalkDirection.DOWN] = downStb;

            Storyboard leftStb = new Storyboard();
            leftStb.Children.Add(createDetonateStretchAni(WalkDirection.LEFT, owner.power));
            leftStb.Children.Add(createDetonateViewportAni(WalkDirection.LEFT, owner.power));
            leftStb.Children.Add(createDetonateOffsetAni(WalkDirection.LEFT, owner.power));
            leftStb.Completed += detonateStb_Completed;
            detonateStb[WalkDirection.LEFT] = leftStb;

            Storyboard rightStb = new Storyboard();
            rightStb.Children.Add(createDetonateStretchAni(WalkDirection.RIGHT, owner.power));
            rightStb.Children.Add(createDetonateViewportAni(WalkDirection.RIGHT, owner.power));
            rightStb.Completed += detonateStb_Completed;
            detonateStb[WalkDirection.RIGHT] = rightStb;

            Storyboard upStb = new Storyboard();
            upStb.Children.Add(createDetonateStretchAni(WalkDirection.UP, owner.power));
            upStb.Children.Add(createDetonateViewportAni(WalkDirection.UP, owner.power));
            upStb.Children.Add(createDetonateOffsetAni(WalkDirection.UP, owner.power));
            upStb.Completed += detonateStb_Completed;
            detonateStb[WalkDirection.UP] = upStb;

            



        }

        void detonateStb_Completed(object sender, EventArgs e)
        {
            lock (this)
            {
                if (++detonateCompleteNum == Enum.GetNames(typeof(WalkDirection)).Length)
                {
                    detonateComplete();
                }
                //else
                //{
                //    double totalLen = 0;
                //    foreach (KeyValuePair<WalkDirection,Rectangle> detonateRect in detonateRects)
                //    {
                //          switch (detonateRect.Key)
                //        {
                //            case WalkDirection.DOWN:
                //            case WalkDirection.UP:
                //                totalLen += detonateRect.Value.ActualHeight;
                //                break;
                //            case WalkDirection.LEFT:
                //            case WalkDirection.RIGHT:
                //                totalLen += detonateRect.Value.ActualWidth;
                //                break;

                //        }
                        
                //    }
                //    if (totalLen <= 0)
                //    {
                //        detonateComplete();
                //    }
                //}
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {

            timer.Stop();
            layStb.Stop();
            detonate();
        }
        public bool isLaid
        {
            get
            {
                return owner.ownedBombs.Contains(this);
            }
        }
        public int _laidGridX;
        public int laidGridX
        {
            get
            {
                return _laidGridX;
            }
            set
            {
                double newX = (value % scene.width) * scene.tileWidth + canvasXOffset;

                this.SetValue(Canvas.LeftProperty, newX);
                detonateRects[WalkDirection.DOWN].SetValue(Canvas.LeftProperty, newX);
                detonateRects[WalkDirection.LEFT].SetValue(Canvas.LeftProperty, newX);
                detonateRects[WalkDirection.RIGHT].SetValue(Canvas.LeftProperty, newX + scene.tileWidth);
                detonateRects[WalkDirection.UP].SetValue(Canvas.LeftProperty, newX);
                _laidGridX = value;
            }
        }
        public int _laidGridY;
        public int laidGridY
        {
            get
            {
                return _laidGridY;
            }
            set
            {
                double newY = (value % scene.height) * scene.tileHeight + canvasYOffset;
                this.SetValue(Canvas.TopProperty, newY);
                detonateRects[WalkDirection.DOWN].SetValue(Canvas.TopProperty, newY + scene.tileHeight);
                detonateRects[WalkDirection.LEFT].SetValue(Canvas.TopProperty, newY);
                detonateRects[WalkDirection.RIGHT].SetValue(Canvas.TopProperty, newY);
                detonateRects[WalkDirection.UP].SetValue(Canvas.TopProperty, newY);
                _laidGridY = value;
            }
        }
        public double canvasXOffset
        {
            get
            {
                return (scene.tileWidth - this.Width) / 2;
            }
        }
        public double canvasYOffset
        {
            get
            {
                return (scene.tileHeight - this.Height) / 2;
            }
        }
        private DispatcherTimer timer { get; set; }
        public Storyboard initLayStb()
        {
            Storyboard retVal = new Storyboard();
            ObjectAnimationUsingKeyFrames layAni = bombSrc.getFrameAnimation(0, 3, layAniDuration, 0, this, null);
            retVal.Children.Add(layAni);

            return retVal;
        }
        public void lay()
        {
            Utility.MW.mdaSe.Stop();
            Utility.MW.mdaSe.Source = new Uri("Musics/ok.mp3", UriKind.Relative);
            Utility.MW.mdaSe.Play();
            if (!owner.isMoving)
            {
                laidGridX = owner.gridX;
                laidGridY = owner.gridY;
            }
            else
            {
                switch (owner.currDir)
                {
                    case WalkDirection.DOWN:

                        if (owner.gridY - 1 < 0)
                        {
                            return;
                        }
                        laidGridX = owner.gridX;
                        laidGridY = owner.gridY - 1;


                        break;
                    case WalkDirection.LEFT:
                        if (owner.gridX + 1 >= scene.width)
                        {
                            return;
                        }
                        laidGridX = owner.gridX + 1;
                        laidGridY = owner.gridY;
                        break;
                    case WalkDirection.RIGHT:
                        if (owner.gridX - 1 < 0)
                        {
                            return;
                        }
                        laidGridX = owner.gridX - 1;
                        laidGridY = owner.gridY;
                        break;
                    case WalkDirection.UP:
                        if (owner.gridY + 1>= scene.height)
                        {
                            return;
                        }
                        laidGridX = owner.gridX;
                        laidGridY = owner.gridY + 1;
                        break;
                }
            }

            scene.bombLayer.addBomb(this);
            
            layStb.Begin();


            //down
            int downActualPower = getActualPower(WalkDirection.DOWN);
            editDetonateStretchAni(WalkDirection.DOWN, downActualPower);
            editDetonateViewportAni(WalkDirection.DOWN,downActualPower);
            //left
            int leftActualPower = getActualPower(WalkDirection.LEFT);
            editDetonateStretchAni(WalkDirection.LEFT, leftActualPower);
            editDetonateViewportAni(WalkDirection.LEFT, leftActualPower);
            editDetonateOffsetAni(WalkDirection.LEFT, leftActualPower);

            //right
            int rightActualPower = getActualPower(WalkDirection.RIGHT);
            editDetonateStretchAni(WalkDirection.RIGHT, rightActualPower);
            editDetonateViewportAni(WalkDirection.RIGHT, rightActualPower);

            //up
            int upActualPower = getActualPower(WalkDirection.UP);
            editDetonateStretchAni(WalkDirection.UP, upActualPower);
            editDetonateViewportAni(WalkDirection.UP, upActualPower);
            editDetonateOffsetAni(WalkDirection.UP, upActualPower);

            timer.Start();

        }
        private void detonate()
        {
            detonateCompleteNum = 0;
            Source = coreDetonateSrc[0];
            foreach (WalkDirection dir in Enum.GetValues(typeof(WalkDirection)))
            {
                detonateStb[dir].Begin();
            }
        }
        private int getActualPower(WalkDirection dir)
        {
            int retVal = 0;
            for (int i = 0; i < owner.power; i++)
            {
                retVal = i;
                int x = 0;
                int y = 0;
                switch (dir)
                {
                    case WalkDirection.DOWN:
                        x = laidGridX;
                        y = laidGridY + i + 1;
                        break;
                    case WalkDirection.LEFT:
                        x = laidGridX - i - 1;
                        y = laidGridY;
                        break;
                    case WalkDirection.RIGHT:
                        x = laidGridX + i + 1;
                        y = laidGridY;
                        break;
                    case WalkDirection.UP:
                        x = laidGridX;
                        y = laidGridY - i - 1;
                        break;

                }
                if (x < 0 || x >= scene.width || y <0 || y >= scene.height)
                {
                    //out of bound
                    break;
                }
                if (scene.obstacleLayer.obstacles[y, x] != null)
                {
                    //obstacle
                    break;
                }
                if (i == owner.power - 1)
                {
                    retVal++;
                }
            }
            return retVal;

        }
        private DoubleAnimation createDetonateStretchAni(WalkDirection dir, int power)
        {
            DoubleAnimation retVal = new DoubleAnimation();
            retVal.Name = dir.ToString();
            retVal.From = 0;
            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.UP:
                    retVal.To = power * scene.tileHeight;
                    break;
                case WalkDirection.LEFT:
                case WalkDirection.RIGHT:
                    retVal.To = power * scene.tileWidth;
                    break;

            }
            retVal.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);
            retVal.FillBehavior = FillBehavior.Stop;
            retVal.AutoReverse = true;
            retVal.EasingFunction = new CubicEase() {  EasingMode= EasingMode.EaseInOut};
            Storyboard.SetTarget(retVal, detonateRects[dir]);
            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.UP:
                    Storyboard.SetTargetProperty(retVal, new PropertyPath(Shape.HeightProperty));
                    break;
                case WalkDirection.LEFT:
                case WalkDirection.RIGHT:
                    Storyboard.SetTargetProperty(retVal, new PropertyPath(Shape.WidthProperty));
                    break;

            }

            switch (dir)
            {
                case WalkDirection.DOWN:
                    retVal.CurrentTimeInvalidated += detonateStretchAni_TimeInvalidated;
                    break;
                case WalkDirection.UP:
                    retVal.CurrentTimeInvalidated += detonateStretchAni_TimeInvalidated;
                    break;
                case WalkDirection.LEFT:
                    retVal.CurrentTimeInvalidated += detonateStretchAni_TimeInvalidated;
                    break;
                case WalkDirection.RIGHT:
                    retVal.CurrentTimeInvalidated += detonateStretchAni_TimeInvalidated;
                    break;

            }


            return retVal;
        }

        private void editDetonateStretchAni(WalkDirection dir, int power)
        {
            DoubleAnimation ani = detonateStb[dir].Children[0] as DoubleAnimation;

            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.UP:
                    ani.To = power * scene.tileHeight;
                    break;
                case WalkDirection.LEFT:
                case WalkDirection.RIGHT:
                    ani.To = power * scene.tileWidth;
                    break;

            }
            ani.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);

        }

        private void detonateStretchAni_TimeInvalidated(object sender, EventArgs e)
        {
            AnimationClock clock = sender as AnimationClock;
            
            DoubleAnimation ani = clock.Timeline as DoubleAnimation;
            WalkDirection dir = (WalkDirection)Enum.Parse(typeof(WalkDirection), ani.Name);
            int atkRange = 0;
            
            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.UP:
                    atkRange = (int)Math.Ceiling(detonateRects[dir].ActualHeight / scene.tileHeight);
                    break;
                case WalkDirection.LEFT:
                case WalkDirection.RIGHT:
                    atkRange = (int)Math.Ceiling(detonateRects[dir].ActualWidth / scene.tileWidth);
                    break;
               

            }

            IList<Player> injuredPlayers = null;
            IList<Box> destroiedBoxes = null;
            IList<Shelter> destroiedShelters = null;
            switch (dir)
            {
                case WalkDirection.DOWN:
                    injuredPlayers = scene.roleLayer.playerWithinActualGrids(laidGridX,laidGridY,1,atkRange);
                    destroiedBoxes = scene.boxLayer.boxWithinGrids(laidGridX, laidGridY, 1, atkRange);
                    destroiedShelters = scene.shelterLayer.shelterWithinGrids(laidGridX, laidGridY, 1, atkRange);
                    break;
                case WalkDirection.UP:
                    injuredPlayers = scene.roleLayer.playerWithinActualGrids(laidGridX,laidGridY-atkRange,1,atkRange);
                    destroiedBoxes = scene.boxLayer.boxWithinGrids(laidGridX, laidGridY-atkRange, 1, atkRange);
                    destroiedShelters = scene.shelterLayer.shelterWithinGrids(laidGridX, laidGridY-atkRange, 1, atkRange);
                    break;
                case WalkDirection.LEFT:
                    injuredPlayers = scene.roleLayer.playerWithinActualGrids(laidGridX-atkRange,laidGridY,atkRange,1);
                    destroiedBoxes = scene.boxLayer.boxWithinGrids(laidGridX-atkRange, laidGridY, atkRange,1 );
                    destroiedShelters = scene.shelterLayer.shelterWithinGrids(laidGridX - atkRange, laidGridY,atkRange, 1);
                    break;
                case WalkDirection.RIGHT:
                    injuredPlayers = scene.roleLayer.playerWithinActualGrids(laidGridX,laidGridY,atkRange,1);
                    destroiedBoxes = scene.boxLayer.boxWithinGrids(laidGridX, laidGridY, atkRange, 1);
                    destroiedShelters = scene.shelterLayer.shelterWithinGrids(laidGridX, laidGridY, atkRange, 1);
                    break;


            }
            
            foreach (Player player in injuredPlayers)
            {
                
                player.currHp -= owner.atk;
            }
            foreach (Box box in destroiedBoxes)
            {
                scene.boxLayer.destroy(box);
            }
            foreach (Shelter shelter in destroiedShelters)
            {
                scene.shelterLayer.destroy(shelter);
            }
        }
       
        private RectAnimation createDetonateViewportAni(WalkDirection dir,int power)
        {
            RectAnimation retVal = new RectAnimation();
            retVal.From = new Rect(0, 0, scene.tileWidth, scene.tileHeight);
            switch (dir)
            {
                case WalkDirection.DOWN:
                    retVal.To = new Rect(0, power * scene.tileHeight, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.LEFT:
                    retVal.To = new Rect(power * -scene.tileWidth, 0, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.RIGHT:
                    retVal.To = new Rect(power * scene.tileWidth, 0, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.UP:
                    retVal.To = new Rect(0, power * -scene.tileHeight, scene.tileWidth, scene.tileHeight);
                    break;

            }
            retVal.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);
            
            
            retVal.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(retVal, detonateRects[dir]);
            //detonateBrushes[dir].Viewport
            Storyboard.SetTargetProperty(retVal, new PropertyPath("Fill.(ImageBrush.Viewport)"));
            return retVal;
        }

        private void editDetonateViewportAni(WalkDirection dir, int power)
        {
            RectAnimation ani = detonateStb[dir].Children[1] as RectAnimation;
            
            switch (dir)
            {
                case WalkDirection.DOWN:
                    ani.To = new Rect(0, power * scene.tileHeight*10, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.LEFT:
                    ani.To = new Rect(power * -scene.tileWidth, 0, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.RIGHT:
                    ani.To = new Rect(power * scene.tileWidth, 0, scene.tileWidth, scene.tileHeight);
                    break;
                case WalkDirection.UP:
                    ani.To = new Rect(0, power * -scene.tileHeight, scene.tileWidth, scene.tileHeight);
                    break;

            }
            ani.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);
          
            
        }

       
        private DoubleAnimation createDetonateOffsetAni(WalkDirection dir, int power)
        {
            DoubleAnimation retVal = new DoubleAnimation();
            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.RIGHT:
                    throw new Exception("Direction DOWN and RIGHT does not need this animation");
                    break;
                case WalkDirection.LEFT:
                    retVal.By = -power * scene.tileWidth;
                    break;


                case WalkDirection.UP:
                    retVal.By = -power * scene.tileHeight;
                    break;

            }
            retVal.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);
            retVal.FillBehavior = FillBehavior.Stop;
            retVal.AutoReverse = true;
            retVal.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseInOut };
            Storyboard.SetTarget(retVal, detonateRects[dir]);
            switch (dir)
            {

                case WalkDirection.UP:
                    Storyboard.SetTargetProperty(retVal, new PropertyPath(Canvas.TopProperty));
                    break;
                case WalkDirection.LEFT:

                    Storyboard.SetTargetProperty(retVal, new PropertyPath(Canvas.LeftProperty));
                    break;

            }
            return retVal;
        }
        private void editDetonateOffsetAni(WalkDirection dir, int power)
        {
            if (dir == WalkDirection.DOWN || dir == WalkDirection.RIGHT)
            {
                throw new Exception("Direction DOWN and RIGHT does not need this animation");
            }
            DoubleAnimation ani = detonateStb[dir].Children[2] as DoubleAnimation;
            switch (dir)
            {
                case WalkDirection.LEFT:
                    ani.By = -power * scene.tileWidth;
                    break;


                case WalkDirection.UP:
                    ani.By = -power * scene.tileHeight;
                    break;

            }
            ani.Duration = TimeSpan.FromSeconds((double)power / detonateSpeed);

        }
        private void detonateComplete()
        {
            this.detonateCompleteNum = 0;
            if (scene.bombLayer.Children.Contains(this))
            {
                scene.bombLayer.removeBomb(this);
            }
            
            Source = bombSrc[0];
            if (!owner.ownedBombs.Contains(this))
            {
                owner.ownedBombs.Enqueue(this);
            }
            
        }



    }
}
