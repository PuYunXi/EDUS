using Abp;
using Abp.Authorization.Users;
using System.Threading.Tasks;

namespace YUNXI.EDUS.Authorization.Users
{
    public  interface IUserLinkManager
    {
        Task<bool> AreUsersLinked(UserIdentifier firstUserIdentifier, UserIdentifier secondUserIdentifier);

        Task<UserAccount> GetUserAccountAsync(UserIdentifier userIdentifier);

        Task Link(User firstUser, User secondUser);

        Task Unlink(UserIdentifier userIdentifier);
    }
}
