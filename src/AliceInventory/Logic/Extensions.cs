using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AliceInventory.Logic
{
    public static class Extensions
    {
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
                default:
                    return "error";
            }
        }

        public static string ToTextList(this Logic.Entry[] entries)
        {
            var stringBuilder = new StringBuilder();

            foreach (var entry in entries)
            {
                stringBuilder.Append($"{entry.Name}: ");

                Dictionary<UnitOfMeasure, double> units = entry.UnitValues;

                if (entry.UnitValues.Count == 1)
                {
                    var (unit, value) = units.First();
                    stringBuilder.AppendLine($"{value} {unit.ToText()}");
                }
                else
                {
                    foreach (var (unit, count) in units.Take(units.Count - 2))
                    {
                        stringBuilder.Append($"{count} {unit.ToText()}, ");
                    }

                    var lasts = units.Skip(units.Count - 2).ToArray();
                    var (preLastUnit, preLastCount) = lasts.First();
                    var (lastUnit, lastCount) = lasts.Last();
                    stringBuilder.AppendLine($"{preLastCount} {preLastUnit.ToText()} и {lastCount} {lastUnit.ToText()}");
                }
            }

            return stringBuilder.ToString();
        }

        private static readonly Random Random = new Random();
        public static T GetRandomItem<T>(this IReadOnlyList<T> collection)
        {
            return collection[Random.Next(0, collection.Count)];
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