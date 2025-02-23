namespace Business.Models.Dtos.Update;

public record UserUpdateForm(int Id, string FirstName, string LastName, string Email, string PhoneNumber);
