using System;
using System.Linq;

namespace AliceInventory.Logic
{
    public static class Extensions
    {
        public static string ToHtml(this UnitOfMeasure unit)
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

        public static string ToHtml(this Entry[] entries)
        {
            return string.Join(
                "<br/>",
                entries
                    .GroupBy(x => x.Name)
                    .Select(x =>
                        $"{x.Key}: {string.Join(",", x.Select(y => $"{y.Quantity} {y.UnitOfMeasure.ToHtml()}"))}"));
        }

        public static Entry ToLogic(this Data.Entry entry)
        {
            if (entry == null)
                return null;

            return new Entry
            {
                Name = entry.Name,
                Quantity = entry.Quantity,
                UnitOfMeasure = entry.UnitOfMeasure.ToLogic()
            };
        }

        public static Entry[] ToLogic(this Data.Entry[] entries)
        {
            return entries == null ? null : Array.ConvertAll(entries, x => x.ToLogic());
        }

        public static UnitOfMeasure ToLogic(this Data.UnitOfMeasure unit)
        {
            switch (unit)
            {
                case Data.UnitOfMeasure.Unit:
                    return UnitOfMeasure.Unit;
                case Data.UnitOfMeasure.Kg:
                    return UnitOfMeasure.Kg;
                case Data.UnitOfMeasure.L:
                    return UnitOfMeasure.L;
                default:
                    return UnitOfMeasure.Unit;
            }
        }

        public static Data.UnitOfMeasure ToData(this UnitOfMeasure unit)
        {
            switch (unit)
            {
                case UnitOfMeasure.Unit:
                    return Data.UnitOfMeasure.Unit;
                case UnitOfMeasure.Kg:
                    return Data.UnitOfMeasure.Kg;
                case UnitOfMeasure.L:
                    return Data.UnitOfMeasure.L;
                default:
                    return Data.UnitOfMeasure.Unit;
            }
        }
    }
}