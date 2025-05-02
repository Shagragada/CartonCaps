using CartonCaps.Enums;

namespace CartonCaps.Models;

public class Referral
{
    public int Id { get; set; }
    public int ReferredId { get; set; }
    public int ReferrerId { get; set; }
    public ReferralStatus Status { get; set; }
    public DateTime CompletedDate { get; set; }
}
