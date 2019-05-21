using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace ConsoleApp
{
    using static ChatResponse;
    class Parser
    {
        //public ChatResponse TryParse(string userText)
        //{
            //some analysis of userText
            //...
            // return new ChatResponse(ChatResponse.UserAction.Add, "someItem", 10, new Unit("kg"));

             /* public ParsedUserQuery TryParse(string userText)
        {
            var result = new ParsedUserQuery ( 
                Actions.Add,
                1, 
                "apple", 
                new Unit() { ShortName = "items" } );
            return result;
        }*/
        //}

        public ChatResponse TryParse (string userInput)
        {
            if(userInput.Contains("Выйти"))
            {
                // возвратить действие с выходом
            }
            if (userInput.Contains("Отмени") || userInput.Contains("Отменить"))
            {
                return new ChatResponse(UserAction.deleteLast,"",0.0, new Unit());//заглушка для отмены
            }
            if (userInput.Contains("Выведи все"))
            {
                return new ChatResponse(UserAction.readAll, "", 0.0, new Unit());
            }
            if (userInput.Contains("Очисти"))
            {
                return new ChatResponse(UserAction.clearAll, "", 0.0, new Unit());
            }
            if(userInput.Contains("Добавь") && userInput.Split(" ").Length == 4)
            {
                // разбиваем запрос на 4 подстроки
                string[] str = userInput.Split(' ');

                UserAction act = UserAction.Add;
                //получаем количество
                double count = 0.0;
                int icount = 0;
                if (!double.TryParse(str[1],out count)&&!int.TryParse(str[1],out icount))
                {
                    return null;
                }
                if (count == 0.0 || count < 0)
                    if (icount != 0 || icount > 0)
                    {
                        count = (double)icount;
                    }
                    else return null;
                //получаем единицы измерения ( без проверки на валидность)
                string unit = str[2];
                // получаем предмет
                string item = str[3];

                return new ChatResponse(act, item, count, new Unit());// подправить юнит
            }
            else
            {
                return null;
            }
        }

        // //this is for the future
        // private string ParseAction(string[] words)
        // {
        //     foreach (string stringwrite in words)
        //     {
        //         if (stringwrite.Contains(endofverbs))
        //         {
        //             if (stringwrite.Contains(addroots))
        //             {
        //                 chatResponse.userAction = Add;
        //             }
        //             else if (stringwrite.Contains(deleteroots))
        //             {
        //                 chatResponse.userAction = deleteLast;
        //             }
        //             else if (stringwrite.Contains(deleteallroots))
        //             {
        //                 chatResponse.userAction = clearAll;
        //             }
        //             else if (stringwrite.Contains(readallroots))
        //             {
        //                 chatResponse.userAction = readAll;
        //             }
        //         }
        //         else
        //         {
        //             chatResponse.userAction = Add;
        //         }
        //     }
        // }

        // //this is for the future
        // private double CountItem (string[] words)
        // {
        //     double count = 1;
        //     foreach (string countstring in words)
        //     try {
        //             Int32.TryParse(countstring, out count);
        //     }
        //         catch (Exception exc)
        //         {

        //         }
        //     return count; 
        // }
        // //this is for the future
        // private Unit UnitItem (string[] words)
        // {
        //     Unit unit = new Unit();

        //     return unit;
        // }

    }
}
