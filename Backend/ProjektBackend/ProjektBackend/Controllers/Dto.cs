namespace ProjektBackend.Controllers
{

    public record CreatePostDto(string Title, string Content);

    public record UpdatePostDto(string Title, string Content);

}
