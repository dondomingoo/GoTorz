namespace GoTorz.Client.Services.Helpers
{
    /// <summary>
    /// Provides logic to calculate total travel package price, including optional markup.
    /// </summary>
    public class TravelPriceCalculator
    {
        /// <summary>
        /// Calculates the total price for a travel package.
        /// </summary>
        /// <param name="flightPrice">Base price for selected flights (EUR).</param>
        /// <param name="hotelPrice">Base price for selected hotel (EUR).</param>
        /// <param name="markupPercentage">Markup percentage to apply (e.g. 20 for 20%).</param>
        /// <returns>Total price including markup.</returns>
        public decimal CalculateTotal(decimal flightPrice, decimal hotelPrice, decimal markupPercentage)
        {
            return (flightPrice + hotelPrice) * (1 + markupPercentage / 100);
        }
    }
}
