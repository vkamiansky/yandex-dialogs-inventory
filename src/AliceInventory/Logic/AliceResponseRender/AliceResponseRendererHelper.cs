using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AliceInventory.Controllers;
using AliceInventory;

namespace AliceInventory.Logic.AliceResponseRender
{
    public static class AliceResponseRendererHelper
    {
        private static readonly TextAndSpeechTemplate[] GreetingRequestTemplates = 
        {
            new TextAndSpeechTemplate("Здравствуй!\nДанный навык предназначен для работы со списком предметов. Просто скажи, что ты хочешь записать, и я сделаю это за тебя"), 
        };
        private static readonly TextAndSpeechTemplate[] AddedTemplates = 
        {
            new TextAndSpeechTemplate("Добавлено {0} {1} {2} "),
            new TextAndSpeechTemplate("Добавила {0} {1} {2} "),
        };
        private static readonly TextAndSpeechTemplate[] AddCanceledTemplates = 
        {
            new TextAndSpeechTemplate("Отменено добавление {0} {1} {2}"),
            new TextAndSpeechTemplate("Отменила добавление {0} {1} {2}"),
        };
        private static readonly TextAndSpeechTemplate[] DeletedTemplates = 
        {
            new TextAndSpeechTemplate("Удалено {0} {1} {2}"),
            new TextAndSpeechTemplate("Удалила {0} {1} {2}"), 
        };
        private static readonly TextAndSpeechTemplate[] ClearRequestedTemplates = 
        {
            new TextAndSpeechTemplate("Вы точно хотите очистить весь список?"), 
        };
        private static readonly TextAndSpeechTemplate[] ClearedTemplates =
        {
            new TextAndSpeechTemplate("Список очищен"),
        };
        private static readonly TextAndSpeechTemplate[] ListReadTemplates = 
        {
            new TextAndSpeechTemplate(""), 
        };
        private static readonly TextAndSpeechTemplate[] MailSentTemplates = 
        {
            new TextAndSpeechTemplate("Отправила на {0}"),
            new TextAndSpeechTemplate("Отправлено"),
            new TextAndSpeechTemplate("Проверьте на {0}"),
        };
        private static readonly TextAndSpeechTemplate[] HelpRequestTemplates = 
        {
            new TextAndSpeechTemplate("Я запишу в список все, что вы произнесете.\n Просто скажите мне название и количество.\nЕсли вы захотите удалить что-то, просто скажите что и сколлько.\n Если понадобится, скажите очистить весь список и я это сделаю. Могу отправить список вам на E-mail."), 
        };
        private static readonly TextAndSpeechTemplate[] ExitRequestTemplates = 
        {
            new TextAndSpeechTemplate("Приятно было записывать за вами.\nВсего доброго!"), 
        };
        private static readonly TextAndSpeechTemplate[] ErrorTemplates =
        {
            new TextAndSpeechTemplate("Очень шумно, повторите пожалуйста"), 
            new TextAndSpeechTemplate("Повторите пожалуйста"),
            new TextAndSpeechTemplate("Ничего не поняла, давайте еще раз"),
        };

        private static readonly Button HelpButton = new Button()
        {
            Title = "Что ты умеешь?",
            Payload = "",
            Hide = true
        };
        private static readonly Button ReadListButton = new Button()
        {
            Title = "Показать всё",
            Payload = "",
            Hide = true
        };
        private static readonly Button ExitButton = new Button()
        {
            Title = "Закончить",
            Payload = "",
            Hide = true
        };
        private static readonly Button CancelButton = new Button()
        {
            Title = "Отмена",
            Payload = "",
            Hide = false
        };
        private static readonly Button YesButton = new Button()
        {
            Title = "Да",
            Payload = "",
            Hide = false
        };
        private static readonly Button NoButton = new Button()
        {
            Title = "Нет",
            Payload = "",
            Hide = false
        };

        private static readonly Button[] YesNoButtons = { YesButton, NoButton};
        private static readonly Button[] MainButtons = {HelpButton, ReadListButton, ExitButton};
        private static readonly Button[] MainButtonsWithCancel = {CancelButton, HelpButton, ReadListButton, ExitButton};


        public static AliceResponse CreateResponse(ProcessingResult result, Session session)
        {
            var aliceResponse = new AliceResponse()
            {
                Response = FormatResponse(result),
                Session = session,
                Version = "1.0"
            };
            return aliceResponse;
        }

        private static Response FormatResponse(ProcessingResult result)
        {
            switch (result.Result)
            {
                case InputProcessingResult.GreetingRequested:
                {
                    return FormatResponse(GreetingRequestTemplates, MainButtons);
                }

                case InputProcessingResult.Added:
                {
                    if (result.Data is Entry entry)
                    {
                        return FormatResponse(AddedTemplates, MainButtonsWithCancel,
                            entry.Name, entry.Count, entry.Unit.ToText());
                    }

                    goto default;
                }

                case InputProcessingResult.AddCanceled:
                {
                    if (result.Data is Entry entry)
                    {
                        return FormatResponse(AddCanceledTemplates, MainButtons,
                            entry.Name, entry.Count, entry.Unit.ToText());
                    }

                    goto default;
                }

                case InputProcessingResult.Deleted:
                {
                    if (result.Data is Entry entry)
                    {
                        return FormatResponse(DeletedTemplates, MainButtonsWithCancel,
                            entry.Name, entry.Count, entry.Unit.ToText());
                    }

                    goto default;
                }

                case InputProcessingResult.ClearRequested:
                {
                    return FormatResponse(ClearRequestedTemplates, YesNoButtons);
                }

                case InputProcessingResult.Cleared:
                {
                    return FormatResponse(ClearedTemplates, MainButtons);
                }

                case InputProcessingResult.ListRead:
                {
                    if (result.Data is Entry[] entries)
                    {
                        return FormatResponse(ListReadTemplates, MainButtons,
                            entries.ToTextList());
                    }

                    goto default;
                }

                case InputProcessingResult.MailSent:
                {
                    if (result.Data is string email)
                    {
                        return FormatResponse(MailSentTemplates, MainButtons, email);
                    }

                    goto default;
                }

                case InputProcessingResult.HelpRequested:
                {
                    return FormatResponse(HelpRequestTemplates, MainButtons);
                }

                case InputProcessingResult.ExitRequested:
                {
                    return FormatResponse(ExitRequestTemplates, MainButtons);
                }

                default:
                {
                    return FormatResponse(ErrorTemplates, MainButtons);
                }
            }
        }

        private static Response FormatResponse(TextAndSpeechTemplate[] templates, Button[] buttons, params object[] parts)
        {
            var response = templates.GetRandomItem().Format(parts);

            return new Response()
            {
                Text = response.Text,
                Tts = response.Speech,
                Buttons = buttons
            };
        }
    }
}