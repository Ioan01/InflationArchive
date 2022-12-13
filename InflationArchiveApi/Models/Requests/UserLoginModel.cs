using System.ComponentModel.DataAnnotations;

namespace InflationArchive.Models.Requests;

public class UserLoginModel
{
    [Required]
    public string LoginName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

}