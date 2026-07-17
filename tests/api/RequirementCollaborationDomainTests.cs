using QAHub.Api.Domain.Requirements;
namespace QAHub.Api.Tests;
public sealed class RequirementCollaborationDomainTests
{
 [Fact] public void CommentTrimsBodyAndCapturesAuthor(){var x=new RequirementComment(Guid.NewGuid(),"qa-user","  reviewed  ");Assert.Equal("reviewed",x.Body);Assert.Equal("qa-user",x.AuthorId);}
 [Fact] public void AttachmentCapturesMetadataAndContent(){var content=new byte[]{1,2,3};var x=new RequirementAttachment(Guid.NewGuid(),"evidence.txt","text/plain",content,"qa-user");Assert.Equal(3,x.SizeBytes);Assert.Equal(content,x.Content);Assert.Equal("evidence.txt",x.FileName);}
}
