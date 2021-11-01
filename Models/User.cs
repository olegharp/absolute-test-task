using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace FilmsCatalog.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FullName { 
            get {
                return FirstName + 
                    (string.IsNullOrWhiteSpace(MiddleName) ? "" : " " + MiddleName) + 
                    (string.IsNullOrWhiteSpace(LastName) ? "" : " " + LastName);
            }
        }
    }
}