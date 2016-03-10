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

namespace LevelGen
{
    /// <summary>
    /// Interaction logic for RoomDesigner.xaml
    /// </summary>
    public partial class RoomDesigner : UserControl
    {
        public RoomDesigner()
        {
            InitializeComponent();
            var rd = new RoomDesignerVM();
            rd.View = this;
            RoomDesignerVM.Instance = rd;
            DataContext = rd;
        }
    }
}
