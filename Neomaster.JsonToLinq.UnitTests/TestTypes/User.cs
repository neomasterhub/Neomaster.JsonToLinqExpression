namespace Neomaster.JsonToLinq.UnitTests;

public record User
{
  public int Id { get; set; }
  public string Email { get; set; }
  public bool IsActive { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string MiddleName { get; set; }
  public decimal Balance { get; set; }
  public DateTime? LastVisitAt { get; set; }
}
