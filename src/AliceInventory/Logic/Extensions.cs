using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
                default:
                    return Data.UnitOfMeasure.Unit;
            }
        }
    }
}