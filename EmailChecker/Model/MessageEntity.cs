using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EmailChecker.Model
{
    public enum MessageStatus
    {
        New,
        Comapred,
        Acknowledged,
        Alert,
        Different
    }

    public  class MessageEntity :INotifyPropertyChanged
    {
        public int Number { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }

        private MessageStatus _status;

        public MessageStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public OrderEntity OrderDetails { get; set; }

        public DispatcherTimer RecordCompareTimer { get; set; }
        
        public bool IsCompared { get; set; }

        public MessageEntity()
        {
            RecordCompareTimer = new DispatcherTimer();
            int acknowledgementTime = int.Parse(Properties.Server.Default.RecordCompareTime);
            RecordCompareTimer.Interval = new TimeSpan(0, 0, 0, 0, acknowledgementTime);
            RecordCompareTimer.Tick +=RecordCompareTimer_Tick;

            Status = MessageStatus.New;
        }
        private void RecordCompareTimer_Tick(object sender, EventArgs e)
        {
           // MessageBox.Show("Record Compare timer");
            Status = MessageStatus.Alert;
            MessageBox.Show("Account 1234 opened an order.(show details) No message from the other account.","Email Checke",MessageBoxButton.OK);
            Status = MessageStatus.Acknowledged;
            RecordCompareTimer.Stop();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
