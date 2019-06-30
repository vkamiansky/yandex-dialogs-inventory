using System;
using System.Collections.Generic;
using System.Text;
using AliceInventory.Controllers;
using AliceInventory.Logic;
using AliceInventory.Logic.AliceResponseRender;
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

        private static readonly SingleEntry entry = new SingleEntry()
        {
            Name = "камни",
            Count = 4,
            Unit = UnitOfMeasure.Unit
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
    }
}
