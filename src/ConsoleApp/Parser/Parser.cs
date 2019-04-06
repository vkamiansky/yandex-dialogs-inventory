using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp {
    public class Parser {

        private char[] separators={' ',',','.'};

        public ParserResponse TryParse(string input) 
        {
            Unit currentUnit=GetDefaultUnit();
            int amount=1;
            string item="";
             try
             {
                 string lastNonSubectWord="";

                 var  words=input.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                 var firstWord=words?.First();
                 if(!string.IsNullOrWhiteSpace(firstWord))
                 {
                    if(firstWord.IsNumber() && words.Length>1)
                    {
                        //если не указан предмет - это хрень, игнорируем

                        //указано количество.
                        int.TryParse(firstWord,out amount);

                        //если дальше указана единциа измерения                        
                        var secondWord=words[1];
                        if(secondWord.IsUnit())
                        {
                            currentUnit=GetUnit(secondWord);
                            lastNonSubectWord=secondWord;
                        }
                        else
                        {
                            lastNonSubectWord=firstWord;
                        }                        
                    }
                    else if(firstWord.IsUnit())
                    {
                        currentUnit=GetUnit(firstWord);
                        lastNonSubectWord=firstWord;
                    }
                 }

                 if(!string.IsNullOrWhiteSpace(lastNonSubectWord))
                 {
                     //находим положение последнего слова
                     var lastNonSubjStart=input.IndexOf(lastNonSubectWord);

                     //находим где заканчивается последнее слово
                     var lastNonSubjEnd=lastNonSubjStart+lastNonSubectWord.Length;

                     //берём остаток
                     //убираем разделители и пробелы в начале
                     item=input.Substring(lastNonSubjEnd);

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
                ItemString = item.TrimStart(separators).TrimStart(' '),
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