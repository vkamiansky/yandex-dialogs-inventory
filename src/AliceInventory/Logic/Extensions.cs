using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic
{
    public static class Extensions
    {
        public static string ToText(this UnitOfMeasure unit)
        {
            switch (unit)
            {
                case UnitOfMeasure.Kg:
                    return "кг.";
                case UnitOfMeasure.L:
                    return "л.";
                case UnitOfMeasure.Unit:
                    return "шт.";
                default:
                    return "error";
            }
        }

        public static string ToTextList(this Entry[] entries)
        {
            var stringBuilder = new StringBuilder();
            foreach (var entry in entries)
            {
                stringBuilder.Append($"{entry.Name}: {entry.Count} {entry.Unit.ToText()}\n");
            }

            return stringBuilder.ToString();
        }

        private static readonly Random Random = new Random();
        public static T GetRandomItem<T>(this IReadOnlyList<T> collection)
        {
            return collection[Random.Next(0, collection.Count - 1)];
        }

        public static Logic.Entry ToLogic(this Data.Entry entry)
        {
            if (entry == null)
                return null;

            return new Logic.Entry
            {
                Name = entry.Name,
                Count = entry.Count,
                Unit = entry.Unit.ToLogic(),
            };
        }

        public static Data.Entry ToData(this Logic.Entry entry)
        {
            if (entry == null)
                return null;

            return new Data.Entry
            {
                Name = entry.Name,
                Count = entry.Count,
                Unit = entry.Unit.ToData(),
            };
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

        public static bool ContainsName(this GroupCollection group, string name)
        {
            return group.Any(x => x.Name == name);
        }
    }
}