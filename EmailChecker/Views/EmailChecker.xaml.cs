using System;
using System.Collections.Generic;
using System.Data;
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
using EmailChecker.Model;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace EmailChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        void CheckRecord_Click(object sender, RoutedEventArgs e)
        {
            //http://stackoverflow.com/questions/15291864/how-to-get-current-element-in-wpf-datagrid-and-how-to-do-something-with-it

            var selectedMessage = ((FrameworkElement)sender).DataContext as MessageEntity;
            selectedMessage.Status = MessageStatus.Acknowledged;
        }
    }
}
