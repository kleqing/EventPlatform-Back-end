using System.ComponentModel.DataAnnotations;

namespace EventPlatform.Application.Contracts.Requests;

public class UpdateUserProfileRequest
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    [StringLength(250)]
    public string? AddressStreet { get; set; }

    [StringLength(150)]
    public string? AddressWard { get; set; }

    [StringLength(150)]
    public string? AddressDistrict { get; set; }

    [StringLength(150)]
    public string? AddressCity { get; set; }

    [StringLength(50)]
    public string? DateOfBirth { get; set; }
}
