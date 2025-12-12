using System.ComponentModel.DataAnnotations;
using WatchAPI.Constants;

namespace WatchAPI.DTOs;

public class RegisterDTO
{
    [Required]
    [StringLength(AuthConstants.MaxUserNameLength, MinimumLength = AuthConstants.MinUserNameLength)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(AuthConstants.MaxPasswordLength, MinimumLength = AuthConstants.MinPasswordLength)]
    [RegularExpression(AuthConstants.PasswordRegex)]
    public string Password { get; set; }
}
