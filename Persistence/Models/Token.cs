using System;
using Microsoft.AspNetCore.Identity;

namespace Persistence.Models
{
    namespace Persistence.Models
    {
        public class UserToken : IdentityUserToken<string>
        {
            // Properti tambahan jika diperlukan
            public DateTime Expiry { get; set; }
        }
    }

}

