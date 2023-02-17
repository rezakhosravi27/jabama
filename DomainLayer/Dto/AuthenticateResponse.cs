using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Dto
{
    public class AuthenticateResponse
    {
        public string Token { get; set; } = null!; 
        public DateTime Expiration { get; set; }

    }
}
