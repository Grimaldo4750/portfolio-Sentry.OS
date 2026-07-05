using Sentry.OS.Persistence.Envelope;

namespace Sentry.OS.Admin.API.Tests.Contract;

public class ApiResponseEnvelopeTests
{
    [Fact]
    public void Success_SetsSuccessResponseCodeAndData()
    {
        var response = ApiResponse<string>.Success("payload");

        Assert.Equal(ResponseCode.Success, response.ResponseCode);
        Assert.Equal("payload", response.Data);
        Assert.False(string.IsNullOrWhiteSpace(response.ResponseMessage));
    }

    [Theory]
    [InlineData(ResponseCode.ValidationError)]
    [InlineData(ResponseCode.Unauthorized)]
    [InlineData(ResponseCode.Forbidden)]
    [InlineData(ResponseCode.NotFound)]
    [InlineData(ResponseCode.Conflict)]
    [InlineData(ResponseCode.InternalServerError)]
    public void Failure_CarriesRequestedResponseCodeAndNoData(ResponseCode code)
    {
        var response = ApiResponse.Failure(code, "something went wrong");

        Assert.Equal(code, response.ResponseCode);
        Assert.Null(response.Data);
        Assert.Equal("something went wrong", response.ResponseMessage);
    }
}
