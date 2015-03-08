﻿using Bomb.Scene;
using Bomb.Src.Game;
using Bomb.Src.Role;
using Bomb.Src.Scene;
using Bomb.Src.Utils;
using Bomb.Src.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Bomb.Src
{
    class Player : Image
    {
        public GameScene scene 
        {
            get 
            {
                return GameManager.currGameMgr.scene;
            } 
        }
        private IPlayerLoader playerLoader { get; set; }
        public int playerId { get; set; }
        public PlayerInfoPanel infoPanel { get;private set; }
        public string playerName { get;private set; }
        private int _maxHp;
        public int maxHp
        {
            get
            {
                return _maxHp;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value < currHp)
                {
                    currHp = value;
                }
                infoPanel.maxHp = value;
                _maxHp = value;
            }
        }
        private int _currHp;
        public int currHp 
        {
            get
            {
                return _currHp;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _currHp = value;
                infoPanel.hp = value;

                if (currHp == 0)
                {
                    this.acceptMovement = false;
                    this.Visibility = Visibility.Hidden;
                }
            }
        }
        public ImgSrcs walkImgSrcs { get; set; }
        public int _maxBombs;
        public int maxBombs
        {
            get
            {
                return _maxBombs;
            }
            set
            {
                if (value < _maxBombs)
                {
                    throw new NotSupportedException("Not support decrease of maxBombs");
                }
                int deltaBombs = value - _maxBombs;
                for (int i = 0; i < deltaBombs; i++)
                {
                    ownedBombs.Enqueue(new WBomb(this));
                }
                infoPanel.maxBomb = value;
                _maxBombs = value;
            }
        }
        public int currOwnedBombs 
        {
            get
            {
                return ownedBombs.Count;
            }
        }
        public bool hurtable { get; set; }
        private bool acceptMovement { get; set; }
        public bool isMoving { get;private set; }
        private double _speed;
        public double speed
        {
            get 
            {
                return _speed;
            }
            set 
            {
                if (value < 0)
                {
                    value = 0;
                }
                _speed = value;
                infoPanel.speed = value;
                foreach (KeyValuePair<WalkDirection, Storyboard> entry in moveStbs)
                {
                    if (entry.Value == currAnimation)
                    {
                        entry.Value.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)(1000/speed)));
                    }
                }

            }
        }

        private int _power;
        public int power
        {
            get
            {
                return _power;
            }
            set
            {
                _power = value;
                if (value < 0)
                {
                    value = 0;
                }
                infoPanel.power = value;
            }
        }
        private int _atk;
        public int atk
        {
            get
            {
                return _atk;
            }
            set
            {
                _atk = value;
                if (value < 0)
                {
                    value = 0;
                }
                infoPanel.atk = value;
            }
        }
        private Storyboard _currAnimation;
        public Storyboard currAnimation 
        {
            get
            {
                return _currAnimation;
            }
            set 
            {
                if (_currAnimation != null)
                {
                    _currAnimation.Pause();
                }
                _currAnimation = value;
                if (value != null)
                {
                    value.Begin();
                }
                
            }
        }
        public KeyControl ctrl { get; private set; }
        public long device { get; set; }
        public Dictionary<WalkDirection, Storyboard> moveStbs { get; set; }
        public Dictionary<WalkDirection, Storyboard> walkStbs { get; set; }
        private WalkDirection _currDir;
        public WalkDirection currDir
        {
            get
            {
                return _currDir;
            }
            set
            {
                _currDir = value;
                this.Source = walkImgSrcs[1 + 4 * (int)value];
            }
        }
        private int _gridX;
        public int gridX
        {
            get
            {
                return _gridX;
            }
            set
            {
                this.SetValue(Canvas.LeftProperty, (value % scene.width) * scene.tileWidth + canvasXOffset);
                _gridX = value;
            }
        }
        private int _gridY;
        public int gridY
        {
            get
            {
                return _gridY;
            }
            set
            {
                this.SetValue(Canvas.TopProperty, (value % scene.height) * scene.tileHeight + canvasYOffset);
                _gridY = value;
            }
        }
        public double canvasX
        {
            get
            {
                return (gridX % scene.width) * scene.tileWidth + canvasXOffset;
            }
        }
        public double canvasY
        {
            get
            {
                return (gridY % scene.width) * scene.tileWidth + canvasXOffset;
            }
        }
        public double canvasXOffset
        {
            get
            {
                return (scene.tileWidth - walkImgSrcs[0].Width) / 2;
            }
        }
        public double canvasYOffset
        {
            get
            {
                return (scene.tileHeight - walkImgSrcs[0].Height)/2;
            }
        }
        public int actualGridX
        {
            get
            {
                double actualLeft = (double)this.GetValue(Canvas.LeftProperty) - canvasXOffset;
                return (int)Math.Ceiling(actualLeft/scene.tileWidth);
            }
            
        }
        public int actualGridY
        {
            get
            {
                double actualTop = (double)this.GetValue(Canvas.TopProperty) - canvasYOffset;
                return (int)Math.Ceiling(actualTop / scene.tileHeight);
            }

        }

        public Queue<WBomb> ownedBombs { get; set; }
        public Player(IPlayerLoader playerLoader,long device)
            : base()

        {
            this.playerLoader = playerLoader;
            this.infoPanel = new PlayerInfoPanel();
            infoPanel.Loaded += infoPanel_Loaded;
            this.hurtable = true;
            this.acceptMovement = true;
            this.playerName = playerLoader.getPlayerName();
            this.walkImgSrcs = playerLoader.getWalkImgSrcs();
            this.ctrl = playerLoader.getKeyControl();
            this.device = device;
            this._speed = playerLoader.getSpeed();
            this.moveStbs = initMoveStb();
            this.currDir = WalkDirection.DOWN;
            

            initCtrl();
            this.ownedBombs = initOwnedBomb();
        }

        private void infoPanel_Loaded(object sender, RoutedEventArgs e)
        {
            this.maxHp = playerLoader.getMaxHp();
            this.currHp = this.maxHp;
            this.speed = playerLoader.getSpeed();
            this.maxBombs = playerLoader.getMaxBomb();
            this.power = playerLoader.getPower();
            this.atk = playerLoader.getAtk();
            infoPanel.faceImgSrc = playerLoader.getFaceImgSrcs()[0];
        }
        
        private Dictionary<WalkDirection, Storyboard> initMoveStb()
        {
            Dictionary<WalkDirection,Storyboard> retVal = new Dictionary<WalkDirection,Storyboard>();
            walkStbs = new Dictionary<WalkDirection, Storyboard>();
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, (int)(1000 / speed));
            //down
            
            Storyboard walkDownStb = new Storyboard();
            ObjectAnimationUsingKeyFrames walkDownAni = walkImgSrcs.getFrameAnimation(0, 3, duration, 1, this, null);
            walkDownStb.Children.Add(walkDownAni);
            walkStbs[WalkDirection.DOWN] = walkDownStb;

            Storyboard moveDownStb = new Storyboard();
            moveDownStb.Name = WalkDirection.DOWN.ToString();
            DoubleAnimation increaseYAni = new DoubleAnimation();
            increaseYAni.FillBehavior = FillBehavior.HoldEnd;
            increaseYAni.Completed += walkAnimation_Complete;
            increaseYAni.Duration = new Duration(duration);
            increaseYAni.By = scene.tileHeight;
            moveDownStb.Children.Add(increaseYAni);
            Storyboard.SetTarget(increaseYAni, this);
            Storyboard.SetTargetProperty(increaseYAni, new PropertyPath("(Canvas.Top)"));
            retVal[WalkDirection.DOWN] = moveDownStb;
            

            //left
            
            Storyboard walkLeftStb = new Storyboard();
            ObjectAnimationUsingKeyFrames walkLeftAni = walkImgSrcs.getFrameAnimation(4, 7, duration, 1, this, null);
            walkLeftStb.Children.Add(walkLeftAni);
            walkStbs[WalkDirection.LEFT] = walkLeftStb;

            Storyboard moveLeftStb = new Storyboard();
            moveLeftStb.Name = WalkDirection.LEFT.ToString();
            DoubleAnimation decreaseXAni = new DoubleAnimation();
            decreaseXAni.FillBehavior = FillBehavior.HoldEnd;
            decreaseXAni.Completed += walkAnimation_Complete;
           
            decreaseXAni.Duration = new Duration(duration);
            decreaseXAni.By = -scene.tileWidth;
            moveLeftStb.Children.Add(decreaseXAni);
            Storyboard.SetTarget(decreaseXAni, this);
            Storyboard.SetTargetProperty(decreaseXAni, new PropertyPath("(Canvas.Left)"));
            retVal[WalkDirection.LEFT] = moveLeftStb;
           

            //right
            Storyboard walkRightStb = new Storyboard();
            ObjectAnimationUsingKeyFrames walkRightAni = walkImgSrcs.getFrameAnimation(8, 11, duration, 1, this, null);
            walkRightStb.Children.Add(walkRightAni);
            walkStbs[WalkDirection.RIGHT] = walkRightStb;

            Storyboard moveRightStb = new Storyboard();
            moveRightStb.Name = WalkDirection.RIGHT.ToString();
            DoubleAnimation increaseXAni = new DoubleAnimation();
            increaseXAni.FillBehavior = FillBehavior.HoldEnd;
            increaseXAni.Completed += walkAnimation_Complete;
            
            increaseXAni.Duration = new Duration(duration);
            increaseXAni.By = scene.tileWidth;
            moveRightStb.Children.Add(increaseXAni);
            Storyboard.SetTarget(increaseXAni, this);
            Storyboard.SetTargetProperty(increaseXAni, new PropertyPath("(Canvas.Left)"));
            
            retVal[WalkDirection.RIGHT] = moveRightStb;
            

            //up
            Storyboard walkUpStb = new Storyboard();
            ObjectAnimationUsingKeyFrames  walkUpAni = walkImgSrcs.getFrameAnimation(12, 15, duration, 1, this, null);
            walkUpStb.Children.Add(walkUpAni);
            walkStbs[WalkDirection.UP] = walkUpStb;

            Storyboard moveUpStb = new Storyboard();
            moveUpStb.Name = WalkDirection.UP.ToString();
            DoubleAnimation decreaseYAni = new DoubleAnimation();
            decreaseYAni.FillBehavior = FillBehavior.HoldEnd;
            decreaseYAni.Completed += walkAnimation_Complete;
            
            decreaseYAni.Duration = new Duration(duration);
            decreaseYAni.By = -scene.tileHeight;
            moveUpStb.Children.Add(decreaseYAni);
            Storyboard.SetTarget(decreaseYAni, this);
            Storyboard.SetTargetProperty(decreaseYAni, new PropertyPath("(Canvas.Top)"));
            retVal[WalkDirection.UP] = moveUpStb;
            return retVal;
            
        }

        

        
        
        private void stand(WalkDirection dir)
        {
            foreach (KeyValuePair<WalkDirection, Storyboard> entry in moveStbs)
            {
                if (entry.Value == currAnimation)
                {
                    entry.Value.Stop();
                }
            }
            currDir = dir;

        }
        private void initCtrl()
        {
            Utility.MW.onRawKeyDown += player_KeyDown;
            Utility.MW.onRawKeyUp += player_KeyUp;
            
        }

        private void player_KeyUp(object sender, RawKeyInputEventArg e)
        {
            if (e.DeviceHnd != this.device)
            {
                return;
            }
            if (ctrl.keyState.ContainsKey(e.Key))
            {
                ctrl.keyState[e.Key] = false;
            }

            if (currOwnedBombs > 0 && e.Key == ctrl.layBombKey)
            {
                ownedBombs.Dequeue().lay();

            }
        }


        //private void player_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        private void player_KeyDown(object sender, RawKeyInputEventArg e)
        {
            if (e.DeviceHnd != this.device)
            {
                return;
            }
            
            if (ctrl.keyState.ContainsKey(e.Key))
            {
                ctrl.keyState[e.Key] = true;
            }

            if (ctrl.walkKeyBinding.ContainsKey(e.Key))
            {
                
                if (GameManager.currGameMgr.isGaming && acceptMovement && !isMoving)
                {
                    
                    WalkDirection dir = ctrl.walkKeyBinding[e.Key];
                    currDir = dir;
                    if (validateOneStep(dir))
                    {
                        moveOneStep(dir);
                    }
                }
            }

            
        }
        private void walkAnimation_Complete(object sender, EventArgs e)
        {
            AnimationClock clk = sender as AnimationClock;

            string name = (clk.Parent.Timeline as Storyboard).Name;
            WalkDirection dir = (WalkDirection)Enum.Parse(typeof(WalkDirection), name);
            walkStbs[dir].Stop();
            foreach (KeyValuePair<Key, WalkDirection> entry in ctrl.walkKeyBinding)
            {
                if (ctrl.keyState[entry.Key])
                {
                    if (validateOneStep(entry.Value))
                    {
                        moveOneStep(entry.Value);
                        return;
                    }
                   
                }
            }
            acceptMovement = true;
            isMoving = false;
        }

        private bool validateOneStep(WalkDirection dir)
        {
            bool retVal = true;
            Box neighborBox = null;
            switch (dir)
            {
                case WalkDirection.DOWN:
                    if(gridY+1 < scene.height)
                    {
                        neighborBox = scene.boxLayer.boxOnGrid(gridX, gridY + 1);
                        retVal &= scene.obstacleLayer.obstacles[gridY + 1,gridX] == null;
                    }
                    else
                    {
                        return false;
                    }
                    
                    break;
                case WalkDirection.LEFT:
                    if (gridX - 1 >= 0)
                    {
                        neighborBox = scene.boxLayer.boxOnGrid(gridX - 1, gridY);
                        retVal &= scene.obstacleLayer.obstacles[gridY, gridX - 1] == null;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case WalkDirection.RIGHT:
                    if (gridX + 1 < scene.width)
                    {
                        neighborBox = scene.boxLayer.boxOnGrid(gridX + 1, gridY);
                        retVal &=  scene.obstacleLayer.obstacles[gridY, gridX + 1] == null;
                    }
                    else
                    {
                        return false;
                    }
                   
                    break;
                case WalkDirection.UP:
                    if (gridY - 1 >= 0)
                    {
                        neighborBox = scene.boxLayer.boxOnGrid(gridX, gridY - 1);
                        retVal &= gridY - 1 >= 0 && scene.obstacleLayer.obstacles[gridY - 1, gridX] == null;
                    }
                    else
                    {
                        return false;
                    }
                    
                    break;
            }
            if (!retVal)
            {
                //fastern
                return retVal;
            }
            if (neighborBox!= null)
            {
                retVal &= scene.boxLayer.moveBoxOne(neighborBox, dir, this);
            }
            return retVal;
        }
        private void moveOneStep(WalkDirection dir)
        {
            isMoving = true;
            acceptMovement = false;
            switch (dir)
            {
                case WalkDirection.DOWN:
                    _gridY += 1;
                    break;
                case WalkDirection.LEFT:
                    _gridX -= 1;
                    break;
                case WalkDirection.RIGHT:
                    _gridX += 1;
                    break;
                case WalkDirection.UP:
                    _gridY -= 1;
                    break;
            }
            currAnimation = moveStbs[dir];
            walkStbs[dir].Begin();
            
            
        }

        public Queue<WBomb> initOwnedBomb()
        {
            Queue<WBomb> retVal = new Queue<WBomb>();
            for (int i = 0; i < maxBombs; i++)
            {
                retVal.Enqueue(new WBomb(this));
            }
            return retVal;
        }


    }
}
