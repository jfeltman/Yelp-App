using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamTeamwork_Yelp_App
{
    // Class used for Business search results
    class Business
    {
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string distance { get; set; }
        public string stars { get; set; }
        public string reviewCount { get; set; }
        public string checkinCount { get; set; }
        public string businessid { get; set; }
    }
}
