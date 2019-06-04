using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceInventory.Logic.Email
{
    public interface IInventoryEmailService
    {
        void SendListAsync(string email, Logic.Entry[] entries);

    }
}
