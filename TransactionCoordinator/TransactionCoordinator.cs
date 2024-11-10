using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TransactionCoordinator
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCoordinator : StatelessService, ITransactionCoordinator
    {
        private readonly string bookstoreServicePath = @"fabric:/CloudVezbe/BookstoreService";
        private readonly string bankServicePath = @"fabric:/CloudVezbe/BankService";

        public TransactionCoordinator(StatelessServiceContext context)
            : base(context)
        { }


        public async Task<List<string>> ListAvailableItems()
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstoreServicePath), new ServicePartitionKey(1));

            try
            {
                return await bookstoreProxy.ListAvailableItems();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistPurchase(string? bookID, uint? count)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstoreServicePath), new ServicePartitionKey(1));

            try
            {
                return await bookstoreProxy.EnlistPurchase(bookID!, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItemPrice(string? bookID)
        {
            IBookstore? bookstoreProxy = ServiceProxy.Create<IBookstore>(new Uri(bookstoreServicePath), new ServicePartitionKey(1));

            try
            {
                return await bookstoreProxy.GetItemPrice(bookID!);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<string>> ListClients()
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankServicePath), new ServicePartitionKey(2));

            try
            {
                return await bankProxy.ListClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistMoneyTransfer(string? userID, double? amount)
        {
            IBank? bankProxy = ServiceProxy.Create<IBank>(new Uri(bankServicePath), new ServicePartitionKey(2));

            try
            {
                return await bankProxy.EnlistMoneyTransfer(userID!, amount!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }


        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    } 
}
