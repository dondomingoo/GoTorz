using GoTorz.Api.Services;
using GoTorz.Shared.DTOs.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoTorz.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Step 1: Select a travel package and initiate a booking session.
        /// </summary>
        [HttpGet("initiate/{packageId}")]
        public async Task<IActionResult> InitiateBooking(string packageId)
        {
            _logger.LogInformation("Initiating booking for packageId: {$PackageId}", packageId);
            var summary = await _bookingService.InitiateBookingAsync(packageId);
            if (summary != null)
            {
                _logger.LogInformation("Booking initaion succesful for packageId: {$PackageId}", packageId);
                return Ok (summary);
            }
            else
            {
                _logger.LogWarning("Package not available for packageId {$PackageId}", packageId);
                return NotFound("Package not available");
            }
        }

        /// <summary>
        /// Step 2: Submit customer information and select a payment method.
        /// </summary>
        [HttpPost("submit-info")]
        public async Task<IActionResult> SubmitCustomerInfo([FromBody] BookingRequestDto bookingRequest)
        {
            var response = await _bookingService.SubmitCustomerInfoAsync(bookingRequest);
            if (response.Success)
            {
                _logger.LogInformation("Customer info submission succeeded for TravelPackageId: {PackageId}", bookingRequest.PackageId);
                return Ok (response);
            }
            else
            {
                _logger.LogWarning("Customer info submission failed for TravelPackageId: {PackageId}. Reason: {Reason}", bookingRequest.PackageId, response.Message);
                return BadRequest(response.Message);
            }
        }

        /// <summary>
        /// Step 3: Handle payment result (success/failure).
        /// </summary>
        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentResultDto payment)
        {
            var result = await _bookingService.ConfirmPaymentAsync(payment);
            if (result.Success)
            {
                _logger.LogInformation("Payment for BookingId {BookingId} is confirmed", payment.BookingId);
                return Ok (result);
            }
            else
            {
                _logger.LogWarning("Payment for BookingId {BookingId} failed. Reason: {Reason}", payment.BookingId, result.Message);
                return StatusCode(402, result.Message); 
            }
        }

        /// <summary>
        /// Retry payment with a new method.
        /// </summary>
        [HttpPost("retry-payment")]
        public async Task<IActionResult> RetryPayment([FromBody] RetryPaymentDto retry)
        {
            _logger.LogInformation("RetryPayment called for BookingId: {BookingId}", retry.BookingId);
            
            var result = await _bookingService.RetryPaymentAsync(retry);
            if (result.Success)
            {
                _logger.LogInformation("Retry payment succeeded for BookingId: {BookingId}", retry.BookingId);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Retry payment failed for BookingId: {BookingId}. Reason: {Reason}", retry.BookingId, result.Message);
                return BadRequest(result.Message);
            }
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
            _logger.LogInformation("GetAllBookings was called with: userId={UserId}, " +
                "bookingId={BookingId}, arrivalDate={Arrival}, " +
                "departureDate={Departure}, orderDate={OrderDate}",
                userId, bookingId, arrivalDate, departureDate, orderDate);

            var bookings = await _bookingService.GetAllBookingsAsync(
                userId, bookingId, arrivalDate, departureDate, orderDate, email);
            return Ok(bookings);
        }



        /// <summary>
        /// Delete a booking by ID.
        /// </summary>
        [Authorize(Roles = "Admin,SalesRep")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBooking(string id)
        {
            var result = await _bookingService.CancelBookingAsync(id);
            if(result)
            {
                _logger.LogInformation("Booking with bookingId {BookingId} has been deleted", id);
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Deletion of BookingId {BookingId} failed, booking could not be found or could not be cancelled.", id);
                return NotFound(result);
            }
        }
    }
}