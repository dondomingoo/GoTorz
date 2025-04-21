using GoTorz.Api.Services;
using GoTorz.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Step 1: Select a travel package and initiate a booking session.
        /// </summary>
        [HttpGet("initiate/{packageId}")]
        public async Task<IActionResult> InitiateBooking(string packageId)
        {
            var summary = await _bookingService.InitiateBookingAsync(packageId);
            return summary != null ? Ok(summary) : NotFound("Package not available.");
        }

        /// <summary>
        /// Step 2: Submit customer information and select a payment method.
        /// </summary>
        [HttpPost("submit-info")]
        public async Task<IActionResult> SubmitCustomerInfo([FromBody] BookingRequestDto bookingRequest)
        {
            var response = await _bookingService.SubmitCustomerInfoAsync(bookingRequest);
            return response.Success ? Ok(response) : BadRequest(response.Message);
        }

        /// <summary>
        /// Step 3: Handle payment result (success/failure).
        /// </summary>
        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentResultDto payment)
        {
            var result = await _bookingService.ConfirmPaymentAsync(payment);
            return result.Success ? Ok(result) : StatusCode(402, result.Message);
        }

        /// <summary>
        /// Retry payment with a new method.
        /// </summary>
        [HttpPost("retry-payment")]
        public async Task<IActionResult> RetryPayment([FromBody] RetryPaymentDto retry)
        {
            var result = await _bookingService.RetryPaymentAsync(retry);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
        /// <summary>
        /// get all bookings
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings(
    [FromQuery] string? userId,
    [FromQuery] string? bookingId,
    [FromQuery] DateTime? arrivalDate,
    [FromQuery] DateTime? departureDate,
    [FromQuery] DateTime? orderDate,
    [FromQuery] string? email)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(
                userId, bookingId, arrivalDate, departureDate, orderDate, email);
            return Ok(bookings);
        }



        /// <summary>
        /// Delete a booking by ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            return result ? Ok() : NotFound("Booking not found or could not be cancelled.");
        }
    }
}