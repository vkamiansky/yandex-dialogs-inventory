
   using System.Collections.Concurrent;

    namespace ConsoleApp
    {
        public class SessionManager     
        {
            ConcurrentDictionary<string, UserSession>  dic;
            UserSession session=new UserSession();
            public UserSession GetSession(string clientId)
            {
                return dic[clientId];
            }
        }
    }