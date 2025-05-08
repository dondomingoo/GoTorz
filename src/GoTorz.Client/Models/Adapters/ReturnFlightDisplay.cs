using GoTorz.Client.Models.UIContracts;
using GoTorz.Shared.DTOs.Travelplanner;

namespace GoTorz.Client.Models.Adapters
{
    /// <summary>
    /// Adapter for return flight DTO used in UI components.
    /// </summary>
    public class ReturnFlightDisplay : ReturnFlightDto, IFlightInfo { }
}
