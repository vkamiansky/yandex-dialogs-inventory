using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    //this class implements the basic entry in the list of user backpack
    public class Entry
    {
        public string ItemName { get; set; }
        public double ItemCount { get; set; }
        public Unit Unit { get; set; }

        public Entry()
        {
            
        }
        public Entry(string name, double count, Unit unit)
        {
            ItemName = name;
            ItemCount = count;
            Unit = unit;
        }
    }
}