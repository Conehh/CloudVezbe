using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Communication.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BookstoreService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class BookstoreService : StatefulService, IBookstore, Communication.ITransaction
    {
        private IReliableDictionary<string, Book>? _bookDictionary;
        
        public BookstoreService(StatefulServiceContext context)
            : base(context)
        { }

        private async Task InitializeBookDictionaryAsync()
        {
            _bookDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<string, Book>>("bookDictionary");
        }

        public async Task<string> EnlistPurchase(string? bookID, uint? count)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<Book> book = await _bookDictionary!.TryGetValueAsync(transaction, bookID!);
             
                if (!await Prepare(count!.Value))
                {
                    return null!;
                }

                var bookToUpdate = book.Value;
                bookToUpdate.Quantity -= Convert.ToInt32(count);

                await _bookDictionary.TryUpdateAsync(transaction, bookID!, bookToUpdate, book.Value);


                return await CompleteTransaction(transaction);
            }
        }

        public async Task<string> GetItemPrice(string bookID)
        {
            using (var transaction = StateManager.CreateTransaction())
            {
                var book = await _bookDictionary!.TryGetValueAsync(transaction, bookID!);

                return book.Value.Price!.ToString();
            }

            throw null!;
        }

        public async Task<List<string>> ListAvailableItems()
        {
            await InitializeBookDictionaryAsync();

            var books = new List<Book>()
            {
                new Book(){ Id = "1324", Name = "Book1", Price = 8.99, Quantity = 5 },
                new Book(){ Id = "235", Name = "Book2", Price = 13.99, Quantity = 3 },
                new Book(){ Id = "3156", Name = "Book3", Price = 9, Quantity = 5 },
            };

            using (var transaction = StateManager.CreateTransaction())
            {
                foreach (Book book in books)
                    await _bookDictionary!.AddOrUpdateAsync(transaction, book.Id!, book, (k, v) => v);

                await CompleteTransaction(transaction);
            }

            var bookNames = new List<string>();

            using (var transaction = StateManager.CreateTransaction())
            {
                var enumerator = (await _bookDictionary!.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var book = enumerator.Current.Value;
                    bookNames.Add(book.Name);
                }
            }

            return bookNames;
        }

        public async Task Commit(Microsoft.ServiceFabric.Data.ITransaction transaction)
        {
            await transaction.CommitAsync();
        }

        public async Task<bool> Prepare(object count)
        {
            return count is uint uintParameter;
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
