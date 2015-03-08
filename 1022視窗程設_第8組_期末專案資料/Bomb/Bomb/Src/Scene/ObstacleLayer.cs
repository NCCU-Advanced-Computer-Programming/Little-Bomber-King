using Bomb.Src.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bomb.Scene
{
    class ObstacleLayer : Grid
    {
        public Obstacle[,] obstacles;
        public ObstacleLayer(int rows,int columns,double rowHeight,double columnWidth)
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
            this.obstacles = new Obstacle[rows,columns];
            
        }

        public void addObstacle(Obstacle obstacle,int row,int column)
        {
            obstacle.SetValue(Grid.ColumnProperty, column);
            obstacle.SetValue(Grid.RowProperty, row);
            obstacles[row, column] = obstacle;
            Children.Add(obstacle);
        }
    }
}
