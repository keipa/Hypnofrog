using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using Hypnofrog.DBModels;
using Hypnofrog.Models;
using Hypnofrog.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

using System.Web;

namespace Hypnofrog
{
    public class ChatHub: Hub
    {
        public void Send(string AchievmentName)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage("Поздравляем! Вы получили достижение: "+AchievmentName, "http://localhost:61065/Home/Famehall");
        }
    }
}