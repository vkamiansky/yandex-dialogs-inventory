using System;
using System.Collections.Generic;
using System.Globalization;
using AliceInventory.Logic;
using AliceInventory.Logic.Core.Errors;

namespace AliceInventory.Controllers.AliceResponseRender
{
    public static class AliceResponseRendererHelper
    {
        class ResponseArgs
        {
            public ResponseFormat ResponseType { get; set; }
            public object[] Data { get; set; } = { };
        }


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

        private static readonly Button[] YesNoButtons = { YesButton, NoButton };
        private static readonly Button[] MainButtons = { HelpButton, ReadListButton, ExitButton };
        private static readonly Button[] MainButtonsWithCancel = { CancelButton, HelpButton, ReadListButton, ExitButton };


        private static readonly ResponseTemplate GreetingRequestTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate(
                    "Здравствуй, я навык Учёт! Скажи \"добавь 5 яблок\", или \"добавь яблочный сок 5 литров\". Я понимаю единицы измерения. Могу вывести итоговый список, или отправить его на почту. Так я работаю :)\n Но ты всегда можешь сказать \"Помощь\".")
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
                new TextAndSpeechTemplate("Список был очищен"),
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

        private static readonly ResponseTemplate ItemReadTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Ваш запрос:\n{0}"),
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

        private static readonly ResponseTemplate NoItemReadTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("В списке такого не найдено")
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
                    "Примеры команд:\nДобавь 5 килограмм яблок.\nУдали 3 груши\nОчисти всё.\nОтправь на Email.\nНо не обязательно говорить именно яблоки и груши, говори как хочешь, а я постараюсь понять.")
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

        private static readonly ResponseTemplate EntryNotFoundTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Вы не добавляли {0} в {1}"),
                new TextAndSpeechTemplate("У вас {0} не храниться в {1}"),
                new TextAndSpeechTemplate("Не нашла {0} в {1} в вашем списке"),
                new TextAndSpeechTemplate("Но в списке нет {0} в {1}"),
                new TextAndSpeechTemplate("В списке нет {0} в {1}"),
                new TextAndSpeechTemplate("Я не смогла найти ни одного {1} {0} в отчёте"),
            },
            Buttons = MainButtons
        };

        private static readonly ResponseTemplate NotEnoughEntryToDeleteErrorTemplate = new ResponseTemplate()
        {
            TextAndSpeechTemplates = new[]
            {
                new TextAndSpeechTemplate("Не могу удалить {0} {1}, в список добавлено только {2}"),
                new TextAndSpeechTemplate("У вас только {2} {1}")
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
                [ResponseFormat.ItemRead] = ItemReadTemplate,
                [ResponseFormat.EmptyListRead] = EmptyListReadTemplate,
                [ResponseFormat.EmptyItemRead] = NoItemReadTemplate,
                [ResponseFormat.MailSent] = MailSentTemplate,
                [ResponseFormat.MailRequest] = RequestMailTemplate,
                [ResponseFormat.MailIsEmpty] = MailIsEmptyTemplate,
                [ResponseFormat.MailAdded] = MailAddedTemplate,
                [ResponseFormat.MailDeleted] = MailDeletedTemplate,
                [ResponseFormat.EntryNotFound] = EntryNotFoundTemplate,
                [ResponseFormat.NotEnoughEntryToDelete] = NotEnoughEntryToDeleteErrorTemplate,
            };
        }

        public static AliceResponse CreateAliceResponse(ProcessingResult result, Session session, Func<int, int> variantSelector)
        {
            var responseArgs = GetResponseArgs(result);
            var response = CreateResponse(responseArgs, result.CultureInfo, variantSelector);

            var aliceResponse = new AliceResponse()
            {
                Response = response,
                Session = session,
                Version = "1.0"
            };
            return aliceResponse;
        }

        private static ResponseArgs GetResponseArgs(ProcessingResult result)
        {
            switch (result.Type)
            {
                case ProcessingResultType.GreetingRequested:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.GreetingRequested
                        };
                    }
                case ProcessingResultType.Declined:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.Declined
                        };
                    }
                case ProcessingResultType.Added
                    when result.Data is Entry entry:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.Added,
                            Data = new object[] { entry.Name, entry.Quantity, entry.UnitOfMeasure.ToText() }
                        };
                    }
                case ProcessingResultType.AddCanceled
                    when result.Data is Entry entry:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.AddCanceled,
                            Data = new object[] { entry.Name, entry.Quantity, entry.UnitOfMeasure.ToText() }
                        };
                    }
                case ProcessingResultType.Deleted
                    when result.Data is Entry entry:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.Deleted,
                            Data = new object[] { entry.Name, entry.Quantity, entry.UnitOfMeasure.ToText() }
                        };
                    }
                case ProcessingResultType.DeleteCanceled
                    when result.Data is Entry entry:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.DeleteCanceled,
                            Data = new object[] { entry.Name, entry.Quantity, entry.UnitOfMeasure.ToText() }
                        };
                    }

                case ProcessingResultType.ClearRequested:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.ClearRequested
                        };
                    }

                case ProcessingResultType.Cleared:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.Cleared
                        };
                    }

                case ProcessingResultType.ListRead
                    when result.Data is Logic.Entry[] entries:
                    {
                        if (entries.Length > 0)
                        {
                            return new ResponseArgs()
                            {
                                ResponseType = ResponseFormat.ListRead,
                                Data = new object[] { entries.ToTextList() }
                            };
                        }
                        else
                        {
                            return new ResponseArgs()
                            {
                                ResponseType = ResponseFormat.EmptyListRead
                            };
                        }
                    }

                case ProcessingResultType.ItemRead
                    when result.Data is Entry[] entries:
                {
                    if (entries.Length > 0)
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.ItemRead,
                            Data = new object[] { entries.ToTextList() }
                        };
                    }
                    else
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.EmptyItemRead
                        };
                    }
                }

                case ProcessingResultType.MailSent
                    when result.Data is string email:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.MailSent,
                            Data = new object[] { email }
                        };
                    }

                case ProcessingResultType.RequestedMail:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.MailRequest,
                        };
                    }

                case ProcessingResultType.MailAdded
                    when result.Data is string email:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.MailAdded,
                            Data = new object[] { email }
                        };
                    }

                case ProcessingResultType.MailDeleted
                    when result.Data is string email:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.MailDeleted,
                            Data = new object[] { email }
                        };
                    }

                case ProcessingResultType.HelpRequested:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.HelpRequested,
                        };
                    }

                case ProcessingResultType.ExitRequested:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.ExitRequested,
                        };
                    }

                case ProcessingResultType.Error
                    when result.Error is EntryNotFoundInDatabaseError error:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.EntryNotFound,
                            Data = new object[] { error.EntryName, error.EntryUnit.ToText() }
                        };
                    }

                case ProcessingResultType.Error
                    when result.Error is NotEnoughEntryToDeleteError error:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.NotEnoughEntryToDelete,
                            Data = new object[] { error.Expected, error.EntryName, error.Actual }
                        };
                    }
                case ProcessingResultType.Error
                    when result.Error is MailIsEmptyError:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.MailIsEmpty
                        };
                    }
                case ProcessingResultType.Error
                    when result.Error is EmptyEntryListError:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.EmptyListRead
                        };
                    }
                default:
                    {
                        return new ResponseArgs()
                        {
                            ResponseType = ResponseFormat.Error
                        };
                    }
            }
        }

        private static Response CreateResponse(ResponseArgs args, CultureInfo cultureInfo, Func<int, int> variantSelector)
        {
            ResponseTemplate template;
            if (responseTemplates.ContainsKey(args.ResponseType))
                template = responseTemplates[args.ResponseType];
            else
                template = ErrorTemplate;

            var answerId = variantSelector(template.TextAndSpeechTemplates.Length);
            var textAndSpeechTemplate = template.TextAndSpeechTemplates[answerId];

            return new Response()
            {
                Text = textAndSpeechTemplate.FormatText(cultureInfo, args.Data),
                Tts = textAndSpeechTemplate.FormatSpeech(cultureInfo, args.Data),
                Buttons = template.Buttons
            };
        }
    }
}