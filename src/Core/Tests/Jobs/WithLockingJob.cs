﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Extensions;
using Foundatio.Jobs;
using Foundatio.Lock;
using Xunit;
using Foundatio.Messaging;

namespace Foundatio.Tests.Jobs {
    public class WithLockingJob : JobBase {
        private readonly ILockProvider _locker = new CacheLockProvider(new InMemoryCacheClient(), new InMemoryMessageBus());
        public int RunCount { get; set; }

        protected override IDisposable GetJobLock() {
            return _locker.TryAcquireLock("WithLockingJob", TimeSpan.FromSeconds(1), TimeSpan.Zero);
        }

        protected override Task<JobResult> RunInternalAsync(CancellationToken token) {
            RunCount++;

            Thread.Sleep(150);
            Assert.True(_locker.IsLocked("WithLockingJob"));

            return Task.FromResult(JobResult.Success);
        }
    }
}
