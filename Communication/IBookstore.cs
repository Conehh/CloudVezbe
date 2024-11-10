using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IBookstore : IService, ITransaction
    {
        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(string bookID, uint count);

        Task<string> GetItemPrice(string bookID);
    }
}
