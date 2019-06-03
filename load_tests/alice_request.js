export let aliceRequest =
{
    "meta":
    {
        "locale": "ru-RU",
        "timezone": "Europe/Moscow",
        "client_id": "ru.yandex.searchplugin/5.80 (Samsung Galaxy; Android 4.4)",
    },
    "request":
    {
        "command": "default",
        "type": "SimpleUtterance",
        "payload": {},
    },
    "session":
    {
        "new": true,
        "message_id": 0,
        "session_id": "default",
        "user_id": "default"
    },
};


var commands =
[
    "привет",
    "здравствуй",
    "добавь 1 яблоко",
    "добавь штуку яблоко",
    "добавь килограмм яблоко",
    "добавь яблоко 1 штуку",
    "9 килограммов картошки",
    "удали яблоко",
    "удали килограмм картошки",
    "удали 2 яблоко",
    "очисти",
    "да",
    "конечно",
    "нет",
    "не надо",
    "отмена",
    "отмени",
    "покажи всё",
    "помоги",
    "что ты умеешь",
    "погода спб",
    "как дела",
];

export function generateCommand()
{
    let index = Math.floor(Math.random() * commands.length);
    return commands[index];
}
