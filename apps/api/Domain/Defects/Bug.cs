namespace QAHub.Api.Domain.Defects;

public sealed class Bug
{
    private static readonly Dictionary<BugStatus, BugStatus[]> Transitions = new()
    {
        [BugStatus.New] = [BugStatus.Triaged, BugStatus.Rejected, BugStatus.Duplicate, BugStatus.CannotReproduce, BugStatus.Deferred],
        [BugStatus.Triaged] = [BugStatus.Assigned, BugStatus.Rejected, BugStatus.Duplicate, BugStatus.Deferred],
        [BugStatus.Assigned] = [BugStatus.InProgress, BugStatus.Deferred],
        [BugStatus.InProgress] = [BugStatus.Fixed, BugStatus.Deferred],
        [BugStatus.Fixed] = [BugStatus.ReadyForRetest],
        [BugStatus.ReadyForRetest] = [BugStatus.Verified, BugStatus.Reopened],
        [BugStatus.Verified] = [BugStatus.Closed, BugStatus.Reopened],
        [BugStatus.Reopened] = [BugStatus.Assigned, BugStatus.InProgress],
        [BugStatus.Deferred] = [BugStatus.Triaged],
    };
    private Bug(){}
    public Bug(Guid productId,string code,string title,string description,string steps,string expected,string actual,BugSeverity severity,BugPriority priority,string reporter)
    {
        if(productId==Guid.Empty)throw new ArgumentException("Product is required.");
        ArgumentException.ThrowIfNullOrWhiteSpace(code);ArgumentException.ThrowIfNullOrWhiteSpace(title);ArgumentException.ThrowIfNullOrWhiteSpace(steps);ArgumentException.ThrowIfNullOrWhiteSpace(actual);
        Id=Guid.NewGuid();ProductId=productId;Code=code.Trim().ToUpperInvariant();Title=title.Trim();Description=description.Trim();StepsToReproduce=steps.Trim();ExpectedResult=expected.Trim();ActualResult=actual.Trim();Severity=severity;Priority=priority;Reporter=reporter.Trim();Status=BugStatus.New;CreatedAtUtc=UpdatedAtUtc=DateTimeOffset.UtcNow;
    }
    public Guid Id{get;private set;}public Guid ProductId{get;private set;}public string Code{get;private set;}=string.Empty;public string Title{get;private set;}=string.Empty;public string Description{get;private set;}=string.Empty;public string StepsToReproduce{get;private set;}=string.Empty;public string ExpectedResult{get;private set;}=string.Empty;public string ActualResult{get;private set;}=string.Empty;public BugSeverity Severity{get;private set;}public BugPriority Priority{get;private set;}public BugStatus Status{get;private set;}public string Reporter{get;private set;}=string.Empty;public string Assignee{get;private set;}=string.Empty;public Guid? FixBuildId{get;private set;}public Guid? CanonicalBugId{get;private set;}public DateTimeOffset CreatedAtUtc{get;private set;}public DateTimeOffset UpdatedAtUtc{get;private set;}public DateTimeOffset? ClosedAtUtc{get;private set;}public List<BugRunLink> RunLinks{get;private set;}=[];public List<BugStatusHistory> History{get;private set;}=[];
    public void Assign(string assignee){ArgumentException.ThrowIfNullOrWhiteSpace(assignee);Assignee=assignee.Trim();UpdatedAtUtc=DateTimeOffset.UtcNow;}
    public void TransitionTo(BugStatus next,string actor,string reason,Guid? fixBuildId=null,Guid? canonicalBugId=null)
    {
        if(!Transitions.TryGetValue(Status,out var allowed)||!allowed.Contains(next))throw new InvalidOperationException($"Cannot transition bug from {Status} to {next}.");
        if(next==BugStatus.Assigned&&string.IsNullOrWhiteSpace(Assignee))throw new InvalidOperationException("Assigned bug requires an assignee.");
        if(next==BugStatus.Fixed&&!fixBuildId.HasValue)throw new InvalidOperationException("Fixed bug requires a fix build.");
        if(next==BugStatus.Duplicate&&!canonicalBugId.HasValue)throw new InvalidOperationException("Duplicate bug requires a canonical bug.");
        if(next==BugStatus.Reopened&&string.IsNullOrWhiteSpace(reason))throw new InvalidOperationException("Reopen requires a reason and new evidence reference.");
        var previous=Status;Status=next;if(fixBuildId.HasValue)FixBuildId=fixBuildId;if(canonicalBugId.HasValue)CanonicalBugId=canonicalBugId;UpdatedAtUtc=DateTimeOffset.UtcNow;if(next==BugStatus.Closed)ClosedAtUtc=UpdatedAtUtc;History.Add(new BugStatusHistory(Id,previous,next,actor,reason));
    }
}
