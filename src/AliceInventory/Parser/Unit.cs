using System.Collections.Generic;
using System;
using System.Linq;

namespace ConsoleApp {
    public class Unit {
        public string CommonName { get; set; }

        public string ShortName { get; set; }

        public List<string> AlternativeNames { get; set; }

        public double Multiplier {get;set;}
        public Unit MainUnit{get;set;}

        public override string ToString() {
            return ShortName;
        }

        public bool Matches(string input)
        {
            var clearedInput=input!=null?input.Trim(Parser.Separators).Trim(' '):input;
            if(string.Equals(
                this.CommonName,
                clearedInput,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            if(string.Equals(
                this.ShortName,
                clearedInput,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            
            if(this.AlternativeNames!=null &&
                this.AlternativeNames.Any(alt=>
                    string.Equals(
                    alt,
                    clearedInput,
                    StringComparison.CurrentCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }
    }
}