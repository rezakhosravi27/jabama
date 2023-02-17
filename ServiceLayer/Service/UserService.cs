using DomainLayer.Dto;
using DomainLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceLayer.IService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace ServiceLayer.Service
{
    public class UserService: IUserService
    {
        private readonly UserManager<IdentityUser> _userManager; 
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwt _JwtService;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager; 

        public UserService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IJwt JwtService,
            IConfiguration configuration,
            IEmailSender emailSender
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _JwtService = JwtService;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = signInManager; 
        }
        public async Task<IdentityResult> Register(User user)
        {
            try
            {
                var userIdentity = new IdentityUser() { UserName = user.UserName, Email = user.Email };
                return await _userManager.CreateAsync(userIdentity, user.Password);
            }
            catch
            {
                throw; 
            }
            
        }

        public async Task<IdentityResult> RegisterAdmin(User user)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(user.UserName);
                IdentityResult result = null; 
                if(userExists == null)
                {
                    var createUser = new IdentityUser() { UserName = user.UserName, Email = user.Email};
                    result = await _userManager.CreateAsync(createUser, user.Password); 
                    if(!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!await _roleManager.RoleExistsAsync("User"))
                        await _roleManager.CreateAsync(new IdentityRole("User"));

                    if (await _roleManager.RoleExistsAsync("Admin"))
                        await _userManager.AddToRoleAsync(createUser, "Admin"); 

                    if(await _roleManager.RoleExistsAsync("Admin"))
                        await _userManager.AddToRoleAsync(createUser, "User");

                }

                return result; 
              
            }
            catch
            {
                throw; 
            }
        }

        public async Task<List<IdentityUser>> GetUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                return users; 
            }
            catch
            {
                throw; 
            }
        }

        public async Task<IdentityUser> GetUser(string Username)
        {
            
            try
            {
               var user =  await _userManager.FindByNameAsync(Username);
                return user; 
            }
            catch
            {
                throw; 
            }
        }

        public async Task<AuthenticateResponse> Login(Login model)
        {
            try
            {
                 
                var user = await _userManager.FindByNameAsync(model.UserName);

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);
                if (result.IsLockedOut)
                {
                    var message = new Message(new string[] { user.Email }, "Locked out account information", "Your account is lockout,Kindly wait for 10 minutes and try again");
                    _emailSender.SendEmail(message);
                    throw new GlobalException(HttpStatusCode.BadRequest, "The account is locked out for 3 minutes");
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);

                var token = (AuthenticateResponse)null;
                if (user != null && isPasswordValid)
                {

                    var userRoles = await _userManager.GetRolesAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email)
                    }; 

                    foreach(var userRole in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole)); 
                    }; 
                   token = _JwtService.CreateToken(user, claims); 
                }

                return token; 
                
            }
            catch
            {
                throw; 
            }
        }

        public async Task ForgotPassword(ForgotPassword model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callback = "https://localhost:7172/" + "ResetPassword" + "/" + "Account" + "?token=" + token + "&email=" +  user.Email;
                    var message = new Message(new string[] { user.Email }, "Reset password token", callback);
                    _emailSender.SendEmail(message);
                }

            }
            catch
            {
                throw;
            }
        }
        public async Task<IdentityResult> ResetPassword(ResetPassword model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var resetPassResult = (IdentityResult)null;
                if (user != null)
                {
                    resetPassResult = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                }

                return resetPassResult;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IdentityResult> DeActiveUser(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var result = (IdentityResult)null; 
                if(user != null)
                {
                    result = await _userManager.SetLockoutEnabledAsync(user, true); 
                }

                return result; 
            }
            catch
            {
                throw; 
            }
        }

        public async Task<IdentityResult> DeleteUser(string id)
        {
            try
            {
                IdentityUser user = await _userManager.FindByIdAsync(id);
                IdentityResult result = null; 
                if (id != null)
                {
                    result = await _userManager.DeleteAsync(user); 
                }
                return result; 
            }
            catch
            {
                throw; 
            }
        }

    }
}
