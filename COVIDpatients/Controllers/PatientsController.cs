using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using COVIDpatients.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using COVIDpatients.Services;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace COVIDpatients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusSender _sender;
        private readonly ILogger _logger;

        public PatientsController(DpDataContext context, ServiceBusSender sender, ILogger logger)
        {
            _context = context;
            _sender = sender;
            _logger = logger;
        }
        
        // Get list of patients from PatientsController
        [HttpGet]
        public IActionResult GetAll() /*Get* / Post* definiuje rodzaj metody w przeglądarce - definicja Explicit do Swaggera*/
        {
            _logger.Information("Requested patients list");

            return Ok(_context.Patients.ToList());

        }


        // Send email on EmailController:44311
        [HttpPost]
        public async Task<IActionResult> Add(Patients p)
        {

            _context.Patients.Add(p);
            _context.SaveChanges();

              string patientEmail = p.email;
            string startDate = p.startDate;

            await _sender.SendMessage(new MessagePayload() { EventName = "NewUserRegistered", UserEmail = patientEmail, dateStart = startDate });
            _logger.Information("Added patient: " + p.name + " " + p.surname);


            return Created("/api/patients", p);
        }

        [HttpPut]
        public IActionResult InvalidAction()
        {
            throw new InvalidOperationException("Test exception");
        }
    }

}
