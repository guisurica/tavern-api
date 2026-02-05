using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface IPostService
{
    Task<Result<PostDTO>> CreatePostAsync(CreatePostDTO input, string userId);
    Task<Result<List<PostDTO>>> GetPostsAsync(string tavernId, string userId);
    Task<Result<string>> LikePostAsync(LikePostDTO input, string userId);
    Task<Result<List<CommentDTO>>> CreateCommentAsync(CreateCommentDTO input, string userId);
    Task<Result<PostDTO>> GetPostDetailsAsync(string tavernId, string postId, string userId);
}
