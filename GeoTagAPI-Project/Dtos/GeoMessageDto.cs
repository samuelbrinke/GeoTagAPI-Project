using GeoTagAPI_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoTagAPI_Project.Dtos
{
    namespace V1
    {
        public class GeoMessageDto
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }
    }
    namespace V2
    {
        public class GeoMessageDto
        {
            public int Id { get; set; }
            public Message Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }
    }
}
