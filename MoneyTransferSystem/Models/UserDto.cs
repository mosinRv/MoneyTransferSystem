using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoneyTransferSystem.Models
{
    public class UserDto
    {
        public string User { get; set; }
        public string Role { get; set; }
        public IEnumerable<AccountDto> Accounts{ get; set; }
    }

    //[JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    // public enum Role
    // {
    //     Admin,
    //     User
    // }
}