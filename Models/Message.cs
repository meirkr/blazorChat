using System.ComponentModel.DataAnnotations;

namespace blazorChat.Models
{
    public class Message
    {
        [Required(AllowEmptyStrings = false, ErrorMessage= "Username is required")]
        public string SenderName { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage= "Please write a message...")]
        public string Text { get; set; }
    }
}