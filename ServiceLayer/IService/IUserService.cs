using DomainLayer.Dto;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IService
{
    public interface IUserService
    {
        Task<IdentityResult> Register(User user);
        Task<IdentityResult> RegisterAdmin(User user);
        Task<List<IdentityUser>> GetUsers(); 
        Task<IdentityUser> GetUser(string Username);
        Task<AuthenticateResponse> Login(Login model);
        Task ForgotPassword(ForgotPassword model);
        Task<IdentityResult> ResetPassword(ResetPassword model);
        Task<IdentityResult> DeActiveUser(string username);
        Task<IdentityResult> DeleteUser(string id); 
    }
}
