using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "The First Name field is required"),MaxLength(100)]
        public string FirstName { get; set; } = "";
        [Required(ErrorMessage = "The Last Name field is required"), MaxLength(100)]
        public string LastName { get; set; } = "";
        [Required,EmailAddress,MaxLength(100)]
        public string Email { get; set; } = "";
        [Phone(ErrorMessage ="The format of phone number is not valid."),MaxLength(10)]
        public string? PhoneNumber { get; set; }
        [Required,MaxLength(200)]
        public string Address { get; set; } = "";
        [Required,MaxLength(200)]
        public string Password { get; set; } = "";
        [Required(ErrorMessage = "The confirm password is required.")]
        [Compare("Password",ErrorMessage ="Password and confirm password do not match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
