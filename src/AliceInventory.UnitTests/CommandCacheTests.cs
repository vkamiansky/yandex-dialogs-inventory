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

            ProcessingResult cachedCommandUser1 = sut.Get("testUser1");
            sut.Set("testUser3", new ProcessingResult());
            ProcessingResult cachedCommandUser2 = sut.Get("testUser2");

            Assert.Null(cachedCommandUser1);
            Assert.Null(cachedCommandUser2);
        }

        [Fact]
        public void GetCommandFromCacheAfterInitialAdd()
        {
            CommandCache sut = new CommandCache();
            ProcessingResult commandUser1 = new ProcessingResult();
            ProcessingResult commandUser2 = new ProcessingResult();

            sut.Set("testUser1", commandUser1);
            sut.Set("testUser2", commandUser2);
            ProcessingResult cachedCommandUser1 = sut.Get("testUser1");
            ProcessingResult cachedCommandUser2 = sut.Get("testUser2");
            
            Assert.Equal(commandUser1, cachedCommandUser1);
            Assert.Equal(commandUser2, cachedCommandUser2);
        }

        [Fact]
        public void GetCommandFromCacheAfterRewrite()
        {
            CommandCache sut = new CommandCache();

            ProcessingResult command1 = new ProcessingResult();
            sut.Set("testUser", command1);
            ProcessingResult cachedCommand1 = sut.Get("testUser");

            Assert.Equal(command1, cachedCommand1);

            ProcessingResult command2 = new ProcessingResult();
            sut.Set("testUser", command2);
            ProcessingResult cachedCommand2 = sut.Get("testUser");

            Assert.Equal(command2, cachedCommand2);
        }
    }
}
//ура!