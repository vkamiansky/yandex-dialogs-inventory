using System;
using AliceInventory;
using AliceInventory.Controllers;

namespace AliceInventory.Logic
{
    public static class Converter
    {
        public static Logic.Entry ToLogic(this Data.Entry entry)
        {
            if (entry == null)
                return null;

            return new Logic.Entry
            {
                Name = entry.Name,
                Count = entry.Count,
                Unit = entry.Unit.ToLogic(),
            };
        }

        public static Data.Entry ToData(this Logic.Entry entry)
        {
            if (entry == null)
                return null;

            return new Data.Entry
            {
                Name = entry.Name,
                Count = entry.Count,
                Unit = entry.Unit.ToData(),
            };
        }

        public static Logic.UnitOfMeasure ToLogic(this Data.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Data.UnitOfMeasure.Unit:
                    return Logic.UnitOfMeasure.Unit;
                case Data.UnitOfMeasure.Kg:
                    return Logic.UnitOfMeasure.Kg;
                case Data.UnitOfMeasure.L:
                    return Logic.UnitOfMeasure.L;
                default:
                    return Logic.UnitOfMeasure.Unit;
            }
        }
        public static Data.UnitOfMeasure ToData(this Logic.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Logic.UnitOfMeasure.Unit:
                    return Data.UnitOfMeasure.Unit;
                case Logic.UnitOfMeasure.Kg:
                    return Data.UnitOfMeasure.Kg;
                case Logic.UnitOfMeasure.L:
                    return Data.UnitOfMeasure.L;
                default:
                    return Data.UnitOfMeasure.Unit;
            }
        }

        public static AliceResponse MakeAliceResponse(AliceRequest request, ProcessingResult answer)
        {
            var response = new AliceResponse();
            response.Response = new Response();
            response.Session = new Session();
            Entry entry = answer.Data as Entry;
            //set buttons
            /* response.Response.Buttons = new Button[]
            {
                    new Button() {Title = "Помощь", Payload = "", Url = null, Hide = true},

                    new Button() {Title = "Like", Payload = "", Url = null, Hide = false},

                    new Button() {Title = "Dislike", Payload = "", Url = null, Hide = true},

                    new Button() {Title = "Что ты еще умеешь?", Payload = "", Url = null, Hide = true}

            };*/

            //set flag to stop dialog
            if (answer.Result == InputProcessingResult.ExitRequested)
            {
                //response.Response.EndSession = true;
            }
            else response.Response.EndSession = false;

            if (request.Session.New)
            {
                answer.Result = InputProcessingResult.GreetingRequested;
            }

            // set processingResult message to Alice
            switch (answer.Result)
            {
                case InputProcessingResult.GreetingRequested:
                    response.Response.Text = "Привет.Давай создадим список вещей";
                    break;
                case InputProcessingResult.Added:
                    response.Response.Text = "Добавила "; //+ entry.Name + " " + entry.Count + " " + entry.Unit;
                    break;
                case InputProcessingResult.AddCanceled:
                    response.Response.Text = "Отменила добавление";
                    break;
                case InputProcessingResult.Deleted:
                    response.Response.Text = "Удалила ";//+ entry.Name + " " + entry.Count + " " + entry.Unit;
                    break;
                case InputProcessingResult.ClearRequested:
                    response.Response.Text = "Вы точно хотите удалить все предметы из инвентаря?";
                    break;
                case InputProcessingResult.Cleared:
                    response.Response.Text = "Теперь инвентарь пуст";
                    break;
                case InputProcessingResult.ListRead:
                    response.Response.Text = "Это все что у вас есть";
                    //а здесь где-то список вещичек
                    break;
                case InputProcessingResult.MailSent:
                    response.Response.Text = "Отправила на почту";
                    break;
                case InputProcessingResult.HelpRequested:
                    response.Response.Text = "А вот и помощь";
                    // здесь видимо добавить возможность помощи возврата
                    break;
                case InputProcessingResult.Error:
                    response.Response.Text = "Неизвестная ошибка";
                    break;
                case InputProcessingResult.ExitRequested:
                    response.Response.Text = "Досвидос";
                    break;
            }


            //response.Response.Tts
            response.Session.MessageId = request.Session.MessageId;
            response.Session.SessionId = request.Session.SessionId;
            response.Session.UserId = request.Session.UserId;

            response.Version = request.Version;
            return response;
        }
    }
}
