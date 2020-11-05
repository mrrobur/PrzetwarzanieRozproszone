using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Notifications.Services
{
    public class EmailSender
    {
        public string emailBody = "Nałożono kwarantanne od: ";
        public string emailSubject = "Informacja o nałożeniu kwarantanny";
        public void SendNewPatientEmail(string email, string dateStart)
        {
            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("devmail.kw@gmail.com", "9P4g$S$9@0K7%C"),
                EnableSsl = true
            };
            smtp.Send("devmail.kw@gmail.com", email, emailSubject, emailBody + dateStart);
        }
    }
}
