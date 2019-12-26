using System;
using System.Collections.Generic;
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
            new Entry(){Name = "груши",Quantity = 10, UnitOfMeasure = UnitOfMeasure.Box},
            new Entry(){Name = "таблетки",Quantity = 10, UnitOfMeasure = UnitOfMeasure.Pack}
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
        public void NotEnoughEntryToDeleteErrorRendering()
        {
            var error = new NotEnoughEntryToDeleteError("камни", 4, 2);
            var expectedRenderings = new[]
            {
                "Не могу удалить 4 камни, в список добавлено только 2",
                "У вас только 2 камни"
            };

            CheckAllResponseRenderings(error, expectedRenderings);
        }


        [Fact]
        public void EntryNotFoundInDatabaseErrorRendering()
        {
            var error = new EntryNotFoundInDatabaseError("яблоки", Logic.UnitOfMeasure.Kg);
            var expectedRenderings = new[]
            {
                "Вы не добавляли яблоки в кг",
                "У вас яблоки не храниться в кг",
                "Не нашла яблоки в кг в вашем списке",
                "Но в списке нет яблоки в кг",
                "В списке нет яблоки в кг",
                "Я не смогла найти ни одного кг яблоки в отчёте",
            };

            CheckAllResponseRenderings(error, expectedRenderings);
        }

        private void CheckAllResponseRenderings(ProcessingResult result, IReadOnlyList<string> expectedRenderings)
        {
            for (var i = 0; i < expectedRenderings.Count; i++)
            {
                var textNumber = i;
                var jsonResponse = AliceResponseRendererHelper.CreateAliceResponse(result, sessionExample, x => textNumber);

                var response = jsonResponse.Response;
                _output.WriteLine($"Text: {response.Text}\n" +
                                  $"Tts: {response.Tts}\n" +
                                  $"Buttons: {response.Buttons.Length}\n" +
                                  $"EndSession: {response.EndSession}\n");

                var session = jsonResponse.Session;
                Assert.Equal(sessionExample.UserId, session.UserId);
                Assert.Equal(sessionExample.MessageId, session.MessageId);
                Assert.Equal(sessionExample.SessionId, session.SessionId);

                Assert.Equal(expectedRenderings[textNumber], response.Text);
            }
        }
    }
}
