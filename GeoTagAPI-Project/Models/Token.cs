using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoTagAPI_Project.Models
{
    public class Token
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public User User { get; set; }
    }
}
