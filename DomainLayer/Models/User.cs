using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class User
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Username must lower than 20 characters")]
        public string UserName { get; set; } = null!; 
        [Required]
        [MinLength(5, ErrorMessage = "Password must grather than 5 characters")]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

    }
}
