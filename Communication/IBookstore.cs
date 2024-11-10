using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IBookstore : IService
    {
        Task<List<string>> ListAvailableItems();

        Task<string> EnlistPurchase(string bookID, uint count);

        Task<string> GetItemPrice(string bookID);
    }
}
