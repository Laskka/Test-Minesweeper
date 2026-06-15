using System;
using App.Domain;
using NUnit.Framework;

namespace App.Tests
{
    public sealed class GameConfigTests
    {
        [Test]
        public void ZeroWidth_Throws()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new GameConfig(0, 9, 10));

        [Test]
        public void ZeroHeight_Throws()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new GameConfig(9, 0, 10));

        [Test]
        public void NegativeMineCount_Throws()
            => Assert.Throws<ArgumentOutOfRangeException>(() => new GameConfig(9, 9, -1));

        [Test]
        public void MineCount_IsClampedToLeaveAtLeastOneSafeCell()
        {
            var config = new GameConfig(3, 3, 100);

            Assert.AreEqual(3 * 3 - 1, config.MineCount);
        }
    }
}
