using tavern_api.Commons.DTOs;
using tavern_api.Entities;

namespace tavern_api.Commons.Contracts.Repositories;

public interface ITavernRepository : IBaseRepository<Tavern>
{
    Task<Membership> CreateMembership(Membership entity);
    Task<List<TavernDTO>> GetAllUserMembershipsAsync(string id);
    Task<List<TavernUserDTO>> GetAllUserTavernUserAsync(string id);
    Task<List<UserTavernDTO>> GetAllTavernUsers(string tavernId);
    Task<Membership> GetUserMembershipAsync(string userId, string tavernId);
    Task RemoveMembership(Membership membership);
    Task<List<TavernGameDaysDTO>> GetAllTavernGameDaysAsync(string id);
    Task<GameDay> GetGameDayByIdAsync(string gameDayId);
    Task UpdateGameDayAsync(GameDay entity);


    Task<Folder> CreateFolderAsync(Folder entity);
    Task<List<FolderDTO>> GetAllUsersMembershipFolders(string id);
    Task<Folder> GetFolderByIsAsync(string folderId);

    Task<Item> CreateNewFileAsync(Item entity);
    Task<List<ItemDTO>> GetAllFileInFolderAndTavernAndSignedUser(string folderId);
    Task<Item> FindItemByIdAsync(string itemId);
    Task RemoveItemFromFolderAsync(Item entity);


}
