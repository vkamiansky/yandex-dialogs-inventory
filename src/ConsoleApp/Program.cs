using System;
using QuickType;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var jsonString="{\"responseId\": \"f46297bb-ca55-4efa-8a0d-6d7ad59f2787\","+
            "\"queryResult\": {\"queryText\": \"Привет привет\",\"action\": \"google\","+
        "\"parameters\": {},\"allRequiredParamsPresent\": true, \"fulfillmentMessages\": ["+
        "{\"text\": {\"text\": [\"\"]}}],\"intent\": {"+
        "\"name\": \"projects/inventory-6d37b/agent/intents/95d5c853-aab6-4310-a12b-2801b4bbde65\","+
        "\"displayName\": \"hello\"},\"intentDetectionConfidence\": 0.75,"+
        "\"languageCode\": \"ru\"},\"originalDetectIntentRequest\": {"+
        "\"payload\": {}},"+
        "\"session\": \"projects/inventory-6d37b/agent/sessions/3dd0a0da-ae9d-dd8b-8948-c93b9c0401e4\"}";

            
            var req=GoogleRequest.FromJson(jsonString);

            var canExit = false;
            var session = new UserSession();

           Console.WriteLine("Учёт-Бот на месте, дайте команду");
           session.ProcessInput("добавить таблетка аспирин");
           session.ProcessInput("add 3 таблетки аспирина");
           session.ProcessInput("add 5 ампул адреналина");
           session.ProcessInput("add 50 миллиграммов глюкозы");
           session.ProcessInput("add грамм глюкозы");
           session.ProcessInput("add 3 грамма глюкозы");
           session.ProcessInput("add ампула адреналина");
           session.ProcessInput("добавить 3 ампулы адреналина");
           session.ProcessInput("add 500 миллилитров физраствора");
           session.ProcessInput("add 150 миллилитров физраствора");
           session.ProcessInput("add кот");
           session.ProcessInput("добавить кот");
           session.ProcessInput("add 2 кота");
           session.ProcessInput("добавить 3 машины");
           session.ProcessInput("add 15 машин");
           session.ProcessInput("add dog");
           session.ProcessInput("add 4 dogs");
           session.ProcessInput("add 100 граммов железа");
           session.ProcessInput("add 5 килограмм железа");
           session.ProcessInput("добавить 50 метров кабеля");
           session.ProcessInput("add 150 сантиметров кабеля");
           session.ProcessInput("add 5000 миллиметров кабеля");
           session.ProcessInput("добавить таблетка амоксиклава по 1000 мг");
           session.ProcessInput("add 3 таблетки амоксиклава по 1000 мг");
           session.ProcessInput("добавить автомат АК-47");
           session.ProcessInput("добавить 10 штук автомат АК-47");
           session.ProcessInput("добавить автомат АК-74");
           Console.WriteLine(session.ProcessInput("list").TextResponse);

            //while (!canExit) {;//Console.ReadLine();
                //Console.WriteLine($"Вы ввели: {input}");

              //  canExit = (input == "quit" || input == "exit" || input == "q");
            //}
        }
    }
}