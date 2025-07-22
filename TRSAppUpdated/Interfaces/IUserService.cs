using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IUserService
    {
        User GetUserByToken(string token);
        User GetUserByEmail(string email);
    }
}
