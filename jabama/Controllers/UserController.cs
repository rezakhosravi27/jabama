using DomainLayer.Dto;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.IService;
using System.Net.WebSockets;

namespace jabama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(); 
                }
                var result =  await _userService.Register(user); 
                if(!result.Succeeded) {
                    return BadRequest(); 
                }
                return CreatedAtAction("GetUser", new { username = user.UserName },  user); 
            }catch(Exception err)
            {
                return StatusCode(500, err.Message); 
            }
        }

        [HttpPost("register-admin")]
        public async Task<ActionResult> RegisterAdmin(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(); 
                }

                var result = await _userService.RegisterAdmin(user);
                if (!result.Succeeded)
                {
                    return BadRequest("User creation failed, please check user details and try again");
                    
                   
                }
                return CreatedAtAction("GetUser", new { username = user.UserName }, user);
            }
            catch(Exception err)
            {
                return StatusCode(500, err.Message); 
            }
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(); 
                };

                var token =  await _userService.Login(model);
                if(token == null)
                {
                    return BadRequest("Username or Password not correct"); 
                }

                return StatusCode(200, token); 
            }
            catch(Exception err)
            {
                return StatusCode(500, err.Message); 
            }
        }

        [HttpGet]
        public async Task<ActionResult<IdentityUser>> GetUser(string Username)
        {
            try
            {
                var user = await _userService.GetUser(Username); 
                if(user == null)
                {
                    return NotFound(); 
                }


                return user;
            }catch(Exception err)
            {
                return StatusCode(500, err.Message); 
            }
        }

        [HttpPost("forgot-password")]
       
        public async Task<ActionResult> ForgotPassword(ForgotPassword model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                await _userService.ForgotPassword(model);
                return Ok("Reset password token send to your email");
            }
            catch (Exception err)
            {
                return StatusCode(500, err.Message);
            }
        }


        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPassword model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var result = await _userService.ResetPassword(model);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                }

                return Ok("Password Changed");

            }
            catch (Exception err)
            {
                return StatusCode(500, err.Message);
            }
        }

        [HttpPost("deActive-user")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeActiveUser(string username)
        {
            try
            {
                if(username == null)
                {
                    return BadRequest(); 
                }
                var result = await _userService.DeActiveUser(username);
                if (!result.Succeeded)
                {
                    return BadRequest(); 
                }

                return Ok("user deActivated"); 
                
                
            }catch(Exception err)
            {
                return StatusCode(500, err.Message); 
            }
        }
    }
}
