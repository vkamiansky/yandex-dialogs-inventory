using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp {
    public class Parser {

        public static char[] Separators={' ',',','.'};

        private static List<Unit> unitsList;

        private static List<string> itemsCache=new List<string>();

        private static void InitUnitsList()
        {
            if(unitsList==null)
            {
                try
                {
                    unitsList=new List<Unit>();
                    unitsList.Add(GetDefaultUnit());

                    var kg=new Unit(){
                        CommonName="грамм",
                        ShortName="г",
                        Multiplier=0,
                        AlternativeNames=new List<string>{"граммов", "грамм", "грамма"}};
                    unitsList.Add(kg);
                    unitsList.Add(new Unit(){
                        CommonName="килограмм",
                        ShortName="кг",
                        Multiplier=1000,
                        MainUnit=kg,
                        AlternativeNames=new List<string>{"килограммов", "килограмм", "килограмма"}});
                    unitsList.Add(new Unit(){
                        CommonName="миллиграмм",
                        ShortName="мг",
                        Multiplier=0.001,
                        MainUnit=kg,
                        AlternativeNames=new List<string>{"миллиграммов", "миллиграмм", "миллиграмма"}});
                    var m=new Unit(){
                        CommonName="метр",
                        ShortName="м",
                        AlternativeNames=new List<string>{"метра", "метров"}};
                    unitsList.Add(m);
                    unitsList.Add(new Unit(){
                        CommonName="сантиметр",
                        ShortName="см",
                        Multiplier=0.01,
                       MainUnit=m,
                        AlternativeNames=new List<string>{"сантиметра", "сантиметров"}});
                    unitsList.Add(new Unit(){
                        CommonName="миллиметр",
                        ShortName="мм",
                        Multiplier=0.001,
                        MainUnit=m,
                        AlternativeNames=new List<string>{"миллиметра", "миллиметров"}});
                    var litr=new Unit(){
                        CommonName="литр",
                        ShortName="л",
                        AlternativeNames=new List<string>{"литра", "литров", "литр"}};
                    unitsList.Add(new Unit(){
                        CommonName="миллилитр",
                        ShortName="мл",
                        Multiplier=0.001,
                        MainUnit=litr,
                        AlternativeNames=new List<string>{"миллилитра", "миллилитров"}});
                    unitsList.Add(new Unit(){
                        CommonName="таблетка",
                        ShortName="табл",
                        Multiplier=1,
                        MainUnit=null,
                        AlternativeNames=new List<string>{"таблетки", "таблеток"}});
                    unitsList.Add(new Unit(){
                        CommonName="ампула",
                        ShortName="ампул",
                        Multiplier=1,
                        MainUnit=null,
                        AlternativeNames=new List<string>{"ампул", "ампулы"}});

                }
                catch(Exception ex)
                {

                }
            }
        }

        public ParserResponse TryParse(string input) 
        {
            Unit currentUnit=GetDefaultUnit();

            double amount=1.0;
            string item="";
             try
             {
                 string lastNonSubectWord="";

                 var  words=input.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                 var firstWord=words?.First();
                 if(!string.IsNullOrWhiteSpace(firstWord))
                 {
                    if(GetNumberFromString(firstWord) > 0 && words.Length>1)
                    {
                        //если не указан предмет - это хрень, игнорируем

                        //указано количество.
                        double.TryParse(firstWord,out amount);

                        //если дальше указана единциа измерения                        
                        var secondWord=words[1];
                        var unit=GetUnitFromString(secondWord);
                        if(unit!=null)
                        {
                            currentUnit=unit;
                            lastNonSubectWord = secondWord;                            
                        }
                        else
                        {
                            lastNonSubectWord=firstWord;
                        }                        
                    }
                    else 
                    {
                        var unit=GetUnitFromString(firstWord);
                        if(unit!=null)
                        {
                            currentUnit = unit;
                            lastNonSubectWord = firstWord;
                        }
                    }
                 }

 
                if(currentUnit.Multiplier != 0 &&
                   currentUnit.Multiplier != 1 &&
                   currentUnit.MainUnit != null)
                    {
                        amount = amount * currentUnit.Multiplier;
                        currentUnit = currentUnit.MainUnit;
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
                ItemString = GetItemFromString(item),
                ItemCount = amount,
                Unit = currentUnit,                
            };
        }

        private string GetItemByMultipleForm(string item, char ending)
        {
            var tempItem=item.TrimEnd(ending);
            if(itemsCache.Contains(tempItem))
            {
                return tempItem;
            }
            else
            {
                itemsCache.Add(item);
                return item;
            }
        }

        private string GetItembySingleForm(string item)
        {
            if(itemsCache.Contains(item))
            {
                return item;
            }
            else if(itemsCache.Contains($"{item}s"))
            {
                return $"{item}s";
            }
            else if(itemsCache.Contains($"{item}ы"))
            {
                return $"{item}ы";
            }
            else if(itemsCache.Contains($"{item}и"))
            {
                return $"{item}и";
            }
            else if(itemsCache.Contains($"{item}ов"))
            {
                return $"{item}ов";
            }
            else if(itemsCache.Contains($"{item}ек"))
            {
                return $"{item}ек";
            }
            else
            {
                itemsCache.Add(item);
                return item;
            }
        }

        private string GetItemFromString(string input)
        {
            try
            {
                var item=input.Trim(Separators).Trim(' ');
                if(itemsCache.Contains(item))
                {
                    return item;
                }
                else if(item.EndsWith("s"))
                {
                    return GetItemByMultipleForm(item, 's');
                }
                else if(item.EndsWith("ы"))
                {
                    return GetItemByMultipleForm(item, 'ы');
                }
                else if(item.EndsWith("и"))
                {
                    return GetItemByMultipleForm(item, 'и');
                }
                else if(item.EndsWith("а"))
                {
                    return GetItemByMultipleForm(item, 'а');
                }
                else 
                {
                    return GetItembySingleForm(item);
                }
            }
            catch(Exception ex)
            {

            }
            return input;
        }

        private Unit GetUnit(string input)
        {
            var unit=GetUnitFromString(input);
            if(unit!=null) return unit;

            return GetDefaultUnit();
        }

        private Unit GetUnitFromString(string input)
        {
            InitUnitsList();

            if(unitsList.Any(unit=>unit.Matches(input)))
            {                
                return unitsList.First(unit=>unit.Matches(input));
            }
            return null;
        }

        public static Unit GetDefaultUnit()
        { 
            var newUnit=new Unit();
            newUnit.CommonName="штука";
            newUnit.ShortName="шт";
            newUnit.AlternativeNames=new List<string>();
            newUnit.AlternativeNames.Add("штук");
            newUnit.AlternativeNames.Add("штуки");
            return newUnit;
        }

        private double GetNumberFromString(string input)
        {
            double d=0;
            double.TryParse(input, out d);
            return d;
        }

    }
}