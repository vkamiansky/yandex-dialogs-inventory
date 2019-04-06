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

            while (!canExit) {
                var input = Console.ReadLine();
                //Console.WriteLine($"Вы ввели: {input}");
                var response = session.ProcessInput(input);
                Console.WriteLine($"[VOICE] {response.VoiceResponse}");
                Console.WriteLine(response.TextResponse);
                canExit = (input == "quit" || input == "exit" || input == "q");
            }
        }
    }
}
