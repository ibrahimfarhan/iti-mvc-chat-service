using ITI.MVC.ChatService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITI.MVC.ChatService.Models.ViewModels
{
    public class MessageContainerViewModel
    {
        public ApplicationUser Receiver { get; set; }
        public ApplicationUser CurrentUser { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
        public bool IsFirstUser { get; set; }
    }
}