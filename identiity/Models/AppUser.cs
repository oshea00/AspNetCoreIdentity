using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identiity.Models
{
    public class AppUser : IdentityUser
    {
        public enum Cities
        {
            None, London, Paris, Chicago
        }

        public enum QualificationLevels
        {
            None, Basic, Advanced
        }

        public Cities City { get; set; }
        public QualificationLevels Qualifications { get; set; }

    }
}
