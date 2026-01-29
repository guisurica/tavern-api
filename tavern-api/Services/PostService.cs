using Npgsql.PostgresTypes;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Services;

internal sealed class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly ITavernRepository _tavernRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;

    public PostService(IPostRepository postRepository, 
        ITavernRepository tavernRepository,
        IUserRepository userRepository,
        IFileService fileService)
    {
        _postRepository = postRepository;
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
        _fileService = fileService;
    }

    public async Task<Result<PostDTO>> CreatePostAsync(CreatePostDTO input, string userId)
    {
        try
        {
            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<PostDTO>().Failure("Taverna não encontrada", null, 404);

            var userFound = await _userRepository.GetById(userId);
            if (userFound == null)
                return new Result<PostDTO>().Failure("Usuário não encontrado", null, 404);

            var userMembershipFound = await _tavernRepository.GetUserMembershipAsync(userFound.Id, tavernFound.Id);
            if (userMembershipFound == null)
                return new Result<PostDTO>().Failure("Usuário não pertence a taverna", null, 404);

            var newPost = Post.Create(input.PostTitle, input.PostContent, userMembershipFound.Id);

            if (input.PostImage.Length > 0)
            {
                var postImageUrl = await _fileService.SaveImageWWWRootUrl(input.PostImage.OpenReadStream(), 
                    Path.GetExtension(input.PostImage.FileName),
                    newPost.Id);

                newPost.UpdateImage(postImageUrl.Data);
            }

            var post = await _postRepository.CreateAsync(newPost);

            var postDTO = new PostDTO
            {
                Comments = new List<CommentDTO>(),
                Likes = new List<LikeDTO>(),
                MembershipUserId = post.MembershipId,
                MembershipUsername = userMembershipFound.User.Username,
                PostContent = post.Content,
                PostImageUrl = post.PostImageUrl,
                PostId = post.Id,
                PostTitle = post.Title,
                TavernId = userMembershipFound.TavernId,
                UserWhoPostedImage = userMembershipFound.User.ProfilePicture
            };

            return new Result<PostDTO>().Success("Post criado com sucesso", postDTO, 200);


        } catch (DomainException ex)
        {
            return new Result<PostDTO>().Failure(ex.Message, null, 400);
        } catch (InfrastructureException ex)
        {
            return new Result<PostDTO>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<List<PostDTO>>> GetPostsAsync(string tavernId, string userId)
    {
        try
        {
            var tavernFound = await _tavernRepository.GetById(tavernId);
            if (tavernFound == null)
                return new Result<List<PostDTO>>().Failure("Taverna não encontrada", null, 404);

            var userFound = await _userRepository.GetById(userId);
            if (userFound == null)
                return new Result<List<PostDTO>>().Failure("Usuário não encontrado", null, 404);

            var membershipFound = await _tavernRepository.GetUserMembershipAsync(userFound.Id, tavernFound.Id);
            if (membershipFound == null)
                return new Result<List<PostDTO>>().Failure("Usuário não pertence a essa taverna", null, 404);

            var allPosts = await _postRepository
                .GetAllTavernPosts(tavernId);

            foreach(var post in allPosts)
            {
                var alreadyLiked = post.Likes
                    .Where(p => p.MembershipId == membershipFound.Id).FirstOrDefault();

                if (alreadyLiked != null)
                {
                    post.UserAlreadyLiked = true;
                }
            }

            return new Result<List<PostDTO>>().Success(string.Empty, allPosts, 200);
        }
        catch (DomainException ex)
        {
            return new Result<List<PostDTO>>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<List<PostDTO>>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<string>> LikePostAsync(LikePostDTO input, string userId)
    {
        try
        {

            var userFound = await _userRepository.GetById(userId);
            if (userFound == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var postFound = await _postRepository.GetPostByIdAsync(input.PostId);
            if (postFound == null)
                return new Result<string>().Failure("Postagem não encontrada", null, 404);

            var membershipFound = await _tavernRepository.GetUserMembershipAsync(userFound.Id, postFound.Membership.TavernId);
            if (membershipFound == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, 404);

            if (postFound.Likes.Count > 0)
            {
                if (postFound.Likes.Select(l => l.MembershipId).Contains(membershipFound.Id))
                {
                    var likedPost = postFound.Likes.Where(l => l.MembershipId == membershipFound.Id && l.PostId == postFound.Id).FirstOrDefault();

                    if (likedPost != null)
                    {
                        var unlikePostRequest = await UnlikePost(postFound, likedPost);

                        return new Result<string>().Success(unlikePostRequest);
                    }

                }
            }

            var newLike = Like.Create(membershipFound.Id, postFound.Id);

            await _postRepository.LikePost(newLike);

            return new Result<string>().Success("Você deu like nessa postagem", null, 201);

        } catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        } catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }

    private async Task<Result<string>> UnlikePost(Post post, Like like)
    {
        try
        {
            await _postRepository.UnlikePost(like.Id);

            return new Result<string>().Success("Você retirou seu like", null, 201);
        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }
}
