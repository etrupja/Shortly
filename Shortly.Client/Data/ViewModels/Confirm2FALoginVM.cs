using Shortly.Client.Helpers.Validators;
using System.ComponentModel.DataAnnotations;

namespace Shortly.Client.Data.ViewModels
{
    public class Confirm2FALoginVM
    {
        public string UserId { get; set; }
        public string UserConfirmationCode { get; set; }
    }
}
