using Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Client.Models;
using Newtonsoft.Json;

namespace Client.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookstoreController : Controller
    {
        [HttpGet]
        [Route("ListAvailableItems")]
        public async Task<IActionResult> ListAvailableItems()
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            List<string> result = await validationProxy.ListAvailableItems();

            if (result is null)
            {
                return RedirectToAction("Error");
            }

            var books = new List<BookViewModel>();

            result.ForEach(x => books.Add(JsonConvert.DeserializeObject<BookViewModel>(x)!));

            return View(books);
        }

        [HttpGet]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase(string bookID, uint count)
        {
            return View(new EnlistPurchaseViewModel() { BookID = bookID, Count = count });
        }

        [HttpPost]
        [Route("EnlistPurchase")]
        public async Task<IActionResult> EnlistPurchase([FromForm] EnlistPurchaseViewModel model)
        {
            IValidation? validationProxy = ServiceProxy.Create<IValidation>(new Uri("fabric:/CloudVezbe/Validation"));

            string result = await validationProxy.EnlistPurchase(model.BookID, model.Count);

            if (result is null)
            {
                return RedirectToAction("Error");
            }

            return RedirectToAction("ListAvailableItems", "Bookstore");
        }

    }
}
