using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cars_DTView.Controllers
{
    public class ChartsController : Controller
    {
        // GET: Charts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetChart()
        {
            var s = new[] { new { label = "2008", value= 20 },
                new { label= "2008", value= 5 },
                new { label= "2010", value= 7 },
                new { label= "2011", value= 10 },
                new { label= "2012", value= 20 }};
            return Content(JsonConvert.SerializeObject(s), "application/json");
        }
    }
}