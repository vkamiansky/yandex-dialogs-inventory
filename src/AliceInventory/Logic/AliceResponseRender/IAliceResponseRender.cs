using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AliceInventory.Controllers;

namespace AliceInventory.Logic.AliceResponseRender
{
    public interface IAliceResponseRender
    {
        AliceResponse CreateAliceResponse(ProcessingResult result, Session session);

    }
}
