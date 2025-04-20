using GoTorz.Shared.Models;
using GoTorz.Client.Models.UIContracts;

namespace GoTorz.Client.Models.Adapters
{
    /// <summary>
    /// Adapter for outbound flight model used in UI components.
    /// </summary>
    public class OutboundFlightModelDisplay : OutboundFlight, IFlightInfo { }
}