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

        public static AliceResponse MakeAliceResponse(AliceRequest request,ProcessingResult answer)
        {
            AliceResponse response = new AliceResponse();

            //set buttons
            response.Response.Buttons= new Button[] 
            {
                    new Button() {Title = "Помощь", Payload = "", Url = null, Hide = true},

                    new Button() {Title = "Like", Payload = "", Url = null, Hide = false},

                    new Button() {Title = "Dislike", Payload = "", Url = null, Hide = true},

                    new Button() {Title = "Что ты еще умеешь?", Payload = "", Url = null, Hide = true}

            };

            response.Response.Text="Вот что я могу: </n> Добавлять объекты </n> Удалять объекты </n> Отображать список объектов </n> ";

            //set flag to stop dialog
            if(answer.Result==InputProcessingResult.Parting)
            {
               response.Response.EndSession=true; 
            }
            else response.Response.EndSession=false;
            // set processingResult message to Alice
            switch(answer.Result)
            {
                case ProcessingResult.ShowGreeting: 
                    response.Response.Text="Привет.Давай создадим список вещей";
                    break;
                case ProcessingCommand.Added:
                    response.Response.Text="Добавила "+answer.Data.Name+" "+answer.Data.Count+" "+answer.Data.Unit;
                    break;
                case ProcessingResult.AddCanceled:
                    response.Response.Text="Отменила добавление";
                    break;
                case ProcessingResult.Deleted:
                    response.Response.Text="Удалила "+answer.Data.Name+" "+answer.Data.Count" "+answer.Data.Unit;
                    break;
                case ProcessingResult.DeleteCanceled:
                    response.Response.Text="Отменила удаление";
                    break;
                case ProcessingResult.EntryNotFoundError:
                    response.Response.Text="У вас ничего нет";
                    break;
                case ProcessingResult.ClearRequested:
                    response.Response.Text="Вы точно хотите удалить все предметы из инвентаря?";
                    break;
                case ProcessingResult.Cleared:
                    response.Response.Text="Теперь инвентарь пуст";
                    break;
                case ProcessingResult.ListRead:
                    response.Response.Text="Это все что у вас есть";
                    break;
                case ProcessingResult.ListEmpty:
                     response.Response.Text="Инвентарь пуст";
                    break;
                case ProcessingResult.MailSent:
                     response.Response.Text="Отправила на почту";
                    break;
                case ProcessingResult.Error:
                     response.Response.Text="Ошибка";
                    break;
                case ProcessingResult.ShowParting:
                    response.Response.Text="До новых встреч";
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
