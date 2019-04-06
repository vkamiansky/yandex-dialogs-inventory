using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var canExit = false;
            var session = new UserSession();

           Console.WriteLine("Учёт-Бот на месте, дайте команду");
           session.ProcessInput("add 3 cats");
           session.ProcessInput("add 2 cats");
           session.ProcessInput("добавить 3 cats");
           session.ProcessInput("add 3 tiger");
           session.ProcessInput("add tiger");
           Console.WriteLine(session.ProcessInput("list").TextResponse);

            //while (!canExit) {;//Console.ReadLine();
                //Console.WriteLine($"Вы ввели: {input}");

              //  canExit = (input == "quit" || input == "exit" || input == "q");
            //}
        }
    }
}
