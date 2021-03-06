﻿using System;
using System.Diagnostics;
using TestUtilities;
using Xunit;

namespace Meziantou.Framework.Win32.Jobs.Tests
{
    [Collection("JobObjectTests")]
    public class JobObjectTests
    {
        [RunIfFact(FactOperatingSystem.Windows, Timeout = 5000)]
        public void ShouldKillProcessOnTerminate()
        {
            using var job = new JobObject();
            job.SetLimits(new JobObjectLimits()
            {
                Flags = JobObjectLimitFlags.DieOnUnhandledException | JobObjectLimitFlags.KillOnJobClose,
            });

            var psi = new ProcessStartInfo
            {
                FileName = "ping",
                Arguments = "127.0.0.1 -n 100",
                UseShellExecute = true,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            Assert.False(process.WaitForExit(500)); // Ensure process is started

            job.AssignProcess(process);
            job.Terminate();

            process.WaitForExit();
        }

        [RunIfFact(FactOperatingSystem.Windows, Timeout = 5000)]
        public void KillOnJobClose_ShouldKillProcessOnClose()
        {
            using var job = new JobObject();
            job.SetLimits(new JobObjectLimits()
            {
                Flags = JobObjectLimitFlags.DieOnUnhandledException | JobObjectLimitFlags.KillOnJobClose,
            });

            var psi = new ProcessStartInfo
            {
                FileName = "ping",
                Arguments = "127.0.0.1 -n 100",
                UseShellExecute = true,
                CreateNoWindow = true,
            };

            using var process = Process.Start(psi);
            Assert.False(process.WaitForExit(500)); // Ensure process is started

            job.AssignProcess(process);
            job.Close();

            process.WaitForExit();
        }

        [RunIfFact(FactOperatingSystem.Windows)]
        public void CreateAndOpenJobObject()
        {
            var objectName = Guid.NewGuid().ToString("N");
            using var job = new JobObject(objectName);
            Assert.False(job.IsInvalid);

            using var openedJob = JobObject.Open(JobObjectAccessRights.AllAccess, inherited: true, objectName);
            Assert.False(openedJob.IsInvalid);
        }

        [RunIfFact(FactOperatingSystem.Windows)]
        public void SetUILimits()
        {
            using var job = new JobObject();
            job.SetUIRestrictions(Natives.JobObjectUILimit.ReadClipboard);
        }

        [RunIfFact(FactOperatingSystem.Windows)]
        public void IsAssignedToProcess_NotAssociated()
        {
            using var job = new JobObject();
            Assert.False(job.IsAssignedToProcess(Process.GetCurrentProcess()));
        }

        [RunIfFact(FactOperatingSystem.Windows)]
        public void IsAssignedToProcess_Associated()
        {
            using var job = new JobObject();
            var process = Process.GetCurrentProcess();
            job.AssignProcess(process);

            Assert.True(job.IsAssignedToProcess(process));
        }
    }
}
