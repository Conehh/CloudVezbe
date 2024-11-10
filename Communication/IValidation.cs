using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication
{
    public interface IValidation : IService
    {
        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(string? bookID, uint? count);

        Task<string> GetItemPrice(string? bookID);

        Task<List<string>> ListClients();

        Task<string> EnlistMoneyTransfer(string? userID, double? amount);

    }
}
