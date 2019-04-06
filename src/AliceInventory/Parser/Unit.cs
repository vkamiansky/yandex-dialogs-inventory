using System.Collections.Generic;

namespace ConsoleApp {
    public class Unit {
        public string CommonName { get; set; }

        public string ShortName { get; set; }

        public IEnumerable<string> AlternativeNames { get; set; }

        public override string ToString() {
            return ShortName;
        }
    }
}