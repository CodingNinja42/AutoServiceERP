
namespace CustomerServices.Domain.Entities;

public class Customer
{
    // Unique identifier for each customer profile.
    public Guid Id { get; set; } = Guid.NewGuid();

    // ID of the corresponding user in AuthService.
    public Guid AuthUserId { get; set; }

    // Customer's personal details.
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    // Customer's address details.
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    // Record creation and modification timestamps.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Indicates whether the customer profile is active.
    public bool IsActive { get; set; } = true;
}
