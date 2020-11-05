using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COVIDpatients.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        
        [HttpGet]
        public IActionResult GetAll() /*Get* / Post* definiuje rodzaj metody w przeglądarce - definicja Explicit do Swaggera*/
        {
            return Ok(_context.Patients.ToList());
        }

        [HttpPost]
        public IActionResult Add(Patients p)
        {

            _context.Patients.Add(p);
            _context.SaveChanges();

            return Created("/api/patients", p);
        }
    }
}
