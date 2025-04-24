using GoTorz.Shared.DTOs;
using GoTorz.Client.Models.UIContracts;

namespace GoTorz.Client.Models.Adapters
{
    /// <summary>
    /// Adapter for return flight DTO used in UI components.
    /// </summary>
    public class ReturnFlightDisplay : ReturnFlightDto, IFlightInfo { }
}
