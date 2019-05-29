using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Email
{
    public interface IAliceEmailService
    {
        void SendListAsync(string email, Data.Entry[] entries);

    }
}
