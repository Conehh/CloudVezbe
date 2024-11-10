using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Communication.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
namespace BankService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BankService : StatefulService, IBank, Communication.ITransaction
    {

        private IReliableDictionary<string, Client>? _clientDictionary;

        public BankService(StatefulServiceContext context)
            : base(context)
        { }

        private async Task InitializeClientDictionaryAsync()
        {
            _clientDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, Client>>("clientDictionary");
        }

        public async Task<string> EnlistMoneyTransfer(string? userID, double? amount)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Client> client = await _clientDictionary!.TryGetValueAsync(transaction, userID!);


                if (!await Prepare(amount!.Value))
                {
                    return null!;
                }



                var updatedClient = client.Value;
                updatedClient.Balance -= amount;

                await _clientDictionary.TryUpdateAsync(transaction, userID!, updatedClient, client.Value);

                return await CompleteTransaction(transaction);
            }
        }

        public async Task<List<string>> ListClients()
        {
            await InitializeClientDictionaryAsync();

            var clients = new List<Client>()
            {
                new Client() { Id = "123", Name = "Mark", Balance = 950.23, },
                new Client() { Id = "234", Name = "Campton", Balance = 35000.10 },
               
            };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (Client client in clients)
                    await _clientDictionary!.AddOrUpdateAsync(transaction, client.Id!, client, (k, v) => v);

                await CompleteTransaction(transaction);
            }

            var clientNames = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await _clientDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var client = enumerator.Current.Value;
                    clientNames.Add(client.Name!);
                }
            }

            return clientNames;
        }
        public async Task<bool> Prepare(object amount)
        {
            return amount is double doubleParameter;
        }

        public async Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task Rollback(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
             transaction.Abort();
        }


        public async Task<string> CompleteTransaction(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            try
            {
                await Commit(transaction);
                return string.Empty;
            }
            catch
            {
                await Rollback(transaction);
                return null!;
            }
        }
        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

    }
}
