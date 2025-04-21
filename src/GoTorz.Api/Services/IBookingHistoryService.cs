namespace GoTorz.Api.Services
{
    public interface IBookingHistoryService
    {
        /// <summary>
        /// Retrieves the booking history for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose booking history is to be retrieved.</param>
        /// <returns>A list of booking history records for the specified user.</returns>
        Task<IEnumerable<BookingHistoryDto>> GetBookingHistoryAsync(string userId);
        /// <summary>
        /// Retrieves the details of a specific booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to retrieve.</param>
        /// <returns>The details of the specified booking.</returns>
        Task<BookingHistoryDto> GetBookingDetailsAsync(string bookingId);
    }
}
