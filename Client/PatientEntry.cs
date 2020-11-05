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
            HttpClient client = new HttpClient();

            Console.WriteLine("Lista pacjentów \n");

            var response = await client.GetStringAsync("https://localhost:44305/api/patients");

            //Console.WriteLine(response);

            dynamic array = NS.JsonConvert.DeserializeObject(response);
            Console.WriteLine(" {0,10} | {1,20} | {2,5} | {3,10}", "Imię", "Nazwisko","Wiek","E-mail");


            foreach (var item in array)
            {
                Console.WriteLine(" {0,10} | {1,20} | {2,5} | {3,10}", item.name, item.surname, item.age, item.email);
            }
            // JObject responseJson = JObject.Parse(response);
            //List<Item> items = NS.JsonConvert.DeserializeObject<List<Item>>(response);

          


        //Console.WriteLine(responseJson.name);


        Console.WriteLine("\nWprowadzanie nowego pacjenta");
            Console.Write("Imię: ");
            string name = Console.ReadLine();

            Console.Write("Nazwisko: ");
            string surname = Console.ReadLine();
            int ageInt;
            bool parsable;

            do
            {
                Console.Write("Wiek: ");
                string age = Console.ReadLine();

                parsable = Int32.TryParse(age, out ageInt);

                if (!parsable)
                    Console.WriteLine(age+" nie jest numerem. Wpisz wartość liczbową");

            } while (!parsable);

            Patients p = new Patients()
            {
                name = name,
                surname = surname,
                age = ageInt,
                email = "test@email.com"
            };


            string patientJson = JsonSerializer.Serialize(p);

            await client.PostAsync("https://localhost:44305/api/patients",
                new StringContent(patientJson, Encoding.UTF8, "application/json"));

           

        }
    }


    public class Patients
    {
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public string email { get; set; }
    }
}
