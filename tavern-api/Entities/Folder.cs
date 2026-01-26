using System.Text.RegularExpressions;
using tavern_api.Commons;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Entities;

public sealed class Folder : BaseEntity
{
    public string FolderName { get; private set; }
    public string MembershipId { get; private set; }
    public Membership Membership { get; private set; }
    public List<Item> Items { get; private set; } = new List<Item>();

    private static readonly string REGEX_FOLDERNAME_PATTERN = @"[^a-zA-Z0-9]";

    private Folder() { }

    private Folder(string folderName, string membershipId)
    {
        FolderName = folderName;
        MembershipId = membershipId;
    }

    public static Folder Create(string folderName, string membershipId)
    {
        VerifyFolderName(folderName);
        VerifyFolderMembership(membershipId);

        return new Folder(folderName, membershipId);
    }

    private static void VerifyFolderName(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainException("Uma pasta deve conter um nome");

        if (Regex.IsMatch(name, REGEX_FOLDERNAME_PATTERN))
        {
            throw new DomainException("Nome de pasta não pode conter nenhum caracter especial");
        }
    }

    private static void VerifyFolderMembership(string membershipId)
    {
        if (string.IsNullOrEmpty(membershipId)) throw new DomainException("A pasta precisa estar vinculada a algum jogador");
    }

    public void ChangeFolderName(string newFolderName)
    {
        this.FolderName = newFolderName;
    }

    public void AddItem(Item item)
    {
        this.Items.Add(item);
    }

    public void RemoveItem(Item item) 
    {
        this.Items.Remove(item);
    }
}
