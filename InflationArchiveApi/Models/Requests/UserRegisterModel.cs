using System.ComponentModel.DataAnnotations;

namespace InflationArchive.Models.Requests;

public class UserRegisterModel
{
    [Required]
    [MinLength(5)]
    [MaxLength(15)]
    [RegularExpression(@"^[a-zA-Z0-9_-]*", ErrorMessage =
        "Only alphanumeric characters and -_ characters are allowed")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]

    public string Email { get; set; }

    [Required]
    [MinLength(5)]
    [MaxLength(15)]
    [RegularExpression(@"^[a-zA-Z0-9_-]*", ErrorMessage =
        "Only alphanumeric characters and -_ characters are allowed")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }
}