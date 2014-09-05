using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EmailChecker.ViewModel
{
    public class EmailCheckerViewModel : ViewModelBase
    {
        private EmailServerSettings serverSettings;

        public EmailServerSettings ServerSettings
        {
            get { return serverSettings; }
            set
            {
                serverSettings = value;
                NotifyPropertyChangedEvent("ServerSettings");
            }
        }

        public EmailCheckerViewModel()
        {
            ServerSettings = new EmailServerSettings();
        }
    }
}
