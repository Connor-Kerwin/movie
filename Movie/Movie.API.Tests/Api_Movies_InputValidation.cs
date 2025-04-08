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

    [Theory(DisplayName = "Valid page returns an OK response")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1000)]
    public async Task Valid_Page_Returns_Ok_Response(int page)
    {
        var query = $"&page={page}&pageSize=1";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }

    [Theory(DisplayName = "Invalid page returns a BAD REQUEST response")]
    [InlineData(-1)]
    public async Task Invalid_Page_Returns_Bad_Request_Response(int page)
    {
        var query = $"&page={page}&pageSize=1";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
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

    [Theory(DisplayName = "Valid genres returns an OK response")]
    [InlineData("genres=action")]
    [InlineData("genres=adventure")]
    [InlineData("genres=science-fiction")]
    [InlineData("genres=crime")]
    [InlineData("genres=mystery")]
    [InlineData("genres=thriller")]
    [InlineData("genres=animation")]
    [InlineData("genres=comedy")]
    [InlineData("genres=family")]
    [InlineData("genres=fantasy")]
    [InlineData("genres=war")]
    [InlineData("genres=horror")]
    [InlineData("genres=drama")]
    [InlineData("genres=music")]
    [InlineData("genres=romance")]
    [InlineData("genres=western")]
    [InlineData("genres=tv-movie")]
    [InlineData("genres=documentary")]
    [InlineData("genres=action&genres=adventure")]
    [InlineData("genres=action&genres=adventure&genres=mystery")]
    public async Task Valid_Genres_Returns_Ok_Response(string genres)
    {
        var query = $"&page=0&pageSize=1&{genres}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }

    [Theory(DisplayName = "Invalid genres returns an BAD REQUEST response")]
    [InlineData("genres=not_a_value")]
    [InlineData("genres=sciencefiction")]
    [InlineData("genres=tvmovie")]
    [InlineData("genres=action&genres=not_a_value")]
    [InlineData("genres=action&genres=not_a_value&genres=mystery")]
    public async Task Invalid_Genres_Returns_Bad_Request_Response(string genres)
    {
        var query = $"&page=0&pageSize=1&{genres}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }

    [Theory(DisplayName = "Valid sortby returns an OK response")]
    [InlineData("sortby=title")]
    [InlineData("sortby=release-date")]
    public async Task Valid_SortBy_Returns_Ok_Response(string sortby)
    {
        var query = $"&page=0&pageSize=1&{sortby}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }

    [Theory(DisplayName = "Invalid sortby returns an BAD REQUEST response")]
    [InlineData("sortby=not_a_value")]
    public async Task Invalid_SortBy_Returns_Bad_Request_Response(string sortby)
    {
        var query = $"&page=0&pageSize=1&{sortby}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.BadRequest, status);
    }

    [Theory(DisplayName = "Valid orderby returns an OK response")]
    [InlineData("orderby=asc")]
    [InlineData("orderby=desc")]
    public async Task Valid_OrderBy_Returns_Ok_Response(string orderby)
    {
        var query = $"&page=0&pageSize=1&{orderby}";
        var address = BuildSearchApiAddress(query);
        var request = new HttpRequestMessage(HttpMethod.Get, address);
        using var response = await client.SendAsync(request);

        var status = response.StatusCode;
        Assert.Equal(HttpStatusCode.OK, status);
    }

    [Theory(DisplayName = "Invalid orderby returns an BAD REQUEST response")]
    [InlineData("orderby=not_a_value")]
    public async Task Invalid_OrderBy_Returns_Bad_Request_Response(string orderby)
    {
        var query = $"&page=0&pageSize=1&{orderby}";
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
}