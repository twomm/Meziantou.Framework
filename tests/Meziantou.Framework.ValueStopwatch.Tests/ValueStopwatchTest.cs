﻿using System;
using System.Threading.Tasks;
using Xunit;

namespace Meziantou.Framework.Tests
{
    public class ValueStopwatchTest
    {
        [Fact]
        public void IsActiveIsFalseForDefaultValueStopwatch()
        {
            Assert.False(default(ValueStopwatch).IsActive);
        }

        [Fact]
        public void IsActiveIsTrueWhenValueStopwatchStartedWithStartNew()
        {
            Assert.True(ValueStopwatch.StartNew().IsActive);
        }

        [Fact]
        public void GetElapsedTimeThrowsIfValueStopwatchIsDefaultValue()
        {
            var stopwatch = default(ValueStopwatch);
            Assert.Throws<InvalidOperationException>(() => stopwatch.GetElapsedTime());
        }

        [Fact]
        public async Task GetElapsedTimeReturnsTimeElapsedSinceStart()
        {
            var stopwatch = ValueStopwatch.StartNew();
            await Task.Delay(200);
            var elapsed = stopwatch.GetElapsedTime();
            Assert.True(elapsed.TotalMilliseconds > 0);
            Assert.True(elapsed.TotalMilliseconds < 5000);
        }
    }
}
