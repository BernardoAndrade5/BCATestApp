using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCATestApp.Model
{
    public class Car
    {
            public required string Make { get; set; }
            public required string Model { get; set; }
            public required string EngineSize { get; set; }
            public required string Fuel { get; set; }
            public int Year { get; set; }
            public int Mileage { get; set; }
            public required string AuctionDateTime { get; set; }
            public int StartingBid { get; set; }
            public bool Favourite { get; set; }
            public required Detail Details { get; set; }

        public class Detail
        {
            public required Specification Specification { get; set; }
            public required Ownership Ownership { get; set; }
            public required string[] Equipment { get; set; }
        }

        public class Specification
        {
            public required string VehicleType { get; set; }
            public required string Colour { get; set; }
            public required string Fuel { get; set; }
            public required string Transmission { get; set; }
            public int NumberOfDoors { get; set; }
            public required string CO2Emissions { get; set; }
            public int NoxEmissions { get; set; }
            public int NumberOfKeys { get; set; }
        }

        public class Ownership
        {
            public required string LogBook { get; set; }
            public int NumberOfOwners { get; set; }
            public required string DateOfRegistration { get; set; }
        }

    }
}
