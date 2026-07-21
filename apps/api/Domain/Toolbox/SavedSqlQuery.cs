namespace QAHub.Api.Domain.Toolbox;
public sealed class SavedSqlQuery
{
 private SavedSqlQuery(){}
 public SavedSqlQuery(Guid? productId,string name,string statement,string createdBy){ArgumentException.ThrowIfNullOrWhiteSpace(name);ArgumentException.ThrowIfNullOrWhiteSpace(statement);ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);Id=Guid.NewGuid();ProductId=productId;Name=name.Trim();Statement=statement.Trim();CreatedBy=createdBy.Trim();CreatedAtUtc=DateTimeOffset.UtcNow;}
 public Guid Id{get;private set;}public Guid? ProductId{get;private set;}public string Name{get;private set;}=string.Empty;public string Statement{get;private set;}=string.Empty;public string CreatedBy{get;private set;}=string.Empty;public DateTimeOffset CreatedAtUtc{get;private set;}
}
