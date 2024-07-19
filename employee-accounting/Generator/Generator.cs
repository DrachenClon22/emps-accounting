using employee_accounting.Database;
using employee_accounting.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace employee_accounting.Generator
{
    public static class Generator
    {
        /// <summary>
        /// Generates <i>amount</i> of people, not the best solution,
        /// but fast enough, anyway this code is disposable, why not
        /// </summary>
        public static async Task<Employee[]> Generate(int amount)
        {
            Console.WriteLine("Generating employees, first 100 may be a little slow...");
            Employee[] employees = new Employee[amount];
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                var responce = new HttpResponseMessage();
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        request = new HttpRequestMessage(HttpMethod.Get, "https://randomuser.me/api/");
                        responce = await client.SendAsync(request);
                        JObject obj = JObject.Parse(await responce.Content.ReadAsStringAsync());
                        Employee e = new Employee
                        {
                            Name =
                            $"{obj.SelectToken("results[0].name.first")} " +
                            $"{obj.SelectToken("results[0].name.last")}",
                            Sex = obj.SelectToken("results[0].gender")?.ToString() == "male" ? 0 : 1,
                            BirthDate = DateOnly.FromDateTime(DateTime.Parse(obj.SelectToken("results[0].dob.date")?.ToString()!))
                        };
                        if (!Regex.IsMatch(e.Name, "^[a-zA-Z ]*$"))
                        {
                            i--;
                            continue;
                        }
                        employees[i] = e;
                        Console.WriteLine($"{i + 1}/{amount} {e.Name} {e.BirthDate} {(e.Sex == 0 ? "MALE" : "FEMALE")}");

                        if (amount > 100)
                        {
                            employees[i] = e;
                        }
                        if (i >= amount)
                        {
                            break;
                        }
                    }
                    if (amount > 100)
                    {
                        var rand = new Random();
                        var getRandEmp = () => employees[rand.Next(0, 100)];
                        for (int i = 100; i < amount; i++)
                        {
                            Employee e = new Employee
                            {
                                Name = getRandEmp().Name,
                                Sex = getRandEmp().Sex,
                                BirthDate = getRandEmp().BirthDate
                            };
                            employees[i] = e;

                            Console.WriteLine($"{i + 1}/{amount} {e.Name} {e.BirthDate} {(e.Sex == 0 ? "MALE" : "FEMALE")}");
                        }
                    }
                    Console.WriteLine("Done!");
                }
                finally
                {
                    responce.Dispose();
                    request.Dispose();
                }
            }
            for (int i = 0; i < ((employees.Length > 100) ? 100 : employees.Length); i++)
            {
                employees[i].Name = 'F' + employees[i].Name.Substring(1);
                employees[i].Sex = 0;
            }
            return employees;
        }
    }
}
