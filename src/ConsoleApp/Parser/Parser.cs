using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp {
    public class Parser {

        private char[] separators={' ',',','.'};

        public ParserResponse TryParse(string input) 
        {
            Init();

            Unit currentUnit=GetDefaultUnit();
            int amount=1;
            string item;
             try
             {
                 string lastNonSubectWord="";

                 var  words=input.Split(separators);

                 var firstWord=words?.First();
                 if(!string.IsNullOrWhiteSpace(firstWord))
                 {
                    if(firstWord.IsNumber() && words.Length>1)
                    {
                        //если не указан предмет - это хрень, игнорируем

                        //указано количество.
                        int.TryParse(firstWord,amount);

                        //если дальше указана единциа измерения                        
                        var secondWord=words[1];
                        if(!string.IsNullOrWhiteSpace(secondWord))
                        {
                            Unit=GetUnit(secondWord);
                            lastNonSubectWord=secondWord;
                        }                        
                    }
                    else if(firstWord.IsUnit())
                    {
                        Unit=GetUnit(firstWord);
                        lastNonSubectWord=firstWord;
                    }
                 }
                 if(!string.IsNullOrWhiteSpace(lastNonSubectWord))
                 {

                 }
                 else
                 {
                     //не указано ни количество, ни единицы
                     item=input;
                 }

             }   
             catch(Exception ex)
             {

             }

            return new ParserResponse {
                ItemString = item,
                ItemCount = amount,
                Unit = currentUnit,
            };
        }

        private Unit GetUnit(string input)
        {


            return GetDefaultUnit();
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