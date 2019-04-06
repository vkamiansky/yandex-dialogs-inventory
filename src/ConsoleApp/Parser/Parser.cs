using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp {
    public class Parser {

        private char[] separators={' ',',','.'};
        private static List<string> unitnames=null;

        private static void Init()
        {
            if(unitnames==null)
            {
                unitnames=new List<string>();
                unitnames.Add("Штука");
            }
        }

        public ParserResponse TryParse(string input) {

             try
             {
                 Init();
                 var  words=input.Split(separators);

                 var first=words.First();
                 if(first!=null )
                 {
                    if(first.IsNumber())
                    {
                        //указано количество
                    }
                    else if(first.IsUnit())
                    {

                    }
                 }

             }   
             catch(Exception ex)
             {

             }

            return new ParserResponse {
                ItemString = "Какой-то предмет",
                ItemCount = 1,
                Unit =  GetDefaultUnit(),
            };
        }
        public static Unit GetDefaultUnit()
        { 
            return new Unit() {
                CommonName = "штука",
                ShortName = "шт.",
                AlternativeNames = new string[0],
            };
        }

    }
}