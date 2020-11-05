using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notifications.Model;

namespace Notifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        public void SendMessage(EmailMessageRequest request)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("devmail.kw@gmail.com", "9P4g$S$9@0K7%C"),
                EnableSsl = true
            };
            smtp.Send("devmail.kw@gmail.com", request.EmailAddress, request.Subject, request.Body);
        }
    }
}
