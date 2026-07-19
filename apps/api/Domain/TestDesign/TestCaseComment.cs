namespace QAHub.Api.Domain.TestDesign;

public sealed class TestCaseComment
{
    private TestCaseComment() { }
    public TestCaseComment(Guid testCaseId, string authorId, string body)
    {
        Id = Guid.NewGuid();
        TestCaseId = testCaseId;
        AuthorId = authorId;
        Body = body.Trim();
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid TestCaseId { get; private set; }
    public string AuthorId { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
}
