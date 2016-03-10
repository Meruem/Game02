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
using System.Windows.Shapes;

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for RoomManagerWindow.xaml
    /// </summary>
    public partial class RoomManagerWindow : Window
    {
        public RoomManagerWindow()
        {
            InitializeComponent();
        }

        private void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFolder(PathTextBox.Text);
            RoomsListBox.ItemsSource = RoomManager.Instance.GetRooms().Select(r => r.RoomName).ToList();
        }

        private void OnOpenFiles(object sender, RoutedEventArgs e)
        {
            RoomManager.Instance.OpenFiles();
            RoomsListBox.ItemsSource = RoomManager.Instance.GetRooms().Select(r => r.RoomName).ToList();
        }
    }
}
