using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.AdminService;
using RR.Dto;
using System.Threading.Tasks;

namespace RR.Admin.Controllers
{
    [Authorize(Roles = "Admin,Subadmin")]
    public class PointTableManagementController : Controller
    {
        #region Constructor

        private readonly IPointTableService _pointTableService;

        public PointTableManagementController(IPointTableService pointTableService)
        {
            _pointTableService = pointTableService;
        }

        #endregion

        [Route("pointstable")]
       // [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> Index()
        {
            var points = await _pointTableService.GetAllPointTableData();

            return View(points);
        }

        [Route("addeditpoint/{id}")]
       // [Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditPointData(int id)
        {
            PointTableDto model = new PointTableDto();

            model = await _pointTableService.GetPointDataByID(id);

            return PartialView(model);
        }

        [HttpPost]
        [Route("addeditpoint/{id}")]
        //[Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> AddEditPointData(PointTableDto model)
        {
            
            long Id = model.Id;
            var resultId = await _pointTableService.AddEditPointTableData(model);
            if (Id == 0)
                return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Point data has been added successfully.</div>");
            else
                return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Point data has been edited successfully.</div>");
        }

        [HttpPost]
        [Route("deletepoint/{id}")]
        //[Authorize(Policy = "PagePermission")]
        public async Task<IActionResult> DeletePointData(int id)
        {
            await _pointTableService.DeletePointDataByID(id);
            return Content("<div class='alert alert-success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>×</span></button><strong>Success!!!</strong> Point data has been deleted successfully.</div>");
        }
    }
}
