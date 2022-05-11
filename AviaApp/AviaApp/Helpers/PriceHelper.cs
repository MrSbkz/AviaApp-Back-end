namespace AviaApp.Helpers;

public static class PriceHelper
{
    public static decimal GetPrice(decimal price, int pricePerCent)
    {
        return price + (price / 100 * pricePerCent);
    }
}