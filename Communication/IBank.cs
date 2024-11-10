﻿using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface IBank : IService
    {
        Task<List<string>> ListClients();

        Task<string> EnlistMoneyTransfer(string userID, double amount);
    }
}