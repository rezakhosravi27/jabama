using DomainLayer.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IService
{
    public interface IJwt
    {
        AuthenticateResponse CreateToken(IdentityUser user, List<Claim> claims); 
    }
}
