namespace QAHub.Api.Domain.Defects;
public sealed class BugRunLink { private BugRunLink(){} public BugRunLink(Guid bugId,Guid attemptId){Id=Guid.NewGuid();BugId=bugId;TestRunAttemptId=attemptId;}public Guid Id{get;private set;}public Guid BugId{get;private set;}public Guid TestRunAttemptId{get;private set;} }
