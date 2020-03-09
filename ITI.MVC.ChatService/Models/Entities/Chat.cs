using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ITI.MVC.ChatService.Models.Entities
{
    [Table("Chat")]
    public class Chat
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("Sender")]
        public string SenderId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }

        [Required]
        public string Message { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Time { get; set; }

        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}