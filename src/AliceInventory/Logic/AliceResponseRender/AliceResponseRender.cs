using System;
using System.Collections.Generic;
using System.Globalization;
using AliceInventory.Controllers;
using Microsoft.Extensions.Localization;


namespace AliceInventory.Logic.AliceResponseRender
{
    public class AliceResponseRender : IAliceResponseRender
    {
        private readonly IStringLocalizer<AliceResponseRender> _localizer;

        public Dictionary<ResponseFormat, ResponseTemplate> responseTemplates;

        private readonly Button HelpButton = new Button()
        {
            Title = "Что ты умеешь?",
            Payload = "Помощь",
            Hide = true
        };

        private readonly Button ReadListButton = new Button()
        {
            Title = "Показать всё",
            Payload = "Показать всё",
            Hide = true
        };

        private readonly Button ExitButton = new Button()
        {
            Title = "Закончить",
            Payload = "Закончить",
            Hide = true
        };

        private readonly Button CancelButton = new Button()
        {
            Title = "Отмена",
            Payload = "Отмена",
            Hide = false
        };

        private Button YesButton = new Button()
        {
            Title = "Да",
            Payload = "Да",
            Hide = false
        };

        private readonly Button NoButton = new Button()
        {
            Title = "Нет",
            Payload = "Нет",
            Hide = false
        };

        private readonly Button[] YesNoButtons;
        private readonly Button[] MainButtons;
        private readonly Button[] MainButtonsWithCancel;

        public AliceResponseRender(IStringLocalizer<AliceResponseRender> localizer)
        {
            this._localizer = localizer;
            responseTemplates = new Dictionary<ResponseFormat, ResponseTemplate>();
            YesNoButtons = new Button[] {YesButton, NoButton};
            MainButtons = new Button[] {HelpButton, ReadListButton, ExitButton};
            MainButtonsWithCancel = new Button[] {CancelButton, HelpButton, ReadListButton, ExitButton};

            responseTemplates[ResponseFormat.GreetingRequested] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate(
                        _localizer["GreetingRequested"].Value),
                },
                Buttons = MainButtons

            };
            responseTemplates[ResponseFormat.Added] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Добавлено {0} {1} {2} "),
                    new TextAndSpeechTemplate("Добавила {0} {1} {2} "),
                    new TextAndSpeechTemplate("Плюс {0} {1} {2} "),
                },
                Buttons = MainButtonsWithCancel
            };
            responseTemplates[ResponseFormat.AddCanceled] = new ResponseTemplate()
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
            responseTemplates[ResponseFormat.Deleted] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Удалено {0} {1} {2}"),
                    new TextAndSpeechTemplate("Удалила {0} {1} {2}"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.DeleteCanceled] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Вернула {0} {1} {2}"),
                    new TextAndSpeechTemplate("Вернула"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.Cleared] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Вы точно хотите очистить весь список?"),
                },
                Buttons = YesNoButtons
            };
            responseTemplates[ResponseFormat.ClearRequested] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Список очищен"),
                    new TextAndSpeechTemplate("Рюкзак чист"),
                    new TextAndSpeechTemplate("Готово"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.Declined] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Хорошо"),
                    new TextAndSpeechTemplate("Продолжаем"),
                    new TextAndSpeechTemplate("Тогда в следующий раз"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.ExitRequested] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Приятно было записывать за вами.\nВсего доброго!"),
                },
                Buttons = MainButtons,
                EndSession = true
            };

            responseTemplates[ResponseFormat.HelpRequested] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate(
                        "Примеры:\nДобавь 5 килограмм яблок\nУдали 3 груши\nОчисти всё\nОтправь на Email\nНо не обязательно говорить именно яблоки и груши, говори как хочешь, а я постараюсь понять"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.ListRead] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Ваш список:\n{0}"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.EmptyListRead] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Ваш список пуст"),
                    new TextAndSpeechTemplate("Пока здесь пусто"),
                    new TextAndSpeechTemplate("Пока ничего нет"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.MailSent] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Отправила на {0}"),
                    new TextAndSpeechTemplate("Отправлено"),
                    new TextAndSpeechTemplate("Проверьте на {0}"),
                },
                Buttons = MainButtons
            };
            responseTemplates[ResponseFormat.Error] = new ResponseTemplate()
            {
                TextAndSpeechTemplates = new[]
                {
                    new TextAndSpeechTemplate("Очень шумно, повторите пожалуйста"),
                    new TextAndSpeechTemplate("Повторите пожалуйста"),
                    new TextAndSpeechTemplate("Ничего не поняла, давайте еще раз"),
                },
                Buttons = MainButtons
            };
        }
    }
}
