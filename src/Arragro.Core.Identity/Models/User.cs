using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Arragro.Core.Identity.Models
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(255)]
        [Required]
        public string FirstName { get; set; } = "";
        [MaxLength(255)]
        [Required]
        public string LastName { get; set; } = "";
        public bool IsEnabled { get; set; } = true;
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
