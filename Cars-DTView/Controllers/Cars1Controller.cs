using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cars_DTView.Models;
using System.Threading.Tasks;
using DataTables.Mvc;
using Newtonsoft.Json;

namespace Cars_DTView.Controllers
{
    public class Cars1Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cars1
        public async Task<ActionResult> Index(string year,string make, string model, string trim)
        {
            ViewBag.make = new SelectList(new List<string>(), make);
            ViewBag.model = new SelectList(new List<string>(), model);
            ViewBag.trim = new SelectList(new List<string>(), trim);
            if (string.IsNullOrWhiteSpace(year))
            {
                year = "2000";
               
            }
            else
            {
                ViewBag.make = new SelectList(await db.GetMakes(year));
            }
            if (!string.IsNullOrWhiteSpace(make))
            {
                ViewBag.model = new SelectList(await db.GetModels(year, make));
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                ViewBag.trim = new SelectList(await db.GetTrims(year, make, model));
            }
            
            ViewBag.year = new SelectList(await db.GetYears(), year);
           

            //var cars = await db.GetCars(year, make, model, trim, "", false, 0, 0);
            return View();
        }

        public async Task<ActionResult> GetMakes(string year)
        {
            var make = new SelectList(await db.GetMakes(year));
            return Content(JsonConvert.SerializeObject(make), "application/json");
        }

        public async Task<ActionResult> GetModels(string year,string make)
        {
            var model = new SelectList(await db.GetModels(year, make));
            return Content(JsonConvert.SerializeObject(model), "application/json");
        }

        public async Task<ActionResult> GetTrims(string year, string make, string model)
        {
            var trim = new SelectList(await db.GetTrims(year, make, model));
            return Content(JsonConvert.SerializeObject(trim), "application/json");
        }

        public async Task<JsonResult> GetCars([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest request,
            string year = "2000", string make="", string model="", string trim="")
        {
            
            var filter = request.Search.Value;

            var totalCount = await db.GetCarsCount(year, make, model, trim, filter);

            var column = request.Columns.FirstOrDefault(r => r.IsOrdered == true);
            var sortColumn = "";
            var sortOrder = "asc";
            if (column != null)
            {
                sortColumn = column.Data;
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    sortOrder = "Desc";
                }
            }
            List<Car> cars = new List<Car>();
            cars = await db.GetCars(year, make, model, trim, filter, true, (request.Start / request.Length + 1), request.Length);

            var paged = cars.Select(c => 
            new CarsViewModel
            {
                id=c.id,
                make=c.make,
                model_name=c.model_name,
                model_trim = c.model_trim,
                model_year = c.model_year,
                body_style = c.body_style,
                engine_num_cyl = c.engine_num_cyl,
                engine_power_ps = c.engine_power_ps,
                drive_type = c.drive_type,
                seats = c.seats,
                link = "<a href=\"/Cars1/Details/" + c.id + "\">Details</a>",

            });
            return Json(new DataTablesResponse(request.Draw, paged, totalCount, db.Cars.Count()),
                JsonRequestBehavior.AllowGet);
        }

        
        // GET: Cars1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Car car = db.Cars.Find(id);
            if (car == null)
            {
                return HttpNotFound();
            }
            return View(car);
        }

        //public async Task<ActionResult> GetMakes(string year)
        //{
        //    ViewBag.make = new SelectList(await db.Cars.Where(c=>c.model_year == year).Select(d=>d.make).Distinct().ToListAsync());
        //    return Content("");
        //}


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
