namespace QAHub.Api.Domain.Execution;
public sealed class TestCycle
{
 private TestCycle(){}
 public TestCycle(Guid productId,Guid environmentId,Guid? buildId,string name,string assignee){Id=Guid.NewGuid();ProductId=productId;EnvironmentId=environmentId;BuildId=buildId;Name=name.Trim();Assignee=assignee.Trim();Status=TestCycleStatus.Draft;CreatedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;}public Guid ProductId{get;private set;}public Guid EnvironmentId{get;private set;}public Guid? BuildId{get;private set;}public string Name{get;private set;}=string.Empty;public string Assignee{get;private set;}=string.Empty;public TestCycleStatus Status{get;private set;}public DateTimeOffset CreatedAtUtc{get;private set;}public DateTimeOffset? StartedAtUtc{get;private set;}public DateTimeOffset? CompletedAtUtc{get;private set;}public List<TestCycleItem> Items{get;private set;}=[];
 public void Start(){if(Status!=TestCycleStatus.Draft||Items.Count==0)throw new InvalidOperationException("Only a non-empty draft cycle can start.");Status=TestCycleStatus.InProgress;StartedAtUtc=DateTimeOffset.UtcNow;}
 public void Complete(){if(Status!=TestCycleStatus.InProgress||Items.Count==0||Items.Any(x=>x.Attempts.Count==0||x.Attempts.OrderByDescending(a=>a.AttemptNumber).First().Result==ExecutionResult.NotRun))throw new InvalidOperationException("Every cycle item must have a final result.");Status=TestCycleStatus.Completed;CompletedAtUtc=DateTimeOffset.UtcNow;}
 public void Cancel(){if(Status is TestCycleStatus.Completed or TestCycleStatus.Cancelled)throw new InvalidOperationException("Cycle cannot be cancelled.");Status=TestCycleStatus.Cancelled;CompletedAtUtc=DateTimeOffset.UtcNow;}
}
