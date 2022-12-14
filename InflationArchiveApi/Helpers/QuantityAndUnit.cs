using System.Text.RegularExpressions;

namespace InflationArchive.Helpers;

public class QuantityAndUnit
{
    private static readonly Regex quantityRegex = new("[0-9]+[.,]?[0-9]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex quantityAndUnitRegex = new("([0-9]+[.,]?[0-9]* *)(([mk]?[gl])|bucati)", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex multiplicationRegex = new(
        "([0-9]+[.,]?[0-9]?) *[gl]?x *(([0-9]+[.,]?[0-9]* *))",
        RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);


    public double Quantity { get; set; }
    public string Unit { get; set; }


    public QuantityAndUnit(double quantity, string unit)
    {
        Quantity = quantity;
        Unit = unit;
    }

    public static QuantityAndUnit getPriceAndUnit(ref string productName)
    {
        // for example 6x200g
        var multiplicationMatch = multiplicationRegex.Match(productName);

        if (multiplicationMatch.Success)
        {
            productName = productName.Replace(multiplicationMatch.Value, (double.Parse(multiplicationMatch.Groups[1].Value)
                * double.Parse(multiplicationMatch.Groups[2].Value)).ToString());

        }


        var match = quantityAndUnitRegex.Match(productName);

        if (!match.Success)
            return new QuantityAndUnit(1, "piece");

        double quantity = double.Parse(match.Groups[1].Value.Replace(',', '.'));

        string unit = match.Groups[2].Value.ToLower();

        switch (unit)
        {
            case "mg":
                quantity /= 1000 * 1000;
                break;
            case "g":
                quantity /= 1000;
                break;
            case "ml":
                quantity /= 1000;
                break;
        }


        if (unit.Contains("g"))
            unit = "Kg";
        else if (unit.Contains("l"))
        {
            unit = "L";
        }
        else unit = "piece";



        return new QuantityAndUnit(quantity, unit);
    }



}