using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoTorz.Shared.DTOs.Travelplanner
{
    public class HotelDto
    {
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public int Stars { get; set; }
        public string Price { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}
