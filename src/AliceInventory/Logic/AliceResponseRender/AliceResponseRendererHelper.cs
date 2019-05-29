using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AliceInventory.Controllers;
using AliceInventory;

namespace AliceInventory.Logic.AliceResponseRender
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
                new TextAndSpeechTemplate("Добавлено {0} {1} {2} "),
                new TextAndSpeechTemplate("Добавила {0} {1} {2} "),
                new TextAndSpeechTemplate("Плюс {0} {1} {2} "),
            },
            Buttons = MainButtonsWithCancel
        };

        private static readonly ResponseTemplate AddCanceledTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Отменено добавление {0} {1} {2}"),
                new TextAndSpeechTemplate("Отменила добавление {0} {1} {2}"),
                new TextAndSpeechTemplate("Убрала {0} {1} {2}"),
                new TextAndSpeechTemplate("Убрано {0} {1} {2}"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate DeletedTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Удалено {0} {1} {2}"),
                new TextAndSpeechTemplate("Удалила {0} {1} {2}"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate DeleteCanceledTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Вернула {0} {1} {2}"),
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
                new TextAndSpeechTemplate("Отправила на {0}"),
                new TextAndSpeechTemplate("Отправлено"),
                new TextAndSpeechTemplate("Проверьте на {0}"),
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
            responseTemplates = new Dictionary<ResponseFormat, ResponseTemplate>() ;
            responseTemplates[ResponseFormat.GreetingRequested] = GreetingRequestTemplate;
            responseTemplates[ResponseFormat.Added] = AddedTemplate;
            responseTemplates[ResponseFormat.AddCanceled] = AddCanceledTemplate;
            responseTemplates[ResponseFormat.Deleted] = DeletedTemplate;
            responseTemplates[ResponseFormat.DeleteCanceled] = DeleteCanceledTemplate;
            responseTemplates[ResponseFormat.Cleared] = ClearedTemplate;
            responseTemplates[ResponseFormat.ClearRequested] = ClearRequestedTemplate;
            responseTemplates[ResponseFormat.Declined] = DeclinedTemplate;
            responseTemplates[ResponseFormat.ExitRequested] = ExitRequestTemplate;
            responseTemplates[ResponseFormat.HelpRequested] = HelpRequestTemplate;
            responseTemplates[ResponseFormat.ListRead] = ListReadTemplate;
            responseTemplates[ResponseFormat.EmptyListRead] =EmptyListReadTemplate;
            responseTemplates[ResponseFormat.MailSent] =  MailSentTemplate;
        }

        public static AliceResponse CreateAliceResponse(ProcessingResult result, Session session)
        {
            var aliceResponse = new AliceResponse()
            {
                Response = CreateResponse(result, result.CultureInfo),
                Session = session,
                Version = "1.0"
            };
            return aliceResponse;
        }

        private static Response CreateResponse(ProcessingResult result, CultureInfo cultureInfo)
        {
            bool isError = false;
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
                    format = ResponseFormat.Added;
                    if (result.Data is Entry entry)
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    break;
                }
                case InputProcessingResult.AddCanceled:
                {
                    format = ResponseFormat.AddCanceled;
                    if (result.Data is Entry entry)
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    break;
                }
                case InputProcessingResult.Deleted:
                {
                    format = ResponseFormat.Deleted;
                    if (result.Data is Entry entry)
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
                    break;
                }
                case InputProcessingResult.DeleteCanceled:
                {
                    format = ResponseFormat.DeleteCanceled;
                    if (result.Data is Entry entry)
                        formatArguments = new object[] {entry.Name, entry.Count, entry.Unit.ToText()};
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
                    if (result.Data is Logic.Entry[] entries)
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
                    format = ResponseFormat.MailSent;
                    if (result.Data is string email)
                    {
                        formatArguments = new object[] {email};
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

            ResponseTemplate template;
            if (isError || !responseTemplates.ContainsKey(format))
            {
                template = ErrorTemplate;
            }
            else
            {
                template = responseTemplates[format];
            }

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