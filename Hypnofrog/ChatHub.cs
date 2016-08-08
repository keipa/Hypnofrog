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
            Clients.All.broadcastMessage(@Resources.Resource.congrats + AchievmentName, "http://localhost:61065/Famehall");
        }
    }
}