using System;
using System.Globalization;
using System.Linq;

namespace AliceInventory.Controllers
{
    public static class Extensions
    {
        public static Logic.UserInput ToUserInput(this AliceRequest request)
        {
            return new Logic.UserInput
            {
                Raw = request.Request.OriginalUtterance,
                Prepared = request.Request.Command,
                Button = request.Request.Payload,
                CultureInfo = new CultureInfo(request.Meta.Locale)
            };
        }

        public static string ToText(this Logic.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Logic.UnitOfMeasure.Kg:
                    return "кг";
                case Logic.UnitOfMeasure.L:
                    return "л";
                case Logic.UnitOfMeasure.Unit:
                    return "шт";
                case Logic.UnitOfMeasure.Box:
                    return "ящиков";
                case Logic.UnitOfMeasure.Pack:
                    return "упаковок";
                default:
                    return "error";
            }
        }

        public static string ToTextList(this Logic.Entry[] entries)
        {
            return string.Join(
                Environment.NewLine,
                entries
                    .GroupBy(x => x.Name)
                    .Select(x => $"{x.Key}: {string.Join(",", x.Select(y => $"{y.Quantity} {y.UnitOfMeasure.ToText()}"))}"));
        }
    }
}