using System;
using System.Linq;
using AliceInventory.Logic;

namespace AliceInventory.Controllers
{
    public static class Extensions
    {
        public static string ToText(this UnitOfMeasure unit)
        {
            switch (unit)
            {
                case UnitOfMeasure.Kg:
                    return "кг";
                case UnitOfMeasure.L:
                    return "л";
                case UnitOfMeasure.Unit:
                    return "шт";
                default:
                    return "error";
            }
        }

        public static string ToTextList(this Entry[] entries)
        {
            return string.Join(
                Environment.NewLine,
                entries
                    .GroupBy(x => x.Name)
                    .Select(x =>
                        $"{x.Key}: {string.Join(",", x.Select(y => $"{y.Quantity} {y.UnitOfMeasure.ToText()}"))}"));
        }
    }
}