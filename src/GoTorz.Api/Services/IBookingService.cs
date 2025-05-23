﻿using GoTorz.Api.Data;
using GoTorz.Shared.DTOs.Booking;
using GoTorz.Shared.Models;

namespace GoTorz.Api.Services
{
    public interface IBookingService
    {
        Task<TravelPackage?> InitiateBookingAsync(string packageId);
        Task<BookingResponseDto> SubmitCustomerInfoAsync(BookingRequestDto request);
        Task<PaymentResponseDto> ConfirmPaymentAsync(PaymentResultDto payment);
        Task<PaymentResponseDto> RetryPaymentAsync(RetryPaymentDto retry);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync(
    string? userId = null,
    string? bookingId = null,
    DateTime? arrivalDate = null,
    DateTime? departureDate = null,
    DateTime? orderDate = null,
    string? email = null
);

        Task<bool> CancelBookingAsync(string bookingId);
        Task<bool> HasUpcomingBookingsAsync(string userId);
    }
}