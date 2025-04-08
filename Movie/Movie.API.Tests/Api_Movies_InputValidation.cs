using System.Net;

namespace Movie.API.Tests;

public class Api_Movies_InputValidation
{
    private static Uri serverAddress = new Uri("http://localhost:5000/");
    private static HttpClient client = new HttpClient();

    private static Uri BuildSearchApiAddress(string query)
    {
        return new Uri(serverAddress, $"api/movies?{query}");
    }

    [Theory(DisplayName = "Valid page size returns an OK response")]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(64)]
    public async Task Valid_PageSize_Returns_Ok_Response(int pageSize)
    {
        var query = $"&page=0&pageSize={pageSize}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Theory(DisplayName = "Invalid page size returns a BAD REQUEST response")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public async Task Invalid_PageSize_Returns_Bad_Request_Response(int pageSize)
    {
        var query = $"&page=0&pageSize={pageSize}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "No query parameters returns bad request response")]
    [Trait("Feature", "API")]
    public async Task No_Query_Parameter_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Only page related query parameters returns ok response")]
    [Trait("Feature", "API")]
    public async Task Only_Page_Related_Query_Parameters_Returns_Ok_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Fact(DisplayName = "Invalid genre returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Invalid_Genre_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&genres=not_a_value");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Valid and invalid genre returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Valid_And_Invalid_Genre_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&genres=action&genres=not_a_value");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Valid genre returns ok response")]
    [Trait("Feature", "API")]
    public async Task Valid_Genre_Returns_Ok_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&genres=action");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Fact(DisplayName = "Multiple valid genres returns ok response")]
    [Trait("Feature", "API")]
    public async Task Multiple_Valid_Genres_Returns_Ok_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&genres=action&genres=adventure");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Fact(DisplayName = "Invalid sortby parameter returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Invalid_SortBy_Parameter_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&sortby=not_a_value");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Valid sortby parameter returns ok response")]
    [Trait("Feature", "API")]
    public async Task Valid_SortBy_Parameter_Returns_Ok_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&sortby=title");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Fact(DisplayName = "Invalid orderby parameter returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Invalid_OrderBy_Parameter_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&orderby=not_a_value");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Valid orderby parameter returns ok response")]
    [Trait("Feature", "API")]
    public async Task Valid_OrderBy_Parameter_Returns_Ok_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=1&orderby=asc");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }
    
    [Fact(DisplayName = "Zero pagesize returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Zero_PageSize_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=0");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Negative pagesize returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Negative_PageSize_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=0&pageSize=-1");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
    
    [Fact(DisplayName = "Negative page returns bad request response")]
    [Trait("Feature", "API")]
    public async Task Negative_Page_Returns_Bad_Request_Response()
    {
        var address = BuildSearchApiAddress("page=-1&pageSize=1");
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);
        
        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }
}