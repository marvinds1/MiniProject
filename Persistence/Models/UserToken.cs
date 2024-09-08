using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Models
{
    public class UserToken
    {
        [Key]
        public string UserId { get; set; }

        [Key]
        public string LoginProvider { get; set; }

        [Key]
        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Expiry { get; set; }
    }
}


