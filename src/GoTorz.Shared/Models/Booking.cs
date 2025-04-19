using System;
using System.Collections.Generic;

namespace GoTorz.Shared.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Use 'Id' to match EF Core convention

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string UserId { get; set; }

        public string TravelPackageId { get; set; } = string.Empty;
        public TravelPackage? TravelPackage { get; set; } // Navigation property

        public string PaymentStatus { get; set; } = "Pending"; // Could be "Pending", "Confirmed", "Failed"

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<Traveller> Travellers { get; set; } = new();
    }
}