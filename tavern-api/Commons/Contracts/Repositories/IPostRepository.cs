using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Commons.Contracts.Repositories;

public interface IPostRepository : IBaseRepository<Post>
{
    Task<List<PostDTO>> GetAllTavernPosts(string tavernId);
    Task LikePost(Like newLike);
    Task UnlikePost(string likeId);

    Task<Post> GetPostByIdAsync(string postId);
    Task<Comment> GetCommentById(string commentId);
    Task<Comment> CreateCommentAsync(Comment entity);
    Task<List<CommentDTO>> GetAllPostComments(string postId);
    Task<List<CommentDTO>> GetCommentRepliesAsync(string commentId);
} 