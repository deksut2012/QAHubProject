using QAHub.Api.Domain.Execution;

namespace QAHub.Api.Tests;

public sealed class ExecutionDomainTests
{
    [Fact] public void FailedRequiresActualEvidenceAndReason() => Assert.Throws<ArgumentException>(() => new TestRunAttempt(Guid.NewGuid(), 1, ExecutionResult.Failed, "", "", "", "qa"));
    [Fact] public void NotRunCannotBeSubmitted() => Assert.Throws<ArgumentException>(() => new TestRunAttempt(Guid.NewGuid(), 1, ExecutionResult.NotRun, "", "", "", "qa"));
    [Fact] public void PassedCanBeRecordedWithoutEvidence() => Assert.Equal(ExecutionResult.Passed, new TestRunAttempt(Guid.NewGuid(), 1, ExecutionResult.Passed, "", "", "", "qa").Result);
    [Fact] public void RerunKeepsIndependentAttemptNumber()
    {
        var id = Guid.NewGuid();
        var first = new TestRunAttempt(id, 1, ExecutionResult.Failed, "actual", "evidence", "reason", "qa");
        var second = new TestRunAttempt(id, 2, ExecutionResult.Passed, "", "", "", "qa");
        Assert.Equal(1, first.AttemptNumber); Assert.Equal(2, second.AttemptNumber);
    }
    [Fact] public void EmptyCycleCannotStart()
    {
        var cycle = new TestCycle(Guid.NewGuid(), Guid.NewGuid(), null, "Regression", "qa");
        Assert.Throws<InvalidOperationException>(cycle.Start);
    }
    [Fact] public void CycleCompletesOnlyAfterEveryItemHasResult()
    {
        var cycle = new TestCycle(Guid.NewGuid(), Guid.NewGuid(), null, "Regression", "qa");
        var item = new TestCycleItem(cycle.Id, Guid.NewGuid(), "qa"); cycle.Items.Add(item); cycle.Start();
        Assert.Throws<InvalidOperationException>(cycle.Complete);
        item.Attempts.Add(new TestRunAttempt(item.Id, 1, ExecutionResult.Passed, "", "", "", "qa")); cycle.Complete();
        Assert.Equal(TestCycleStatus.Completed, cycle.Status); Assert.NotNull(cycle.CompletedAtUtc);
    }
    [Fact] public void EvidenceRejectsFilesOverTenMegabytes() => Assert.Throws<ArgumentException>(() => new TestRunEvidence(Guid.NewGuid(), "large.bin", "application/octet-stream", new byte[10 * 1024 * 1024 + 1], "qa"));
}
