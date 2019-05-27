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
            new TextAndSpeechTemplate("Здравствуй, я рюкзак! Добавь в меня 5 яблок, а потом ещё 6. Почему бы ещё и немного груш не положить? Собери свой рюкзак и узнай \"Что в итоге?\" насчиталось. Так я работаю :)\nНо ты всегда можешь сказать Помощь"), 
        };
        private static readonly TextAndSpeechTemplate[] AddedTemplates = 
        {
            new TextAndSpeechTemplate("Добавлено {0} {1} {2} "),
            new TextAndSpeechTemplate("Добавила {0} {1} {2} "),
            new TextAndSpeechTemplate("Плюс {0} {1} {2} "),
        };
        private static readonly TextAndSpeechTemplate[] AddCanceledTemplates = 
        {
            new TextAndSpeechTemplate("Отменено добавление {0} {1} {2}"),
            new TextAndSpeechTemplate("Отменила добавление {0} {1} {2}"),
            new TextAndSpeechTemplate("Убрала {0} {1} {2}"),
            new TextAndSpeechTemplate("Убрано {0} {1} {2}"),
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
            new TextAndSpeechTemplate("Ваш список:\n{0}"), 
        };
        private static readonly TextAndSpeechTemplate[] MailSentTemplates = 
        {
            new TextAndSpeechTemplate("Отправила на {0}"),
            new TextAndSpeechTemplate("Отправлено"),
            new TextAndSpeechTemplate("Проверьте на {0}"),
        };
        private static readonly TextAndSpeechTemplate[] HelpRequestTemplates = 
        {
            new TextAndSpeechTemplate("Примеры:\nДобавь 5 килограмм яблок\nУдали 3 груши\nОчисти всё\nОтправь на Email\nНо не обязательно говорить именно яблоки и груши, говори как хочешь, а я постараюсь понять"), 
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
            Payload = "Help",
            Hide = true
        };
        private static readonly Button ReadListButton = new Button()
        {
            Title = "Показать всё",
            Payload = "ReadList",
            Hide = true
        };
        private static readonly Button ExitButton = new Button()
        {
            Title = "Закончить",
            Payload = "Exit",
            Hide = true
        };
        private static readonly Button CancelButton = new Button()
        {
            Title = "Отмена",
            Payload = "Cancel",
            Hide = false
        };
        private static readonly Button YesButton = new Button()
        {
            Title = "Да",
            Payload = "Yes",
            Hide = false
        };
        private static readonly Button NoButton = new Button()
        {
            Title = "Нет",
            Payload = "No",
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