using Bomb.Src.Weapon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bomb
{
	/// <summary>
	/// PlayerInfoPanel.xaml 的互動邏輯
	/// </summary>
	public partial class PlayerInfoPanel : UserControl
	{
		public PlayerInfoPanel()
		{
			this.InitializeComponent();
		}
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

                rectHp.Width = (bdrHp.ActualWidth- bdrHp.BorderThickness.Left-bdrHp.BorderThickness.Right) * ((double)hp / value);
                _maxHp = value;

            }
        }
        private int _hp;
        public int hp
        {
            get
            {
                return _hp;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                if (value > maxHp)
                {
                    value = maxHp;
                }
                rectHp.Width = (bdrHp.ActualWidth- bdrHp.BorderThickness.Left-bdrHp.BorderThickness.Right) * ((double)value / maxHp);
                _hp = value;
            }
        }
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
                lblSpeed.Text = value.ToString("F2");
                _speed = value;

            }
        }
        private int _power { get; set; }
        public int power
        {
            get
            {
                return _power;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                lblPower.Text = value.ToString();
                _power = value;
            }
        }
        private int _maxBomb;
        public int maxBomb
        {
            get
            {
                return _maxBomb;
            }
            set
            {

                if (value < 0)
                {
                    value = 0;
                }
                lblMaxBomb.Text = value.ToString() + " x ";
                _maxBomb = value;
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
                if (value < 0)
                {
                    value = 0;
                }
                _atk = value;
                lblAtk.Text = value.ToString();
            }
        }
        public ImageSource faceImgSrc
        {
            get
            {
                return this.imgFace.Source;
            }
            set
            {
                imgFace.Source = value;
            }
        }
		private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
            imgMaxBomb.Source = WBomb.bombSrc[0];
		}
	}
}