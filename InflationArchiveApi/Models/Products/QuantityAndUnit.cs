using System.Text.RegularExpressions;

namespace InflationArchive.Models.Products;


public class QuantityAndUnit {
    private static  Regex quantityRegex = new Regex
        ("[0-9]+[.,]?[0-9]*",RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static Regex quantityAndUnitRegex = new Regex
        ("([0-9]+[.,]?[0-9]* *)(([mk]?[gl])|bucati)",RegexOptions.Multiline|RegexOptions.Compiled | RegexOptions.IgnoreCase);



    private double quantity;
    private string unit;


    public QuantityAndUnit(double quantity, string unit)
    {
        this.quantity = quantity;
        this.unit = unit;
    }

    public static QuantityAndUnit getPriceAndUnit(String productName)
    {
        var match = quantityAndUnitRegex.Match(productName);

        if (match.Groups.Count == 0)
            return new QuantityAndUnit(1,"piece");
        if (match.Groups.Count > 1)
        {
        }
        

        double quantity = Double.Parse(match.Groups[1].Value.Replace(',','.'));

        string unit = match.Groups[2].Value.ToLower();

        switch (unit)
        {
            case "mg":
                quantity /=  1000 * 1000;
                break;
            case "g" :
                quantity /= 1000;
                break;
            case "ml":
                quantity /= 1000;
                break;
        }


        if (unit.Contains("g"))
            unit = "Kg";
        else if (unit.Contains("l")) {
            unit = "L";
        }
        else unit = "piece";



        return new QuantityAndUnit(quantity,unit);
    }

    

}
