using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Models.Products;


public class Store : ScraperEntity
{
    
    public string? Website { get; set; }
    
    public IEnumerable<Product> Products { get; set; }
}