using System;
using AliceInventory;

namespace AliceInventory.Logic
{
    public static class Converter
    {
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
    }
}
