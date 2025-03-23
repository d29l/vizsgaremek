using System.ComponentModel.DataAnnotations;

namespace ProjektBackend.Controllers
{
    // Post
    public record CreatePostDto
        (
        string Title, 
        string Content
        );

    public record UpdatePostDto
        (
        string Title,
        string Content
        );

    // User

    public record RegisterUserDto
        (
        string FirstName,
        string LastName,
        string Email,
        string Password
        );

    public record LoginUserDto
        (
        string Email, 
        string Password
        );

    public record UpdateUserDto
        (
        string FirstName,
        string LastName,
        string Email
        );

    public record CreateProfileDto
        (
        string Headline, 
        string Bio, 
        string Location, 
        string ProfilePicture
        );
    public record UpdateProfileDto
        (
        string Headline,
        string Bio,
        string Location,
        string ProfilePicture
        );

    public record CreateEmployerDto
        (
        string CompanyName,
        string CompanyAddress,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );
    public record UpdateEmployerDto
        (
        string CompanyName,
        string CompanyAddress,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );

    public record CreateRequestDto
        (
        string CompanyName,
        string CompanyAddress,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );
    public record UpdateRequestDto
        (
        string CompanyName,
        string CompanyAddress,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );

    public record RefreshTokenDto
        (
        string RefreshToken
        );

    public record CreateConnectionDto
        (
        int ReceiverId
        );
    public record UpdateConnectionDto
        (
        string Status
        );

    public record ChangePasswordDto
    (
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword
    );
}
