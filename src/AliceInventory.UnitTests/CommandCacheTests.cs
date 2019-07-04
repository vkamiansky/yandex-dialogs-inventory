using System;
using Xunit;
using AliceInventory.Logic;

namespace AliceInventory.UnitTests
{
    public class ResultCacheTests
    {
        [Fact]
        public void GetEmptyResultFromCache()
        {
            ResultCache sut = new ResultCache();

            ProcessingResult cachedResultUser1 = sut.Get("testUser1");
            sut.Set("testUser3", new ProcessingResult());
            ProcessingResult cachedResultUser2 = sut.Get("testUser2");

            Assert.Null(cachedResultUser1);
            Assert.Null(cachedResultUser2);
        }

        [Fact]
        public void GetResultFromCacheAfterInitialAdd()
        {
            ResultCache sut = new ResultCache();
            ProcessingResult resultUser1 = new ProcessingResult();
            ProcessingResult resultUser2 = new ProcessingResult();

            sut.Set("testUser1", resultUser1);
            sut.Set("testUser2", resultUser2);
            ProcessingResult cachedResultUser1 = sut.Get("testUser1");
            ProcessingResult cachedResultUser2 = sut.Get("testUser2");
            
            Assert.Equal(resultUser1, cachedResultUser1);
            Assert.Equal(resultUser2, cachedResultUser2);
        }

        [Fact]
        public void GetResultFromCacheAfterRewrite()
        {
            ResultCache sut = new ResultCache();

            ProcessingResult result1 = new ProcessingResult();
            sut.Set("testUser", result1);
            ProcessingResult cachedResult1 = sut.Get("testUser");

            Assert.Equal(result1, cachedResult1);

            ProcessingResult Result = new ProcessingResult();
            sut.Set("testUser", Result);
            ProcessingResult cachedResult2 = sut.Get("testUser");

            Assert.Equal(Result, cachedResult2);
        }
    }
}
//ура!