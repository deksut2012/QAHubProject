using QAHub.Api.Domain.Toolbox;
namespace QAHub.Api.Tests;
public sealed class SavedSqlQueryDomainTests
{
 [Fact]public void QueryTrimsFieldsAndCapturesOwner(){var x=new SavedSqlQuery(null," Active products "," SELECT * FROM Products "," qa-user ");Assert.Equal("Active products",x.Name);Assert.Equal("SELECT * FROM Products",x.Statement);Assert.Equal("qa-user",x.CreatedBy);}
 [Theory][InlineData("")][InlineData(" ")]public void QueryRequiresName(string name)=>Assert.Throws<ArgumentException>(()=>new SavedSqlQuery(null,name,"SELECT 1","qa"));
}
