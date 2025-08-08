using Microsoft.EntityFrameworkCore;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly string _Title = "User";
        public UserService(AppDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<UserVM> GetAll()
        {
            try
            {
                var _result = _context.Users.Where(x => !x.IsDeleted)
                             .Select(x => new UserVM()
                             {
                                 ID = x.ID,
                                 FirstName = x.FirstName,
                                 LastName = x.LastName,
                             })
                             .AsNoTracking()
                             .AsEnumerable()
                             .OrderBy(x => x.FullName)
                             .ToList();
                return _result ?? new List<UserVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetAll in {_Title} Service due to {ex}");
            }
            return new List<UserVM>();
        }
        //Get User By Email
        public User GetUserByEmail(string email)
        {
            try
            {
                var _user = _context.Users.Where(x => !x.IsDeleted
                && x.Email.Trim() == email.Trim()).FirstOrDefault();
                return _user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetUserByEmail in {_Title} Service due to {ex}");
            }
            return null;
        }
        //Get User By Token
        public User GetUserByToken(string token)
        {

            try
            {
                var _user = _context.Users.Where(x => !x.IsDeleted
                && !string.IsNullOrEmpty(x.UserToken) && x.UserToken.Trim() == token.Trim()).FirstOrDefault();
                return _user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GetUserByToken in {_Title} Service due to {ex}");
            }
            return null;
        }
    }
}
