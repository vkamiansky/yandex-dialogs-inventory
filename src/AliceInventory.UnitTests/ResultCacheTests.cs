using AliceInventory.Logic;
using AliceInventory.Logic.Cache;
using Xunit;

namespace AliceInventory.UnitTests
{
    public class ResultCacheTests
    {
        [Fact]
        public void GetEmptyResultFromCache()
        {
            var sut = new ResultCache();

            var cachedResultUser1 = sut.Get("testUser1");
            sut.Set("testUser3", new ProcessingResult());
            var cachedResultUser2 = sut.Get("testUser2");

            Assert.Null(cachedResultUser1);
            Assert.Null(cachedResultUser2);
        }

        [Fact]
        public void GetResultFromCacheAfterInitialAdd()
        {
            var sut = new ResultCache();
            var resultUser1 = new ProcessingResult();
            var resultUser2 = new ProcessingResult();

            sut.Set("testUser1", resultUser1);
            sut.Set("testUser2", resultUser2);
            var cachedResultUser1 = sut.Get("testUser1");
            var cachedResultUser2 = sut.Get("testUser2");

            Assert.Equal(resultUser1, cachedResultUser1);
            Assert.Equal(resultUser2, cachedResultUser2);
        }

        [Fact]
        public void GetResultFromCacheAfterRewrite()
        {
            var sut = new ResultCache();

            var result1 = new ProcessingResult();
            sut.Set("testUser", result1);
            var cachedResult1 = sut.Get("testUser");

            Assert.Equal(result1, cachedResult1);

            var Result = new ProcessingResult();
            sut.Set("testUser", Result);
            var cachedResult2 = sut.Get("testUser");

            Assert.Equal(Result, cachedResult2);
        }
    }
}