using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Domain
{
    /// <summary>
    /// https://medium.com/@zarkopafilis/asp-net-core-2-2-rest-api-13-refreshing-jwts-with-refresh-tokens-b656713ed03a
    /// </summary>
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsInvalided { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
