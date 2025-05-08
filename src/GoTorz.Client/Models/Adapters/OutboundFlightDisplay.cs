using GoTorz.Client.Models.UIContracts;
using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Client.Models.Adapters
{
    /// <summary>
    /// Adapter for outbound flight DTO used in UI components.
    /// </summary>
    public class OutboundFlightDisplay : OutboundFlightDto, IFlightInfo { }
}