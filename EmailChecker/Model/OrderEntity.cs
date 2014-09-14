using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailChecker.Model
{
    public class OrderEntity
    {
        public string AccountNumber { get; set; }
        public string Separator { get; set; }
        public string Number { get; set; }
        public string State { get; set; }
        public DateTime OpenDate { get; set; }
        public TimeSpan OpenTime { get; set; }
        public string Direction { get; set; }
        public string Size { get; set; }
        public string Symbol { get; set; }
        public string OpenPrice { get; set; }
        public string StopLoss { get; set; }
        public string Profit { get; set; }
        public string ClosedProfit { get; set; }
        public DateTime CloseDate { get; set; }
        public TimeSpan CloseTime { get; set; }
        public string Swap { get; set; }
        public string Commission { get; set; }
        public string FinalProfit { get; set; }
    }
}
