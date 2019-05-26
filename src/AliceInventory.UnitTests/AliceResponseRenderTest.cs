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

        private static readonly Entry entry = new Entry()
        {
            Name = "камни",
            Count = 4,
            Unit = UnitOfMeasure.Unit
        };
        private static readonly Entry[] entries = new []
        {
            new Entry() {Name = "яблоки", Count = 10, Unit = UnitOfMeasure.Kg},
            new Entry() {Name = "арбузы", Count = 10, Unit = UnitOfMeasure.L},
            new Entry() {Name = "молоко", Count = 10, Unit = UnitOfMeasure.Unit},
        };

        private static readonly Session sessionExample = new Session()
        {
            UserId = "123",
            SessionId = "321",
            MessageId = 1
        };

        [Theory]
        [InlineData(InputProcessingResult.Added)]
        [InlineData(InputProcessingResult.AddCanceled)]
        [InlineData(InputProcessingResult.Deleted)]
        [InlineData(InputProcessingResult.ClearRequested)]
        [InlineData(InputProcessingResult.Cleared)]
        [InlineData(InputProcessingResult.MailSent)]
        [InlineData(InputProcessingResult.Error)]
        [InlineData(InputProcessingResult.ListRead)]
        [InlineData(InputProcessingResult.GreetingRequested)]
        [InlineData(InputProcessingResult.HelpRequested)]
        [InlineData(InputProcessingResult.ExitRequested)]
        public void Rendering(InputProcessingResult type)
        {
            var result = new ProcessingResult() { Result = type };

            switch (type)
            {
                case InputProcessingResult.Added:
                case InputProcessingResult.AddCanceled:
                case InputProcessingResult.Deleted:
                    result.Data = entry;
                    break;
                case InputProcessingResult.ListRead:
                    result.Data = entries;
                    break;
                case InputProcessingResult.MailSent:
                    result.Data = "somemail@mail.ru";
                    break;
            }

            var jsonResponse = AliceResponseRendererHelper.CreateResponse(result, sessionExample);
            
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
