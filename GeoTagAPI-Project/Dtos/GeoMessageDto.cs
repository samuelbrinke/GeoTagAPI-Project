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
            public string Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }
    }
    namespace V2
    {
        public class GetGeoMessageDto
        {
            public GetMessageDto Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        public class GetMessageDto
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string Author { get; set; }
        }

        public class AddGeoMessageDto
        {
            public AddMessageDto Message { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
        }

        public class AddMessageDto
        {
            public string Title { get; set; }
            public string Body { get; set; }
        }
    }
}
