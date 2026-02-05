using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using tavern_api.Commons;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Database;
using tavern_api.Entities;

namespace tavern_api.Repositories;

internal sealed class PostRepository : BaseRepository<Post>, IPostRepository
{
    private readonly TavernDbContext _context;

    public PostRepository(TavernDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Comment> CreateCommentAsync(Comment entity)
    {
        try
        {
            var comment = await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return comment.Entity;
        } catch(Exception ex)
        {
            throw new InfrastructureException();
        }
    }

    public async Task<List<CommentDTO>> GetAllPostComments(string postId)
    {
        try
        {
            var comments = await _context
                .Comments
                .Where(c => c.PostId == postId && !c.IsDeleted)
                    .Include(c => c.Membership)
                        .ThenInclude(c => c.User)
                 .ToListAsync();

            return comments.Select(c => new CommentDTO
            {
                CommentContent = c.CommentContent,
                CommentId = c.Id,
                ParentCommentId = c.ParentCommentId,
                UserWhoCommentId = c.Membership.User.Id,
                UserWhoCommentUsername = c.Membership.User.Username
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException();
        }
    }

    public async Task<List<PostDTO>> GetAllTavernPosts(string tavernId)
    {
        try
        {
            var posts = await _context
            .Posts
            .Where(p => p.Membership.TavernId == tavernId)
            .Include(p => p.Membership)
                .ThenInclude(m => m.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Membership)
                    .ThenInclude(m => m.User)
            .Include(p => p.Likes)
                .ThenInclude(l => l.Membership)
                    .ThenInclude(m => m.User)
            .AsSplitQuery()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

            return posts.Select(p => new PostDTO
            {
                Comments = p.Comments.Select(c => new CommentDTO
                {
                    CommentContent = c.CommentContent,
                    CommentId = c.Id,
                    ParentCommentId = c.ParentCommentId,
                    UserWhoCommentId = c.Membership.User.Id,
                    UserWhoCommentUsername = c.Membership.User.Username
                }).ToList(),
                TavernId = p.Membership.TavernId,
                Likes = p.Likes.Select(l => new LikeDTO
                {
                    LikeId = l.Id,
                    UserWhoLikedId = l.Membership.User.Id,
                    UserWhoLikedUsername = l.Membership.User.Username,
                    MembershipId = l.Membership.Id,
                    TavernId = p.Membership.TavernId
                }).ToList(),
                MembershipUserId = p.Membership.UserId,
                MembershipUsername = p.Membership.User.Username,
                PostContent = p.Content,
                PostId = p.Id,
                PostImageUrl = p.PostImageUrl,
                PostTitle = p.Title,
                UserWhoPostedImage = p.Membership.User.ProfilePicture
            }).ToList();

        } catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }

    public async Task<Comment> GetCommentById(string commentId)
    {
        try
        {
            return await _context.Comments.Where(c => c.Id == commentId && !c.IsDeleted)
                .FirstOrDefaultAsync();
        } catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }

    public async Task<List<CommentDTO>> GetCommentRepliesAsync(string commentId)
    {
        try
        {
            var comments = _context
                .Comments
                .Where(c => c.ParentCommentId == commentId)
                .Include(c => c.Membership)
                    .ThenInclude(c => c.User);

            return comments
                .Select(c => new CommentDTO
                {
                    CommentContent = c.CommentContent,
                    CommentId = c.Id,
                    ParentCommentId = c.ParentCommentId,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId,
                    UserWhoCommentId = c.Membership.User.Id,
                    UserWhoCommentUsername = c.Membership.User.Username,
                    UserWhoCommmentProfilePicture = c.Membership.User.ProfilePicture
                })
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }

    public async Task<Post> GetPostByIdAsync(string postId)
    {
        try
        {
            return await _context.Posts
                .Where(p => p.Id == postId)
                .Include(p => p.Membership)
                    .ThenInclude(p => p.User)
                .Include(p => p.Likes)
                    .ThenInclude(p => p.Membership)
                    .ThenInclude(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(p => p.Membership)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync();
                
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }

    public async Task LikePost(Like newLike)
    {
        try
        {
            await _context.Likes.AddAsync(newLike);
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }

    public async Task UnlikePost(string likeId)
    {
        try
        {
            var likeEntity = await _context.Likes.Where(l => l.Id == likeId).FirstOrDefaultAsync();
            _context.Likes.Remove(likeEntity);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("");
        }
    }
}
