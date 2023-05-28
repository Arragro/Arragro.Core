using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Core.Identity.Models
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(255)]
        public string FirstName { get; set; } = "";
        [MaxLength(255)]
        public string LastName { get; set; } = "";
        public bool IsEnabled { get; set; }
        public bool External { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
