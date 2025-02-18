namespace ProjektBackend.Controllers
{
    // Post
    public record CreatePostDto(string Title, string Content);

    public record UpdatePostDto(string Title, string Content);

    // User

    public record RegisterUserDto(string FirstName, string LastName, string Email, string Password);

    public record LoginUserDto(string Email, string Password);
}
