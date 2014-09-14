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
using EmailChecker.ViewModel;

namespace EmailChecker
{
    /// <summary>
    /// Interaction logic for EmailServerSettings.xaml
    /// </summary>
    public partial class EmailServerSettings : UserControl
    {
        public EmailServerSettingViewModel EmailServerSettingVM { get; set; }
        public EmailServerSettings()
        {
            EmailServerSettingVM = new EmailServerSettingViewModel();
            this.DataContext = EmailServerSettingVM;
            InitializeComponent();
        }
    }
}
