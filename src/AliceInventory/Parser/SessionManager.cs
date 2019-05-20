using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace ConsoleApp
{
    public class SessionManager
    {
        ConcurrentDictionary<string, UserSession> sessions;
        UserSession userSession = new UserSession();
        public UserSession GetSession(string clientId)
        {
            return sessions[clientId];
        }
    }
}
