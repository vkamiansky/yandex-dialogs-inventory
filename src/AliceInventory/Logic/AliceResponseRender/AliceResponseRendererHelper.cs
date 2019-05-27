using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AliceInventory.Controllers;
using AliceInventory;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            Buttons = MainButtons
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

        private static readonly Dictionary<InputProcessingResult, ResponseTemplate> responseTemplates;

        static AliceResponseRendererHelper()
        {
            responseTemplates = new Dictionary<InputProcessingResult, ResponseTemplate>() ;
            responseTemplates[InputProcessingResult.Added] = AddedTemplate;
            responseTemplates[InputProcessingResult.GreetingRequested] = GreetingRequestTemplate;
            responseTemplates[InputProcessingResult.AddCanceled] = AddCanceledTemplate;
            responseTemplates[InputProcessingResult.Cleared] = ClearedTemplate;
            responseTemplates[InputProcessingResult.ClearRequested] = ClearRequestedTemplate;
            responseTemplates[InputProcessingResult.Deleted] = DeletedTemplate;
            responseTemplates[InputProcessingResult.ExitRequested] = ExitRequestTemplate;
            responseTemplates[InputProcessingResult.HelpRequested] = HelpRequestTemplate;
            responseTemplates[InputProcessingResult.ListRead] = ListReadTemplate;
            responseTemplates[InputProcessingResult.MailSent] =  MailSentTemplate;
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
            object[] parts = new object[0];

            switch (result.Result)
            {
                case InputProcessingResult.Added:
                case InputProcessingResult.AddCanceled:
                case InputProcessingResult.Deleted:
                {
                    if (result.Data is Entry entry)
                    {
                        parts = new object[] {entry.Name, entry.Count, entry.Unit.ToText()}; 
                    }
                    else
                    {
                        isError = true;
                    }
                    break;
                }
                case InputProcessingResult.ListRead:
                {
                    if (result.Data is Entry[] entries)
                    {
                        parts = new object[] {entries.ToTextList()};
                    }
                    else
                    {
                        isError = true;
                    }
                    break;
                }

                case InputProcessingResult.MailSent:
                {
                    if (result.Data is string email)
                    {
                        parts = new object[] {email};
                    }
                    else
                    {
                        isError = true;
                    }
                    break;
                }
            }

            ResponseTemplate template;
            if (isError || !responseTemplates.ContainsKey(result.Result))
            {
                template = ErrorTemplate;
            }
            else
            {
                template = responseTemplates[result.Result];
            }

            var textAndSpeechTemplate = template.TextAndSpeechTemplates.GetRandomItem();

            return new Response()
            {
                Text = textAndSpeechTemplate.FormatText(cultureInfo, parts),
                Tts = textAndSpeechTemplate.FormatSpeech(cultureInfo, parts),
                Buttons = template.Buttons
            };

        }
    }
}