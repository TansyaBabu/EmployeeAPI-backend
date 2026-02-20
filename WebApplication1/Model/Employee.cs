using System.ComponentModel.DataAnnotations.Schema;

public class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Designation { get; set; }
    public string Address { get; set; }
    public string Department { get; set; }
    public string Skillset { get; set; }
    public string Role { get; set; }

    public string? Status { get; set; }   // 👈 FIXED
    public string CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    public DateTime? JoiningDate { get; set; }

    [NotMapped]
    public string? ProfileImage { get; set; }

    public byte[]? ProfileImageBytes { get; set; }   // 👈 MUST BE NULLABLE
}
