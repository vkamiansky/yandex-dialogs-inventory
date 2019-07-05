using System;
using AliceInventory.Controllers;
using AliceInventory.Controllers.AliceResponseRender;
using AliceInventory.Logic;
using AliceInventory.Logic.Core.Errors;
using Xunit;
using Xunit.Abstractions;

namespace AliceInventory.UnitTests
{
    public class AliceResponseRenderTest
    {
        private readonly ITestOutputHelper _output;

        public AliceResponseRenderTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private static readonly Entry entry = new Entry()
        {
            Name = "камни",
            Quantity = 4,
            UnitOfMeasure = UnitOfMeasure.Unit
        };
        private static readonly Entry[] entries = new[]
        {
            new Entry(){ Name = "яблоки", Quantity = 10, UnitOfMeasure = UnitOfMeasure.Kg },
            new Entry(){ Name = "арбузы", Quantity = 10, UnitOfMeasure = UnitOfMeasure.L },
            new Entry(){ Name = "молоко", Quantity = 10, UnitOfMeasure = UnitOfMeasure.Unit },
        };

        private static readonly Session sessionExample = new Session()
        {
            UserId = "123",
            SessionId = "321",
            MessageId = 1
        };

        // [Theory]
        // [InlineData(ProcessingResultType.Added)]
        // [InlineData(ProcessingResultType.AddCanceled)]
        // [InlineData(ProcessingResultType.Deleted)]
        // [InlineData(ProcessingResultType.ClearRequested)]
        // [InlineData(ProcessingResultType.Cleared)]
        // [InlineData(ProcessingResultType.MailSent)]
        // [InlineData(ProcessingResultType.Error)]
        // [InlineData(ProcessingResultType.ListRead)]
        // [InlineData(ProcessingResultType.GreetingRequested)]
        // [InlineData(ProcessingResultType.HelpRequested)]
        // [InlineData(ProcessingResultType.ExitRequested)]
        // public void Rendering(ProcessingResultType type)
        // {
        //     ProcessingResult result = null;

        //     switch (type)
        //     {
        //         case ProcessingResultType.Added:
        //         case ProcessingResultType.AddCanceled:
        //         case ProcessingResultType.Deleted:
        //             result = new ProcessingResult(type, entry);
        //             break;
        //         case ProcessingResultType.ListRead:
        //             result = new ProcessingResult(type, entries);
        //             break;
        //         case ProcessingResultType.MailSent:
        //             result = new ProcessingResult(type, "somemail@mail.ru");
        //             break;
        //     }

        //     var jsonResponse = AliceResponseRendererHelper.CreateAliceResponse(result, sessionExample);

        //     var response = jsonResponse.Response;
        //     _output.WriteLine($"Text: {response.Text}\nTts: {response.Tts}\nButtons: {response.Buttons.Length}\nEndSession: {response.EndSession}");

        //     var session = jsonResponse.Session;
        //     Assert.Equal(sessionExample.UserId, session.UserId);
        //     Assert.Equal(sessionExample.MessageId, session.MessageId);
        //     Assert.Equal(sessionExample.SessionId, session.SessionId);


        //     Assert.NotEmpty(response.Text);
        // }

        [Fact]
        public void EntryNotFoundInDatabaseErrorRendering()
        {
            CheckAllResponseRenderings(
                new ProcessingResult(new EntryNotFoundInDatabaseError("яблоки", Logic.UnitOfMeasure.Kg)),
                new[]
                {
                    "Вы не добавляли яблоки в кг",
                    "У вас яблоки не храниться в кг",
                    "Не нашла яблоки в кг в вашем списке",
                    "Но в списке нет яблоки в кг",
                    "В списке нет яблоки в кг",
                    "Я не смогла найти ни одного кг яблоки в отчёте",
                });
        }

        
        [Fact]
        public void NotEnoughEntryToDeleteErrorRendering()
        {
            CheckAllResponseRenderings(
                new ProcessingResult(new NotEnoughEntryToDeleteError("камни", 4, 2)),
                new[]
                {
                    "Не могу удалить 4 камни, в список добавлено только 2",
                    "У вас только 2 камни"
                });
        }

        public void CheckAllResponseRenderings(ProcessingResult result, string[] expectedTexts)
        {
            bool testedAllTexts = false;
            int textNumber = 1;
            while (!testedAllTexts)
            {
                var jsonResponse = AliceResponseRendererHelper.CreateAliceResponse(
                    result,
                    sessionExample,
                    x =>
                    {
                        if (textNumber == x)
                            testedAllTexts = true;
                        return textNumber;
                    });

                var response = jsonResponse.Response;
                _output.WriteLine($"Text: {response.Text}\nTts: {response.Tts}\nButtons: {response.Buttons.Length}\nEndSession: {response.EndSession}\n");

                var session = jsonResponse.Session;
                Assert.Equal(sessionExample.UserId, session.UserId);
                Assert.Equal(sessionExample.MessageId, session.MessageId);
                Assert.Equal(sessionExample.SessionId, session.SessionId);

                Assert.Equal(expectedTexts[textNumber - 1], response.Text);
                textNumber++;
            }
        }
    }
}
