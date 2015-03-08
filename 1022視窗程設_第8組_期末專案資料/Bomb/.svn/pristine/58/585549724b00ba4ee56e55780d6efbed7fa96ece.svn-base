using Bomb.Src;
using Bomb.Src.Game;
using Bomb.Src.Scene;
using Bomb.Src.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Bomb.Scene
{
    class BoxLayer :Grid
    {
        public Box[,] boxes { get; private set; }
        public Dictionary<Box, Int32Point> boxPositions { get; private set; }
        private GameScene scene
        {
            get
            {
                return GameManager.currGameMgr.scene;
            }
        }
        public BoxLayer(int rows,int columns,double rowHeight,double columnWidth)
        {
            this.Width = columns * columnWidth;
            this.Height = rows * rowHeight;
            for (int i = 0; i < rows; i++)
            {
                this.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowHeight) });
            }
            for (int i = 0; i < columns; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(columnWidth) });
            }
            this.boxes = new Box[rows,columns];
            this.boxPositions = new Dictionary<Box, Int32Point>();
            
        }
        
        public void addBox(Box box,int row,int column)
        {

            if (boxes[row, column] == null)
            {
                //box.HorizontalAlignment = HorizontalAlignment.Left;
                //box.VerticalAlignment = VerticalAlignment.Top;
                box.RenderTransform = new TranslateTransform(0,0);
                box.SetValue(Grid.ColumnProperty, column);
                box.SetValue(Grid.RowProperty, row);
                boxes[row, column] = box;
                boxPositions[box] = new Int32Point(column, row);
                Children.Add(box);
            
            }
            
            

        }
        public void destroy(Box box)
        {
            Int32Point pos = boxPositions[box];
            if (pos == null)
            {
                return;
            }
            boxes[pos.y, pos.x] = null;
            boxPositions[box] = null;
            Children.Remove(box);
        }
        public void destroyAt(int row, int column)
        {
            if (boxes[row, column] != null)
            {
                destroy(boxes[row, column]);
            }
            
        }
        public IList<Box> boxWithinGrids(int x, int y, int width, int height)
        {
            IList<Box> retVal = new List<Box>();
            for (int i = y; i < y + height;i++ )
            {
                for (int j = x; j < x + width; j++)
                {
                    if (boxes[i, j] != null)
                    {
                        retVal.Add(boxes[i, j]);
                    }
                }
            }
            
            return retVal;
        }
        public Box boxOnGrid(int x, int y)
        {
            IList<Box> boxList =  boxWithinGrids(x, y, 1, 1);
            if (boxList.Count > 0)
            {
                return boxList[0];
            }
            return null;

        }
        public bool moveBoxOne(Box box,WalkDirection dir,Player player)
        {
            Int32Point boxPos = boxPositions[box];
            if (boxPos == null)
            {
                return false;
            }
            Int32Point newBoxPos = new Int32Point(boxPos.x, boxPos.y);
            Box neighborBox = null;
            switch (dir)
            {
                case WalkDirection.DOWN:
                    if (boxPos.y + 1 < boxes.GetLength(0) && scene.obstacleLayer.obstacles[boxPos.y+1,boxPos.x]==null)
                    {
                        if (boxes[boxPos.y + 1, boxPos.x] != null && boxes[boxPos.y + 1, boxPos.x].canKick)
                        {
                            neighborBox = boxes[boxPos.y + 1, boxPos.x];
                            
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                    break;
                case WalkDirection.UP:

                    if (boxPos.y - 1 >= 0 && scene.obstacleLayer.obstacles[boxPos.y - 1, boxPos.x] == null)
                    {
                        if (boxes[boxPos.y - 1, boxPos.x] != null && boxes[boxPos.y - 1, boxPos.x].canKick)
                        {
                            neighborBox = boxes[boxPos.y - 1, boxPos.x];

                        }
                    }
                    else
                    {
                        return false;
                    }
                   
                    break;
                case WalkDirection.LEFT:
                    if (boxPos.x - 1 >= 0 && scene.obstacleLayer.obstacles[boxPos.y,boxPos.x-1]==null)
                    {
                        if (boxes[boxPos.y, boxPos.x - 1] != null && boxes[boxPos.y, boxPos.x - 1].canKick)
                        {
                            neighborBox = boxes[boxPos.y, boxPos.x - 1];
                        }
                    }
                    else
                    {
                        return false;
                    }

                    
                    break;
                case WalkDirection.RIGHT:
                    if (boxPos.x + 1 < boxes.GetLength(1) &&  scene.obstacleLayer.obstacles[boxPos.y,boxPos.x+1]==null)
                    {
                        if (boxes[boxPos.y, boxPos.x + 1] != null && boxes[boxPos.y, boxPos.x + 1].canKick)
                        {
                            neighborBox = boxes[boxPos.y, boxPos.x + 1];
                        }
                    }
                    else
                    {
                        return false;
                    }
                  
                    break;


            }

            if (neighborBox != null )
            {
                if (!moveBoxOne(neighborBox, dir, player))
                {
                    return false;
                }
                
            }

            box.canKick = false;
            DoubleAnimation ani = null;
            switch (dir)
            {
                case WalkDirection.DOWN:
                case WalkDirection.UP:
                    ani = createBoxMoveAni(box, dir, player.speed, scene.tileHeight);
                    break;
                case WalkDirection.RIGHT:
                case WalkDirection.LEFT:
                    ani = createBoxMoveAni(box, dir, player.speed, scene.tileWidth);
                    break;
                
            }
            Storyboard stb = new Storyboard();
            stb.Children.Add(ani);
            ani.Completed+=delegate(object sender,EventArgs e)
                {
                    switch (dir)
                    {
                        case WalkDirection.DOWN:
                            newBoxPos.y += 1;

                            break;
                        case WalkDirection.UP:
                            newBoxPos.y -= 1;
                            

                            break;
                        case WalkDirection.LEFT:
                            newBoxPos.x -= 1;

                            break;
                        case WalkDirection.RIGHT:
                            newBoxPos.x += 1;
                            break;


                    }
                    box.RenderTransform = new TranslateTransform(0, 0);
                    box.SetValue(Grid.RowProperty,newBoxPos.y);
                    box.SetValue(Grid.ColumnProperty, newBoxPos.x);
                    boxes[boxPos.y, boxPos.x] = null;
                    boxes[newBoxPos.y, newBoxPos.x] = box;
                    boxPositions[box] = newBoxPos;
                    box.canKick = true;

                };
            
            stb.Begin();
            return true;






        }

        private DoubleAnimation createBoxMoveAni(Box box,WalkDirection dir,double walkSpped, double by)
        {

            DoubleAnimation retVal = new DoubleAnimation();
            Storyboard.SetTarget(retVal, box);
            switch (dir)
            {
                case WalkDirection.DOWN:
                    retVal.By = scene.tileHeight;
                    Storyboard.SetTargetProperty(retVal, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

                    break;
                case WalkDirection.RIGHT:
                    retVal.By = scene.tileWidth;
                    Storyboard.SetTargetProperty(retVal, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
                    break;
                case WalkDirection.LEFT:
                    retVal.By = -scene.tileWidth;
                    Storyboard.SetTargetProperty(retVal, new PropertyPath("RenderTransform.(TranslateTransform.X)"));
                    break;


                case WalkDirection.UP:
                    retVal.By = -scene.tileHeight;
                    Storyboard.SetTargetProperty(retVal, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
                    break;

            }
            retVal.Duration = TimeSpan.FromSeconds(1 / walkSpped);
            retVal.FillBehavior = FillBehavior.HoldEnd;
            return retVal;

        }
    }
}
