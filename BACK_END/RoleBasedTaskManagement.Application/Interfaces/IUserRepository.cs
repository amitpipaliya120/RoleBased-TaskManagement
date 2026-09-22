using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        bool SaveUser(UserModel user);
        bool DeleteUser(int id);
        UserModel? GetUserById(int id);
        UserModel? GetUserByEmail(string email);
        List<UserModel> GetAllUsers();
        List<UserModel> GetUsersByFlag(UserModel model);
    }
}
