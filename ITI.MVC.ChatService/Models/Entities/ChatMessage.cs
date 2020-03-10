using ITI.MVC.ChatService.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITI.MVC.ChatService.Models.Entities
{
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }

        [Required]
        public string Message { get; set; }

        public MessageStatus MessageStatus { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Time { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}