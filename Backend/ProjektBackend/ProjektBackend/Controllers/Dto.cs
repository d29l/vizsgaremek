using System.ComponentModel.DataAnnotations;

namespace ProjektBackend.Controllers
{
    // Post
    public record CreatePostDto
        (
        string Title,
        string Content,
        string Category,
        string Location
        );

    public record UpdatePostDto
        (
        string Title,
        string Content,
        string Category,
        string Location
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

    public record DeleteUserRequestDto
        (
        string Password
        );

    public record CreateProfileDto
        (
        IFormFile? Banner,
        string? Bio,
        string? Location,
        IFormFile? ProfilePicture
        );

    public record UpdateProfileDto
        (
        IFormFile? Banner,
        string? Bio,
        string? Location,
        IFormFile? ProfilePicture
        );

    public record CreateEmployerDto
        (
        string CompanyName,
        string CompanyAddress,
        string CompanyEmail,
        int CompanyPhoneNumber,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );
    public record UpdateEmployerDto
        (
        string CompanyName,
        string CompanyAddress,
        string CompanyEmail,
        int? CompanyPhoneNumber,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );

    public record CreateRequestDto
        (
        string CompanyName,
        string CompanyAddress,
        string CompanyEmail,
        int CompanyPhoneNumber,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );
    public record UpdateRequestDto
        (
        string CompanyName,
        string CompanyAddress,
        string CompanyEmail,
        int CompanyPhoneNumber,
        string Industry,
        string CompanyWebsite,
        string CompanyDescription,
        int EstabilishedYear
        );

    public record RefreshTokenDto
        (
        string RefreshToken
        );

    public record LogoutRequestDto
        (
        string RefreshToken
        );

    public record RevokeTokenRequestDto
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

    public record CreateMessageDto
        (
        int ReceiverId,
        string Content
        );
}
