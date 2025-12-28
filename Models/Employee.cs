using System.ComponentModel.DataAnnotations;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Date of Birth is required")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Country is required")]
    public string Country { get; set; }

    public string? ProfileImage { get; set; }
}
