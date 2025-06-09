using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GoTorz.Client.Services;
using GoTorz.Client.Services.Interfaces;
using GoTorz.Shared.DTOs.Booking;
using Moq;
using Moq.Protected;
using Xunit;

public class ProfileServiceTests
{
    /// <summary>
    /// Tests the GetMyBookingsAsync method of the ProfileService.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyBookingsAsync_ShouldReturnUpcomingAndPastBookings()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var mockAuthService = new Mock<IClientAuthService>();
        var request = new HttpRequestMessage(HttpMethod.Get, "api/booking/my");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        mockAuthService
            .Setup(x => x.CreateAuthorizedRequest(HttpMethod.Get, "api/booking/my"))
            .ReturnsAsync(request);

        var allBookings = new List<BookingDto>
        {
            new BookingDto { Departure = now.AddDays(-2), Arrival = now.AddDays(-1) }, // past
            new BookingDto { Departure = now.AddDays(1), Arrival = now.AddDays(2) }    // upcoming
        };

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(allBookings)
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://localhost/") 
        };

        var service = new ProfileService(httpClient, mockAuthService.Object);

        // Act
        var (upcoming, past) = await service.GetMyBookingsAsync();

        // Assert
        Assert.Single(upcoming);
        Assert.Single(past);
        Assert.True(upcoming.All(b => b.Departure >= now));
        Assert.True(past.All(b => b.Departure < now));
    }
    /// <summary>
    /// Tests the GetMyBookingsAsync method of the ProfileService when the user is not authorized.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyBookingsAsync_ShouldReturnEmptyLists_WhenUnauthorized()
    {
        // Arrange
        var mockAuthService = new Mock<IClientAuthService>();
        mockAuthService
            .Setup(x => x.CreateAuthorizedRequest(HttpMethod.Get, "api/booking/my"))
            .ReturnsAsync((HttpRequestMessage?)null); // simulates missing token

        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(handlerMock.Object);

        var service = new ProfileService(httpClient, mockAuthService.Object);

        // Act
        var (upcoming, past) = await service.GetMyBookingsAsync();

        // Assert
        Assert.Empty(upcoming);
        Assert.Empty(past);
    }
    /// <summary>
    /// Tests the GetMyBookingsAsync method of the ProfileService when the server returns an error.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetMyBookingsAsync_ShouldReturnEmptyLists_WhenServerReturnsError()
    {
        // Arrange
        var mockAuthService = new Mock<IClientAuthService>();
        var request = new HttpRequestMessage(HttpMethod.Get, "api/booking/my");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "test-token");

        mockAuthService
            .Setup(x => x.CreateAuthorizedRequest(HttpMethod.Get, "api/booking/my"))
            .ReturnsAsync(request);

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError // simulates server error
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://localhost/")
        };

        var service = new ProfileService(httpClient, mockAuthService.Object);

        // Act
        var (upcoming, past) = await service.GetMyBookingsAsync();

        // Assert
        Assert.Empty(upcoming);
        Assert.Empty(past);
    }


}
