using IdentityModel.Client;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            var app = PublicClientApplicationBuilder.Create("0a862973-d546-4aca-a248-1d6743ab05cd")
                .WithAuthority("https://login.microsoftonline.com/61a84301-8c97-40f0-aa97-66d871c63d8f/v2.0/")
                .WithDefaultRedirectUri()
                .Build();

            Console.WriteLine("------------------ COVID Pacjenci --------------------");

            Console.WriteLine("Wymagana autentykacja. Za chwilę nastąpi przekierowanie do logowania");
            System.Threading.Thread.Sleep(2000);

            string token = "";
            var token_api = await app.AcquireTokenInteractive(new[] { "api://0a862973-d546-4aca-a248-1d6743ab05cd/.default" }).ExecuteAsync();
            token = token_api.AccessToken;
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);
            if (token.Length > 0)
            {
                Console.WriteLine("Autentykacja udana");
            }


            Console.WriteLine("\nDostępne opcje: \t L - wyświetl listę pacjentów \t D - dodaj pacjenta \t Q - wyjdź z programu");
            Console.WriteLine("Wybierz opcję: ");




            string line = "";

            while ((line = Console.ReadLine().ToUpper()) != "Q")
            {

                switch (line)
                {
                        case "L":
                        Console.WriteLine("Ładowanie... \n");

                        var response = await client.GetStringAsync("https://localhost:44305/api/patients");

                       // string unauth = "Response status code does not indicate success: 401 (Unauthorized).";

     
                        dynamic array = NS.JsonConvert.DeserializeObject(response);
                        Console.WriteLine("Lista pacjentów:");
                        Console.WriteLine("{5,5} | {0,10} | {1,20} | {2,5} | {3,40} | {4,20}", "Imię", "Nazwisko", "Wiek", "E-mail", "Kwarantanna od", "ID");

                        foreach (var item in array)
                        {
                            Console.WriteLine("{5,5} | {0,10} | {1,20} | {2,5} | {3,40} |  {4,20}", item.name, item.surname, item.age, item.email, item.startDate, item.id);
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

                        //var ex = await client.PutAsync("https://localhost:44305/api/patients",
                        //    new StringContent(patientJson, Encoding.UTF8, "application/json"));

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

                    case "E":
                        var t = "fff";
                        var ex = await client.PutAsync("https://localhost:44305/api/patients",
                            new StringContent(t, Encoding.UTF8, "application/json"));
                        //throw new InvalidOperationException("Test exception");
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


    private static async Task<string> GetToken()
    {
    using var client = new HttpClient();

     DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
    {

    Address = "https://login.microsoftonline.com/61a84301-8c97-40f0-aa97-66d871c63d8f/v2.0/",

    //PR
    //Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",

    Policy =
    {
    ValidateEndpoints = false
    }
    });

     if (disco.IsError)
    throw new InvalidOperationException(
    $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "0a862973-d546-4aca-a248-1d6743ab05cd",
                ClientSecret = "6YiV7dLd1L..X~_kQ7CiSY8tAXc8W.j2R4",
                Scope = "api://0a862973-d546-4aca-a248-1d6743ab05cd/.default"
            };

            //PR
            //var tokenRequest = new ClientCredentialsTokenRequest
            //{
            //Address = disco.TokenEndpoint,
            //ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
            //ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
            //Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            //};


            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

     if (token.IsError)
    throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

     return token.AccessToken;
    }

    }


    public class Patients
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public int age { get; set; }
        public string email { get; set; }
        public string startDate { get; set; }
    }
}
