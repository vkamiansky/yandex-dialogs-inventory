using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AliceInventory.Controllers;
using AliceInventory;
using AliceInventory.Logic;

namespace Controllers.AliceResponseRender
{
    public static class AliceResponseRendererHelper
    {
        private static readonly Button HelpButton = new Button()
        {
            Title = "Что ты умеешь?",
            Payload = "Помощь",
            Hide = true
        };

        private static readonly Button ReadListButton = new Button()
        {
            Title = "Показать всё",
            Payload = "Показать всё",
            Hide = true
        };

        private static readonly Button ExitButton = new Button()
        {
            Title = "Закончить",
            Payload = "Закончить",
            Hide = true
        };

        private static readonly Button CancelButton = new Button()
        {
            Title = "Отмена",
            Payload = "Отмена",
            Hide = false
        };

        private static readonly Button YesButton = new Button()
        {
            Title = "Да",
            Payload = "Да",
            Hide = false
        };

        private static readonly Button NoButton = new Button()
        {
            Title = "Нет",
            Payload = "Нет",
            Hide = false
        };

        private static readonly Button[] YesNoButtons = {YesButton, NoButton};
        private static readonly Button[] MainButtons = {HelpButton, ReadListButton, ExitButton};
        private static readonly Button[] MainButtonsWithCancel = {CancelButton, HelpButton, ReadListButton, ExitButton};


        private static readonly ResponseTemplate GreetingRequestTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate(
                    "Здравствуй, я рюкзак! Добавь в меня 5 яблок, а потом ещё 6. Почему бы ещё и немного груш не положить? Собери свой рюкзак и узнай \"Что в итоге?\" насчиталось. Так я работаю :)\nНо ты всегда можешь сказать Помощь"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate AddedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Добавлено \"{0}\" {1} {2} "),
                new TextAndSpeechTemplate("Добавила \"{0}\" {1} {2} "),
                new TextAndSpeechTemplate("Плюс \"{0}\" {1} {2} "),
            },
            Buttons = MainButtonsWithCancel
        };

        private static readonly ResponseTemplate AddCanceledTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Отменено добавление \"{0}\" {1} {2}"),
                new TextAndSpeechTemplate("Отменила добавление \"{0}\" {1} {2}"),
                new TextAndSpeechTemplate("Убрала \"{0}\" {1} {2}"),
                new TextAndSpeechTemplate("Убрано \"{0}\" {1} {2}"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate DeletedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Удалено \"{0}\" {1} {2}"),
                new TextAndSpeechTemplate("Удалила \"{0}\" {1} {2}"),
            },
            Buttons = MainButtonsWithCancel
        };

        private static readonly ResponseTemplate DeleteCanceledTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Вернула \"{0}\" {1} {2}"),
                new TextAndSpeechTemplate("Вернула"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate ClearRequestedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Вы точно хотите очистить весь список?"),
            },
            Buttons = YesNoButtons
        };

        private static readonly ResponseTemplate ClearedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Список очищен"),
                new TextAndSpeechTemplate("Рюкзак чист"),
                new TextAndSpeechTemplate("Готово"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate DeclinedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Хорошо"),
                new TextAndSpeechTemplate("Продолжаем"),
                new TextAndSpeechTemplate("Тогда в следующий раз"),
            },
            Buttons = MainButtons
        };
        
        private static readonly ResponseTemplate ListReadTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Ваш список:\n{0}"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate EmptyListReadTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Ваш список пуст"),
                new TextAndSpeechTemplate("Пока здесь пусто"),
                new TextAndSpeechTemplate("Пока ничего нет"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate MailSentTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Отправила на \"{0}\""),
                new TextAndSpeechTemplate("Отправлено"),
                new TextAndSpeechTemplate("Проверьте на \"{0}\""),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate RequestMailTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("К сожалению, я не знаю, какая у Вас почта, но если вы скажете, я запомню и никому не скажу. Какая у вас почта?")
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate MailIsEmptyTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("У меня и не было вашей почты")
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate MailAddedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Я добавила почту \"{0}\". Отправить на неё отчет?", "Я добавила почту. Отправить на неё отчет?"),
                new TextAndSpeechTemplate("Я сохранила почту \"{0}\". Отправить на неё отчет?", "Я сохранила почту. Отправить на неё отчет?")
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate MailDeletedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Я удалила почту \"{0}\"", "Я удалила вашу почту")
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate HelpRequestTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate(
                    "Примеры:\nДобавь 5 килограмм яблок\nУдали 3 груши\nОчисти всё\nОтправь на Email\nНо не обязательно говорить именно яблоки и груши, говори как хочешь, а я постараюсь понять"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate ExitRequestTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Приятно было записывать за вами.\nВсего доброго!"),
            },
            Buttons = MainButtons,
            EndSession = true
        };

        private static readonly ResponseTemplate ErrorTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Очень шумно, повторите пожалуйста"),
                new TextAndSpeechTemplate("Повторите пожалуйста"),
                new TextAndSpeechTemplate("Ничего не поняла, давайте еще раз"),
            },
            Buttons = MainButtons
        };

        private static readonly Dictionary<ResponseFormat, ResponseTemplate> responseTemplates;

        static AliceResponseRendererHelper()
        {
            responseTemplates = new Dictionary<ResponseFormat, ResponseTemplate>
            {
                [ResponseFormat.GreetingRequested] = GreetingRequestTemplate,
                [ResponseFormat.Added] = AddedTemplate,
                [ResponseFormat.AddCanceled] = AddCanceledTemplate,
                [ResponseFormat.Deleted] = DeletedTemplate,
                [ResponseFormat.DeleteCanceled] = DeleteCanceledTemplate,
                [ResponseFormat.Cleared] = ClearedTemplate,
                [ResponseFormat.ClearRequested] = ClearRequestedTemplate,
                [ResponseFormat.Declined] = DeclinedTemplate,
                [ResponseFormat.ExitRequested] = ExitRequestTemplate,
                [ResponseFormat.HelpRequested] = HelpRequestTemplate,
                [ResponseFormat.ListRead] = ListReadTemplate,
                [ResponseFormat.EmptyListRead] = EmptyListReadTemplate,
                [ResponseFormat.MailSent] = MailSentTemplate,
                [ResponseFormat.MailRequest] = RequestMailTemplate,
                [ResponseFormat.MailIsEmpty] = MailIsEmptyTemplate,
                [ResponseFormat.MailAdded] = MailAddedTemplate,
                [ResponseFormat.MailDeleted] = MailDeletedTemplate,
            };
        }

        public static AliceResponse CreateAliceResponse(ProcessingResult result, Session session, Responses responses)
        {
            var aliceResponse = new AliceResponse()
            {
                Response = CreateResponse(result, result.CultureInfo, responses.responseTemplates),
                Session = session,
                Version = "1.0"
            };
            return aliceResponse;
        }

        private static Response CreateResponse(ProcessingResult result, CultureInfo cultureInfo, Dictionary<ResponseFormat, ResponseTemplate> responseTemplates)
        {
            object[] formatArguments = new object[0];


            ResponseFormat format = ResponseFormat.Error;

            switch (result.Result)
            {
                case InputProcessingResult.GreetingRequested:
                {
                    format = ResponseFormat.GreetingRequested;
                    break;
                }
                case InputProcessingResult.Declined:
                {
                    format = ResponseFormat.Declined;
                    break;
                }
                case InputProcessingResult.Added:
                {
                    if (result.Data is SingleEntry entry)
                    {
                        format = ResponseFormat.Added;
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    }

                    break;
                }
                case InputProcessingResult.AddCanceled:
                {
                    if (result.Data is SingleEntry entry)
                    {
                        format = ResponseFormat.AddCanceled;
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    }

                    break;
                }
                case InputProcessingResult.Deleted:
                {
                    if (result.Data is SingleEntry entry)
                    {
                        format = ResponseFormat.Deleted;
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    }

                    break;
                }
                case InputProcessingResult.DeleteCanceled:
                {
                    if (result.Data is SingleEntry entry)
                    {
                        format = ResponseFormat.DeleteCanceled;
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    }

                    break;
                }
                case InputProcessingResult.ClearRequested:
                    format = ResponseFormat.ClearRequested;
                    break;
                case InputProcessingResult.Cleared:
                {
                    format = ResponseFormat.Cleared;
                    break;
                }
                case InputProcessingResult.ListRead:
                {
                    if (result.Data is Entry[] entries)
                    {
                        if (entries.Length > 0)
                        {
                            format = ResponseFormat.ListRead;
                            formatArguments = new object[] {entries.ToTextList()};
                        }
                        else
                        {
                            format = ResponseFormat.EmptyListRead;
                        }

                    }

                    break;
                }
                case InputProcessingResult.MailSent:
                {
                    if (result.Data is string email)
                    {
                        format = ResponseFormat.MailSent;
                        formatArguments = new object[] {email};
                    }
                    break;
                }
                case InputProcessingResult.RequestedMail:
                {
                    format = ResponseFormat.MailRequest;
                    break;
                }
                case InputProcessingResult.MailAdded:
                {
                    if (result.Data is string email)
                    {
                        format = ResponseFormat.MailAdded;
                        formatArguments = new object[] {email};
                    }
                    break;
                }
                case InputProcessingResult.MailDeleted:
                {
                    if (result.Data is string email)
                    {
                        format = ResponseFormat.MailDeleted;
                        formatArguments = new object[] {email};
                    }
                    else
                    {
                        format = ResponseFormat.MailIsEmpty;
                    }
                    break;
                }
                case InputProcessingResult.HelpRequested:
                {
                    format = ResponseFormat.HelpRequested;
                    break;
                }
                case InputProcessingResult.ExitRequested:
                {
                    format = ResponseFormat.ExitRequested;
                    break;
                }
                default: break;
            }

            var template = !responseTemplates.ContainsKey(format) ?
                ErrorTemplate : responseTemplates[format];

            var textAndSpeechTemplate = template.TextAndSpeechTemplates.GetRandomItem();

            return new Response()
            {
                Text = textAndSpeechTemplate.FormatText(cultureInfo, formatArguments),
                Tts = textAndSpeechTemplate.FormatSpeech(cultureInfo, formatArguments),
                Buttons = template.Buttons
            };

        }
    }
}