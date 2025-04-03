<<<<<<< HEAD
﻿using GoTorz.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GoTorz.Api.Data;
using GoTorz.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
=======
﻿using Microsoft.AspNetCore.Mvc;
using GoTorz.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoTorz.Api.Services;
>>>>>>> Marlen

namespace GoTorz.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelPackageController : ControllerBase
    {
<<<<<<< HEAD
        private readonly ApplicationDbContext _context;

        public TravelPackageController (ApplicationDbContext context)
        {
            _context = context;
        }

        private List<TravelPackage> TravelPackages = new()
        {
            new() {
                TravelPackageId = "TP001",
                Destination = "Paris, France",
                Arrival = new DateTime(2025, 6, 15),
                Departure = new DateTime(2025, 6, 20),
                Hotel = new Hotel { Name = "Hotel Le Meurice" },
                OutboundFlight = new OutboundFlight { Airline = "Air France", FlightNumber = "AF123" },
                ReturnFlight = new ReturnFlight { Airline = "Air France", FlightNumber = "AF124" },
                price = "$1200"
            },
            new() {
                TravelPackageId = "TP002",
                Destination = "New York, USA",
                Arrival = new DateTime(2025, 7, 10),
                Departure = new DateTime(2025, 7, 15),
                Hotel = new Hotel { Name = "The Plaza Hotel" },
                OutboundFlight = new OutboundFlight { Airline = "Delta Airlines", FlightNumber = "DL456" },
                ReturnFlight = new ReturnFlight { Airline = "Delta Airlines", FlightNumber = "DL457" },
                price = "$1500"
            },
            new() {
                TravelPackageId = "TP003",
                Destination = "Tokyo, Japan",
                Arrival = new DateTime(2025, 9, 5),
                Departure = new DateTime(2025, 9, 15),
                Hotel = new Hotel { Name = "The Ritz-Carlton Tokyo" },
                OutboundFlight = new OutboundFlight { Airline = "Japan Airlines", FlightNumber = "JL789" },
                ReturnFlight = new ReturnFlight { Airline = "Japan Airlines", FlightNumber = "JL790" },
                price = "$2200"
            },
            new() {
                TravelPackageId = "TP004",
                Destination = "Rome, Italy",
                Arrival = new DateTime(2025, 8, 12),
                Departure = new DateTime(2025, 8, 18),
                Hotel = new Hotel { Name = "Hassler Roma" },
                OutboundFlight = new OutboundFlight { Airline = "Alitalia", FlightNumber = "AZ321" },
                ReturnFlight = new ReturnFlight { Airline = "Alitalia", FlightNumber = "AZ322" },
                price = "$1800"
            },
            new() {
                TravelPackageId = "TP005",
                Destination = "Sydney, Australia",
                Arrival = new DateTime(2025, 11, 20),
                Departure = new DateTime(2025, 11, 30),
                Hotel = new Hotel { Name = "Park Hyatt Sydney" },
                OutboundFlight = new OutboundFlight { Airline = "Qantas", FlightNumber = "QF567" },
                ReturnFlight = new ReturnFlight { Airline = "Qantas", FlightNumber = "QF568" },
                price = "$2500"
            }
        };

        [HttpPost]
        public ActionResult Create([FromBody] TravelPackage travelPackage)
        {
            if (travelPackage == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid travel package data.");
            }

            // Generate a unique ID (better to use GUID for uniqueness)
            travelPackage.TravelPackageId = "TP" + (TravelPackages.Count + 1).ToString("D3");

            var newPackage = new TravelPackage
            {
                TravelPackageId = travelPackage.TravelPackageId,
                Destination = travelPackage.Destination,
                Arrival = travelPackage.Arrival,
                Departure = travelPackage.Departure,
                Hotel = travelPackage.Hotel,
                OutboundFlight = travelPackage.OutboundFlight,
                ReturnFlight = travelPackage.ReturnFlight,
                price = travelPackage.price
            };

            _context.TravelPackages.AddAsync(newPackage);
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            var travelPackage = await _context.TravelPackages.Where(tp => tp.TravelPackageId == id).FirstOrDefaultAsync();
            if (travelPackage is not null)
            {
                return Ok(travelPackage);
            }
            return NotFound("Travel package not found.");
        }

        [HttpGet("List")]
        public async Task<ActionResult<List<TravelPackage>>> List()
        {
            var travelPackages = await _context.TravelPackages.ToListAsync();
            return Ok(travelPackages);
        }

        [HttpPut]
        public ActionResult Update(TravelPackage newPackage)
        {
            var oldPackage = _context.TravelPackages.FirstOrDefault(tp => tp.TravelPackageId == newPackage.TravelPackageId);
            if (oldPackage is not null)
            {
                // Update fields as necessary
                oldPackage.Destination = newPackage.Destination;
                oldPackage.Arrival = newPackage.Arrival;
                oldPackage.Departure = newPackage.Departure;
                oldPackage.Hotel = newPackage.Hotel;
                oldPackage.OutboundFlight = newPackage.OutboundFlight;
                oldPackage.ReturnFlight = newPackage.ReturnFlight;
                oldPackage.price = newPackage.price;

                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return NotFound("Travel package not found.");
        }
        
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var travelPackage = _context.TravelPackages.FirstOrDefault(tp => tp.TravelPackageId == id);
            if (travelPackage is not null)
            {
                _context.TravelPackages.Remove(travelPackage);
                _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return NotFound("Travel package not found.");
        }

=======
        private readonly ITravelPackageService _travelPackageService;

        public TravelPackageController(ITravelPackageService travelPackageService)
        {
            _travelPackageService = travelPackageService;
            GetAll();
        }

        // Loads all travel packages initially when opening the page
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> GetAll()
        {
            var packages = await _travelPackageService.GetAllTravelPackagesAsync();
            return Ok(packages.ToList());
        }

        // Search function
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TravelPackage>>> Search(
            string? destination = null,
            DateTime? arrivalDate = null,
            DateTime? departureDate = null)
        {
            var packages = await _travelPackageService.GetTravelPackagesAsync(destination, arrivalDate, departureDate);
            return Ok(packages.ToList());
        }
>>>>>>> Marlen
    }
}
