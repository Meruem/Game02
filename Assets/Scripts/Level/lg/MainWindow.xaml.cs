using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assets.Scripts;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BitmapImage _freeBitmap = new BitmapImage(new Uri("pack://application:,,,/FreeVerySmall.png", UriKind.Absolute));
        private readonly BitmapImage _wallBitmap = new BitmapImage(new Uri("pack://application:,,,/WallVerySmall.png", UriKind.Absolute));
        private readonly BitmapImage _exitBitmap = new BitmapImage(new Uri("pack://application:,,,/VerySmallRoomExit.png", UriKind.Absolute));

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Generate(int width, int height, float ratio)
        {
            var mapGenerator = new MapGenerator(width, height, ratio, RoomManager.Instance, DefaultCheckBox.IsChecked.GetValueOrDefault(false));
            var map = mapGenerator.GenerateMap();
            var bitmapWidth = _freeBitmap.Width;
            var bitmapHeight = _freeBitmap.Height;

            for (var i = 0; i < mapGenerator.MapWidth; i++)
            {
                for (var j = 0; j < mapGenerator.MapHeight; j++)
                {
                    Image img;
                    switch (map[i, j])
                    {
                        case MapGenerator.TileType.Room: img = new Image { Source = _freeBitmap, Width = bitmapWidth, Height = bitmapHeight }; break;
                        case MapGenerator.TileType.Wall: img = new Image { Source = _wallBitmap, Width = bitmapWidth, Height = bitmapHeight }; break;
                        case MapGenerator.TileType.Exit: img = new Image { Source = _exitBitmap, Width = bitmapWidth, Height = bitmapHeight }; break;
                        default: img = new Image { Source = _wallBitmap, Width = bitmapWidth, Height = bitmapHeight }; break;
                    }

                    Canvas.SetLeft(img, 10 + bitmapWidth * j);
                    Canvas.SetTop(img, 10 + bitmapHeight * i);
                    GameCanvas.Children.Add(img);
                }
            }

            GameCanvas.Width = mapGenerator.MapWidth * bitmapWidth + 50;
            GameCanvas.Height = mapGenerator.MapHeight * bitmapHeight + 50;
        }

        private void OnDesignerOpen(object sender, RoutedEventArgs e)
        {
            var w = new Designer();
            w.Show();
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            GameCanvas.Children.Clear();
            Generate(int.Parse(WidthTextBox.Text), int.Parse(HeightTextBox.Text), float.Parse(RatioTextBox.Text));
        }

        private void OnRoomManagerOpen(object sender, RoutedEventArgs e)
        {
            var w = new RoomManagerWindow();
            w.Show();
        }
    }
}
