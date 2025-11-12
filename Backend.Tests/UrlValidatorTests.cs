namespace Backend.Tests;

public class UrlValidatorTest
{
    [Theory]
    [InlineData("http://example.com")]
    [InlineData("https://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://sub.domain.example.com/path?query=1#fragment")]
    [InlineData("http://example.com/")]
    [InlineData("https://example.co.uk")]
    public void IsValidUrl_ValidUrls_ReturnsTrue(string url)
    {
        bool result = UrlValidator.IsValidUrl(url);
        Assert.True(result);
    }

    [Theory]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    [InlineData("http:/example.com")]
    [InlineData("https//example.com")]
    [InlineData("https://example")]
    [InlineData("https://example.c")]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData("https://example.com,")]
    [InlineData("https://example.com.")]
    public void IsValidUrl_InvalidUrls_ReturnsFalse(string url)
    {
        bool result = UrlValidator.IsValidUrl(url);
        Assert.False(result);
    }
}
