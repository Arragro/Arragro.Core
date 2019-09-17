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
        public bool IsEnabled { get; set; } = true;
        public bool External { get; set; } = false;
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
