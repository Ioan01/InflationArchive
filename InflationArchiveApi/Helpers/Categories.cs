namespace InflationArchive.Helpers;

public static class Categories
{
    static Categories()
    {
        MegaImageCategories = new Dictionary<string, ISet<string>>
        {
            ["Fructe/Legume"] = new HashSet<string> { "001" },
            ["Lactate/Oua"] = new HashSet<string> { "002" },
            ["Mezeluri"] = new HashSet<string> { "003001" },
            ["Carne"] = new HashSet<string> { "003002" },
            ["Peste si icre"] = new HashSet<string> { "003003" },
            ["Semipreparate"] = new HashSet<string> { "003004" },
            ["Produse congelate"] = new HashSet<string> { "004" },
            ["Cafea/Mic dejun"] = new HashSet<string> { "005001", "005002", "005005", "005006", "005007" },
            ["Paine/Patiserie"] = new HashSet<string> { "005004" },
            ["Zahar/Faina/Malai"] = new HashSet<string> { "007001" },
            ["Orez/Legume uscate"] = new HashSet<string> { "007002" },
            ["Paste"] = new HashSet<string> { "007004" },
            ["Condimente"] = new HashSet<string> { "007005" },
            ["Conserve"] = new HashSet<string> { "007007", "007008", "007009", "007010", "007011" },
            ["Apa/Sucuri"] = new HashSet<string> { "008" },
            ["Alcool/Tutun"] = new HashSet<string> { "009" }
        };
    }

    public static IDictionary<string, ISet<string>> MegaImageCategories { get; }
}