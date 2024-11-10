using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Validation
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Validation : StatelessService, IValidation
    {
        private readonly string transactionCoordinatorPath = @"fabric:/CloudVezbe/TransactionCoordinator";
        public Validation(StatelessServiceContext context)
            : base(context)
        { }

        public async Task<List<string>> ListAvailableItems()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListAvailableItems();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistPurchase(string? bookID, uint? count)
        {
            if (bookID is null || count is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistPurchase(bookID, count!.Value);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> GetItemPrice(string? bookID)
        {
            if (bookID is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.GetItemPrice(bookID);
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<string>> ListClients()
        {
            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.ListClients();
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<string> EnlistMoneyTransfer(string? userID, double? amount)
        {
            if (userID is null || amount is null)
            {
                return null!;
            }

            ITransactionCoordinator? transactionProxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri(transactionCoordinatorPath));

            try
            {
                return await transactionProxy.EnlistMoneyTransfer(userID, amount!.Value);
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
