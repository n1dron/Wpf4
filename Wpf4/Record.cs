using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wpf4
{
    public class Record
    {
        public int id { get; set; } = 0;
        public string Name { get; set; }
        public string Type { get; set; }
        public bool islncome { get; set; }
        private double _Money;
        public double Money
        {
            get { return _Money; }
            set { _Money = value; islncome = value >= 0; }
        }
        public DateTime Date;
        public Record(int id, string Name, string Type, double Money, DateTime Date)
        {
            this.id = id;
            this.Name = Name;
            this.Type = Type;
            this.Money = Money;
            this.Date = Date;
        }
    }
}
