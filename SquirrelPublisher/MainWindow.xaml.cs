using SquirrelPublisher.Xml;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace SquirrelPublisher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //void Button_Click(object sender, RoutedEventArgs e) {
        //    var ser = new XmlSerializer(typeof(Nuspec));
        //    using(FileStream stream = File.OpenRead(@"c:\Users\zinovyev.petr\Documents\DXVisualTestFixer\packages\squirrel.windows.1.8.0\tools\DXVisualTestFixer.2.0.7.nuspec")) {
        //        var res = ser.Deserialize(stream);
        //    }
        //}
    }
}
