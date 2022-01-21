using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RR.Service;
using System.Threading.Tasks;

namespace RR.Web.Controllers
{
     [ViewComponent(Name = "TransactionHistoryComponent")]
     public class TransactionHistoryComponent : ViewComponent
     {
          private readonly UserManager<IdentityUser> _userManager;
          private readonly ITransactionService _transService;

          public TransactionHistoryComponent(
             UserManager<IdentityUser> userManager,
             ITransactionService transService)
          {
               _userManager = userManager;
               _transService = transService;
          }

          /// <summary>
          /// Uet user transaction history
          /// </summary>
          /// <returns></returns>
          public async Task<IViewComponentResult> InvokeAsync()
          {
               var transactionHistory = await _transService.TransactionHistory(_userManager.GetUserId(HttpContext.User));
               return View(transactionHistory);
          }
     }
}
