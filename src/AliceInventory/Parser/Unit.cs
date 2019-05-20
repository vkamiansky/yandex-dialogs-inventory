using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    public class Unit
    {
        public string UnitName { get; set; }

        public Unit(string unitName)
        {
            UnitName = unitName;
        }
        //not sure, perhaps this method implements some part of parser analysis funcion TryParse()
        public List<string> GetSynonyms()
        {
            return new List<string>();
        }

        public override string ToString()
        {
            return UnitName;
        }
       
    }
}
