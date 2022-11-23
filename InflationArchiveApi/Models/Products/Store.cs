using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InflationArchive.Models.Products;

[Table("user")]
[Index(nameof(Name),IsUnique=true)]
public class Store
{
    
    public int Id { get; set; }
    public string Name { get; set; }
    public Uri Website { get; set; }
    
    
}