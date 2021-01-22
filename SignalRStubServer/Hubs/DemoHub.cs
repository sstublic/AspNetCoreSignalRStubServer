using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRStubServer.Hubs
{
    public class DemoHub : Hub
    {
        private readonly DemoHubService demoHubService;
        public DemoHub(DemoHubService demoHubService)
        {
            this.demoHubService = demoHubService;
        }

        public string ClientPing()
        {
            demoHubService.AddClientPing();
            return "Client ping successful!";
        }
    }
}
