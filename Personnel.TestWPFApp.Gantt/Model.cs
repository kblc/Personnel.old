using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.TestWPFApp.Gantt
{
    public class Item
    {
        public DateTime Begin { get; set; }
        public int Days { get; set; }
    }

    public class Model
    {
        public Model()
        {
            From = new DateTime(2016, 1, 1);
            To = new DateTime(2017, 1, 1);
            for(int i = 0; i < 12; i++)
            {
                items.Add(new Item() { Begin = new DateTime(2016, i + 1, 1), Days = 20 - i });
            }
        }

        private readonly ObservableCollection<Item> items = new ObservableCollection<Item>();
        public ObservableCollection<Item> Items { get { return items; } }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
