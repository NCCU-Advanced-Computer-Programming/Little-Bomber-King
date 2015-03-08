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
    class ShelterLayer : Grid
    {
        public Shelter[,] shelters { get; private set; }
        public Dictionary<Shelter, Int32Point> shelterPositions { get; private set; }
        public ShelterLayer(int rows, int columns, double rowHeight, double columnWidth)
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
            this.shelters = new Shelter[rows, columns];
            this.shelterPositions = new Dictionary<Shelter, Int32Point>();
        }
        
        public void addShelter(Shelter shelter,int row,int column)
        {
           

            shelter.SetValue(Grid.ColumnProperty, column);
            shelter.SetValue(Grid.RowProperty, row);
            shelters[row, column] = shelter;
            shelterPositions[shelter] = new Int32Point(column, row);
            Children.Add(shelter);
            
            

        }
        public void destroy(Shelter shelter)
        {
            Int32Point pos = shelterPositions[shelter];
            if (pos == null)
            {
                return;
            }
            shelters[pos.y, pos.x] = null;
            shelterPositions[shelter] = null;
            Children.Remove(shelter);
        }
        public void destroyAt(int row, int column)
        {
            destroy(shelters[row,column]);
        }
        public IList<Shelter> shelterWithinGrids(int x, int y, int width, int height)
        {
            IList<Shelter> retVal = new List<Shelter>();
            for (int i = y; i < y + height; i++)
            {
                for (int j = x; j < x + width; j++)
                {
                    if (shelters[i, j] != null)
                    {
                        retVal.Add(shelters[i, j]);
                    }
                }
            }

            return retVal;
        }
        public Shelter shelterOnGrid(int x, int y)
        {
            IList<Shelter> shelterList = shelterWithinGrids(x, y, 1, 1);
            if (shelterList.Count > 0)
            {
                return shelterList[0];
            }
            return null;

        }
    }
}
