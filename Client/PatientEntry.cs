using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NS = Newtonsoft.Json;


namespace Client
{
    class PatientEntry
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("------------------ COVID Pacjenci --------------------");
            Console.WriteLine("\nDostępne opcje: \t L - wyświetl listę pacjentów \t D - dodaj pacjenta \t Q - wyjdź z programu");
            Console.WriteLine("Wybierz opcję: ");


            HttpClient client = new HttpClient();

            string line = "";

            while ((line = Console.ReadLine().ToUpper()) != "Q")
            {

                switch (line)
                {
                        case "L":
                        Console.WriteLine("Ładowanie... \n");

                        var response = await client.GetStringAsync("https://localhost:44305/api/patients");

                        dynamic array = NS.JsonConvert.DeserializeObject(response);
                        Console.WriteLine("Lista pacjentów:");
                        Console.WriteLine(" {0,10} | {1,20} | {2,5} | {3,40} | {4,20}", "Imię", "Nazwisko", "Wiek", "E-mail","Kwarantanna od");

                        foreach (var item in array)
                        {
                            Console.WriteLine(" {0,10} | {1,20} | {2,5} | {3,40} |  {4,20}", item.name, item.surname, item.age, item.email, item.startDate);
                        }

                        Console.Write("\nWybierz opcję: ");

                        break;
                    case "D":
                        Console.WriteLine("\nWprowadzanie nowego pacjenta");
                        Console.Write("Imię: ");
                        string name = Console.ReadLine();

                        Console.Write("Nazwisko: ");
                        string surname = Console.ReadLine();
                       
                        int ageInt;
                        bool parsable;

                        // Get and validate age
                        do
                        {
                            Console.Write("Wiek: ");
                            string age = Console.ReadLine();

                            parsable = Int32.TryParse(age, out ageInt);

                            if (!parsable)
                                Console.WriteLine(age + " nie jest numerem. Wpisz wartość liczbową");

                        } while (!parsable);

                        // Get and validate E-mail
                        string email;
                        bool valid;
                        do
                        {
                            Console.Write("E-mail: ");
                            email = Console.ReadLine();

                            valid = IsValidEmail(email);

                            if (!valid)
                                Console.WriteLine(email + " nie jest poprawnym adresem e-mail");

                        } while (!valid);

                        Patients p = new Patients()
                        {
                            name = name,
                            surname = surname,
                            age = ageInt,
                            email = email,
                            startDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };


                        string patientJson = JsonSerializer.Serialize(p);


                        var status = await client.PostAsync("https://localhost:44305/api/patients",
                            new StringContent(patientJson, Encoding.UTF8, "application/json"));

                        if (status.IsSuccessStatusCode)
                        {
                            Console.Write("Dodano pacjenta. Response(" + status.StatusCode + "). Zlecono wysyłkę maila");
                        }
                        else
                        {
                            Console.Write("Błąd dodawania. Response(" + status.StatusCode + ")");
                        }

                        Console.Write("\n\nWybierz opcję: ");

                        break;

                    default:
                        Console.WriteLine("\n"+line + " nie jest poprawną opcją");
                        Console.WriteLine("Dostępne opcje: \t L - wyświetl listę pacjentów \t D - dodaj pacjenta \t Q - wyjdź z programu");
                        Console.Write("Wybierz opcję: ");

                        break;
                }

                bool IsValidEmail(string email)
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(email);
                        return addr.Address == email;
                    }
                    catch
                    {
                        return false;
                    }
                }

            }

            

        //Console.WriteLine(responseJson.name);

        }
    }


    public class Patients
    {
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        public string startDate { get; set; }
    }
}
