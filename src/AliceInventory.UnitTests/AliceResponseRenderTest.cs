using System;
using AliceInventory.Controllers;
using AliceInventory.Controllers.AliceResponseRender;
using AliceInventory.Logic;
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
        private static readonly Entry[] entries = new []
        {
            new Entry("яблоки", 10, UnitOfMeasure.Kg),
            new Entry("арбузы", 10, UnitOfMeasure.L),
            new Entry("молоко", 10, UnitOfMeasure.Unit),
        };

        private static readonly Session sessionExample = new Session()
        {
            UserId = "123",
            SessionId = "321",
            MessageId = 1
        };

        [Theory]
        [InlineData(ProcessingResultType.Added)]
        [InlineData(ProcessingResultType.AddCanceled)]
        [InlineData(ProcessingResultType.Deleted)]
        [InlineData(ProcessingResultType.ClearRequested)]
        [InlineData(ProcessingResultType.Cleared)]
        [InlineData(ProcessingResultType.MailSent)]
        [InlineData(ProcessingResultType.Error)]
        [InlineData(ProcessingResultType.ListRead)]
        [InlineData(ProcessingResultType.GreetingRequested)]
        [InlineData(ProcessingResultType.HelpRequested)]
        [InlineData(ProcessingResultType.ExitRequested)]
        public void Rendering(ProcessingResultType type)
        {
            var result = new ProcessingResult() { Type = type };

            switch (type)
            {
                case ProcessingResultType.Added:
                case ProcessingResultType.AddCanceled:
                case ProcessingResultType.Deleted:
                    result.Data = entry;
                    break;
                case ProcessingResultType.ListRead:
                    result.Data = entries;
                    break;
                case ProcessingResultType.MailSent:
                    result.Data = "somemail@mail.ru";
                    break;
            }

            var jsonResponse = AliceResponseRendererHelper.CreateAliceResponse(result, sessionExample);

            var response = jsonResponse.Response;
            _output.WriteLine($"Text: {response.Text}\nTts: {response.Tts}\nButtons: {response.Buttons.Length}\nEndSession: {response.EndSession}");

            var session = jsonResponse.Session;
            Assert.Equal(sessionExample.UserId, session.UserId);
            Assert.Equal(sessionExample.MessageId, session.MessageId);
            Assert.Equal(sessionExample.SessionId, session.SessionId);


            Assert.NotEmpty(response.Text);
        }

        [Fact]
        public void ExceptionsOutput()
        {
            var entryNotFoundExceptionResult = new ProcessingResult()
            {
                Type = ProcessingResultType.Error,
                Error = new EntryNotFoundError("someId",
                    "яблоки")
            };

            var entryUnitNotFoundExceptionResult = new ProcessingResult()
            {
                Type = ProcessingResultType.Error,
                Error = new EntryUnitNotFoundError("someId",
                    new Data.Entry("камни"), Data.UnitOfMeasure.L)
            };

            var notEnoughEntryToDeleteExceptionResult = new ProcessingResult()
            {
                Type = ProcessingResultType.Error,
                Error = new NotEnoughEntryToDeleteError("someId",
                    4, 2, new Data.Entry("камни"))
            };

            foreach (var result in new[]
            {
                entryNotFoundExceptionResult,
                entryUnitNotFoundExceptionResult,
                notEnoughEntryToDeleteExceptionResult
            })
            {
                _output.WriteLine(result.Error.GetType().Name);
                var jsonResponse = AliceResponseRendererHelper.CreateAliceResponse(result, sessionExample);

                var response = jsonResponse.Response;
                _output.WriteLine($"Text: {response.Text}\nTts: {response.Tts}\nButtons: {response.Buttons.Length}\nEndSession: {response.EndSession}\n");

                var session = jsonResponse.Session;
                Assert.Equal(sessionExample.UserId, session.UserId);
                Assert.Equal(sessionExample.MessageId, session.MessageId);
                Assert.Equal(sessionExample.SessionId, session.SessionId);


                Assert.NotEmpty(response.Text);
            }
        }
    }
}
