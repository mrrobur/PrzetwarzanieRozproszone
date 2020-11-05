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

namespace COVIDpatients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusSender _sender;

        public PatientsController(DpDataContext context, ServiceBusSender sender)
        {
            _context = context;
            _sender = sender;
        }
        
        // Get list of patients from PatientsController
        [HttpGet]
        public IActionResult GetAll() /*Get* / Post* definiuje rodzaj metody w przeglądarce - definicja Explicit do Swaggera*/
        {
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

           // await _sender.SendMessage(new MessagePayload() { EventName = "NewUserRegistered", UserEmail = "devmail.kw@gmail.com" });
            await _sender.SendMessage(new MessagePayload() { EventName = "NewUserRegistered", UserEmail = patientEmail, dateStart = startDate });


            return Created("/api/patients", p);
        }
    }

}
