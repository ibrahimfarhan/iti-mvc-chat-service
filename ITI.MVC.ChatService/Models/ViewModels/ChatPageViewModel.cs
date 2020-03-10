using ITI.MVC.ChatService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITI.MVC.ChatService.Models.ViewModels
{
    public class ChatPageViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public MessageContainerViewModel MessageContainerViewModel { get; set; }
    }
}