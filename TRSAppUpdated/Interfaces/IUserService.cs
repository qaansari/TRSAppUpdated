using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Interfaces
{
    public interface IUserService
    {
        List<UserVM> GetAll();
        User GetUserByToken(string token);
        User GetUserByEmail(string email);
    }
}
