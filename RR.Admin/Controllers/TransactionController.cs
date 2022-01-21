using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    
    public class TransactionController : Controller
    {
        #region Constructor
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        #endregion
        
        [Route("transactions")]
        [Authorize(Policy = "PagePermission")]
        public IActionResult Index()
        {
            
            return View();
        }
                
        [HttpPost]
        [Route("transaction/allTransactions")]
        public async Task<JsonResult> GetTransactions()
        {
            var draw = Request.Form["draw"];
            var start = Request.Form["start"];
            var length = Request.Form["length"];

            //Global search field
            var search = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = !(string.IsNullOrEmpty(length)) ? Convert.ToInt32(length) : 0;
            int skip = !(string.IsNullOrEmpty(start)) ? Convert.ToInt32(start) : 0;
            var sort = Request.Form["order[0][dir]"];
            var column = Convert.ToInt32(Request.Form["order[0][column]"]);

            var contests = await _transactionService.GetAllTransactionHistory(skip / pageSize, pageSize, column, search, sort);

            var data = contests.Item1;
            int recordsTotal = contests.Item2;

            return Json(new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal,
                data = data
            });
        }


    }
}