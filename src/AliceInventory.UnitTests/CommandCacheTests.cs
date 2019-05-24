using System;
using Xunit;
using AliceInventory.Logic;

namespace AliceInventory.UnitTests
{
    public class CommandCacheTests
    {
        [Fact]
        public void GetEmptyCommandFromCache()
        {
            CommandCache sut = new CommandCache();

            ProcessingCommand cachedCommandUser1 = sut.Get("testUser1");
            sut.Set("testUser3", new ProcessingCommand());
            ProcessingCommand cachedCommandUser2 = sut.Get("testUser2");

            Assert.Equal(null, cachedCommandUser1);
            Assert.Equal(null, cachedCommandUser2);
        }

        [Fact]
        public void GetCommandFromCacheAfterInitialAdd()
        {
            CommandCache sut = new CommandCache();
            ProcessingCommand commandUser1 = new ProcessingCommand();
            ProcessingCommand commandUser2 = new ProcessingCommand();

            sut.Set("testUser1", commandUser1);
            sut.Set("testUser2", commandUser2);
            ProcessingCommand cachedCommandUser1 = sut.Get("testUser1");
            ProcessingCommand cachedCommandUser2 = sut.Get("testUser2");
            
            Assert.Equal(commandUser1, cachedCommandUser1);
            Assert.Equal(commandUser2, cachedCommandUser2);
        }

        [Fact]
        public void GetCommandFromCacheAfterRewrite()
        {
            CommandCache sut = new CommandCache();
            
            ProcessingCommand command1 = new ProcessingCommand();
            sut.Set("testUser", command1);
            ProcessingCommand cachedCommand1 = sut.Get("testUser");

            Assert.Equal(command1, cachedCommand1);

            ProcessingCommand command2 = new ProcessingCommand();
            sut.Set("testUser", command2);
            ProcessingCommand cachedCommand2 = sut.Get("testUser");

            Assert.Equal(command2, cachedCommand2);
        }
    }
}
//ура!