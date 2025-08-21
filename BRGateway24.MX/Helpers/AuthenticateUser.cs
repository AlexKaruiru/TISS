using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace BRGateway24.Helpers
{
    public class AuthenticateUser
    {
        public string? LanguageID { get; set; }
        [Key]
        public string Password { get; set; }
        public DateTime? TimeLoggedIn { get; set; }
        public string? IPAddress { get; set; }
        //public short? FailedAttempts { get; set; }
        public string? LoginFailureIPAddress { get; set; }

        public string RoleID { get; set; }
        public short Status { get; set; }
        public string? SessionID { get; set; }

        public string UserID { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfilePic { get; set; }
        public string OutputJSON { get; set; }
    }
}
