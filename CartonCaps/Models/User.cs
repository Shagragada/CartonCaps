namespace CartonCaps.Models;

public class User
{
    public int Id { get; set; }
    public string ReferralCode { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Zipcode { get; set; } = null!;
}
