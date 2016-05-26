using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cars_DTView.Models
{
    public class CarsViewModel
    {
        public int id { get; set; }
        public string make { get; set; }
        public string model_name { get; set; }
        public string model_trim { get; set; }
        public string model_year { get; set; }
        public string body_style { get; set; }
        public string engine_num_cyl { get; set; }
        public string engine_power_ps { get; set; }
        public string drive_type { get; set; }
        public string seats { get; set; }
        public string link { get; set; }
     
    }
}