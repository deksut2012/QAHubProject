using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
namespace QAHub.Api.Tests;
public sealed class AuthorizationEndpointsTests
{
 [Fact] public async Task RequirementsRejectAnonymousUserOutsideDevelopment(){await using var factory=new WebApplicationFactory<Program>().WithWebHostBuilder(builder=>{builder.UseEnvironment("Testing");builder.ConfigureAppConfiguration((_,config)=>config.AddInMemoryCollection(new Dictionary<string,string?>{{"Authentication:Authority","https://identity.example.test"},{"Authentication:Audience","qahub-api"}}));});using var client=factory.CreateClient();var response=await client.GetAsync("/api/v1/requirements");Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);}
}
