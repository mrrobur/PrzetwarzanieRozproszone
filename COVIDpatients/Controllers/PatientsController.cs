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

namespace COVIDpatients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;
        public PatientsController(DpDataContext context)
        {
            _context = context;
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


            HttpClient client = new HttpClient();
            string EmailJson = JsonSerializer.Serialize(new EmailMessageRequest()
            {
                EmailAddress = "devmail.kw@gmail.com",
                Subject = "Zarejestrowano pacjenta",
                Body = "Treść wiadomości"
            });

            await client.PostAsync("https://localhost:44311/api/email",
                new StringContent(EmailJson, Encoding.UTF8, "application/json"));



            return Created("/api/patients", p);
        }
    }

    public class EmailMessageRequest
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
