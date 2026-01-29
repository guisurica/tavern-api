using tavern_api.Commons;
using tavern_api.Commons.Exceptions;
namespace tavern_api.Entities;

public class Post : BaseEntity
{
    private const int MinTitleLength = 3;
    private const int MaxTitleLength = 200;
    private const int MinContentLength = 10;
    private const int MaxContentLength = 10000;
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };

    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public string? PostImageUrl { get; private set; } = null;
    public Membership Membership { get; private set; }
    public string MembershipId { get; private set; }
    public List<Like> Likes { get; private set; } = new List<Like>();
    public List<Comment> Comments { get; private set; } = new List<Comment>();

    private Post() { }

    private Post(string title, 
        string content,
        string membershipId)
    {
        Title = title;
        Content = content;
        MembershipId = membershipId;
    }

    public static Post Create(string title, string content, string membershipId)
    {
        VerifyPostTitle(title);
        VerifyPostContent(content);
        return new Post(title, content, membershipId);
    }

    private static void VerifyPostTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título do post não pode estar vazio.");

        if (title.Length < MinTitleLength)
            throw new DomainException($"O título deve ter no mínimo {MinTitleLength} caracteres.");

        if (title.Length > MaxTitleLength)
            throw new DomainException($"O título deve ter no máximo {MaxTitleLength} caracteres.");
    }

    private static void VerifyPostContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("O conteúdo do post não pode estar vazio.");

        if (content.Length < MinContentLength)
            throw new DomainException($"O conteúdo deve ter no mínimo {MinContentLength} caracteres.");

        if (content.Length > MaxContentLength)
            throw new DomainException($"O conteúdo deve ter no máximo {MaxContentLength} caracteres.");
    }

    private static void VerifyPostImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
            throw new DomainException("A URL da imagem é inválida.");

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new DomainException("A URL da imagem deve usar protocolo HTTP ou HTTPS.");

        var extension = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
            throw new DomainException($"Formato de imagem não suportado. Use: {string.Join(", ", AllowedImageExtensions)}");
    }

    public void UpdateTitle(string newTitle)
    {
        VerifyPostTitle(newTitle);
        Title = newTitle;
    }

    public void UpdateContent(string newContent)
    {
        VerifyPostContent(newContent);
        Content = newContent;
    }

    public void UpdateImage(string? newImageUrl)
    {
        VerifyPostImage(newImageUrl);
        PostImageUrl = newImageUrl;
    }
}