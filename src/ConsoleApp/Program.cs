using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var canExit = false;


            Console.WriteLine("Учёт-Бот на месте, дайте команду");
            ProcessInput("add 3 cats");
           ProcessInput("add 2 cats");
           ProcessInput("добавить 3 cats");
           ProcessInput("add 3 tiger");
           ProcessInput("add tiger");
           ProcessInput("list");

            //while (!canExit) {;//Console.ReadLine();
                //Console.WriteLine($"Вы ввели: {input}");

              //  canExit = (input == "quit" || input == "exit" || input == "q");
            //}
        }

        static void ProcessInput(string input)
        {
            var session = new UserSession();
            var response = session.ProcessInput(input);
            if(response==null)
            { 
                Console.WriteLine("Unable to process");
            }
            else
            {
                Console.WriteLine($"[VOICE] {response?.VoiceResponse}");
                Console.WriteLine(response?.TextResponse);
            }
        }
    }
}
