using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Domin.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
