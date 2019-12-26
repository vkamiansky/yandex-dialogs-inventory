using System;
using System.Linq;

namespace AliceInventory.Logic
{
    public static class Extensions
    {
        public static string ToHtml(this Logic.UnitOfMeasure unit)
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
                default:
                    return "error";
            }
        }

        public static string ToHtml(this Logic.Entry[] entries)
        {
            return string.Join(
                "<br/>",
                entries
                    .GroupBy(x => x.Name)
                    .Select(x => $"{x.Key}: {string.Join(",", x.Select(y => $"{y.Quantity} {y.UnitOfMeasure.ToHtml()}"))}"));
        }

        public static Logic.Entry ToLogic(this Data.Entry entry)
        {
            if (entry == null)
                return null;

            return new Logic.Entry()
            {
                Name = entry.Name,
                Quantity = entry.Quantity,
                UnitOfMeasure = entry.UnitOfMeasure.ToLogic()
            };
        }

        public static Logic.Entry[] ToLogic(this Data.Entry[] entries)
        {
            return entries == null ? null : Array.ConvertAll(entries, x => x.ToLogic());
        }

        public static Logic.UnitOfMeasure ToLogic(this Data.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Data.UnitOfMeasure.Unit:
                    return Logic.UnitOfMeasure.Unit;
                case Data.UnitOfMeasure.Kg:
                    return Logic.UnitOfMeasure.Kg;
                case Data.UnitOfMeasure.L:
                    return Logic.UnitOfMeasure.L;
                case Data.UnitOfMeasure.Box:
                    return Logic.UnitOfMeasure.Box;
                default:
                    return Logic.UnitOfMeasure.Unit;
            }
        }

        public static Data.UnitOfMeasure ToData(this Logic.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Logic.UnitOfMeasure.Unit:
                    return Data.UnitOfMeasure.Unit;
                case Logic.UnitOfMeasure.Kg:
                    return Data.UnitOfMeasure.Kg;
                case Logic.UnitOfMeasure.L:
                    return Data.UnitOfMeasure.L;
                case Logic.UnitOfMeasure.Box:
                    return Data.UnitOfMeasure.Box;
                case Logic.UnitOfMeasure.Pack:
                    return Data.UnitOfMeasure.Pack;
                default:
                    return Data.UnitOfMeasure.Unit;
            }
        }
    }
}