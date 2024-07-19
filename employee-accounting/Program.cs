using employee_accounting.Database;
using employee_accounting.Objects;
using Newtonsoft.Json.Linq;
using employee_accounting.Generator;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace employee_accounting
{
    internal class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            using var context = new TestContext();
            string global_choice = string.Empty;
            
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Seems like no arguments specified, write them here.");
                    Console.Write("Write arguments:\n> ");
                    var input = Console.ReadLine()!.Split(" ");
                    var counter = -1;
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (input[i].Contains("\""))
                        {
                            if (counter == -1)
                            {
                                counter = i;
                            }
                            else
                            {
                                for (int j = counter + 1; j <= i; j++)
                                {
                                    input[counter] += $" {input[j]}";
                                    input[j] = null!;
                                }
                                counter = -1;
                            }
                        }
                    }
                    args = input
                        .Where(x => x is not null)
                        .Select(x => x.Trim().Replace("\"","")).ToArray();
                }
                else
                {
                    global_choice = args[0];
                }
            } catch
            {
                Console.WriteLine("Something went wrong");
                return;
            }
            switch (args[0])
            {
                case "1":
                    {
                        Console.WriteLine("DB created");
                        break;
                    }
                case "2":
                    {
                        try
                        {
                            var name = args[1];
                            var date = DateOnly.FromDateTime(DateTime.Parse(args[2]));
                            var sex = args[3].ToLower() == "female" ? 1 : 0;
                            using (var sqliteprinter = new DbPrinter(context))
                            {
                                Employee e = new Employee()
                                {
                                    Name = name,
                                    BirthDate = date,
                                    Sex = sex
                                };
                                await sqliteprinter.PutValue(e);
                                Console.WriteLine($"{e.Name} added!");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Specify arguments");
                        }
                        break;
                    }
                case "3":
                    {
                        var emps = context.Employees
                                .GroupBy(x => x.Name)
                                .Select(x => x.First());
                        foreach (var item in emps)
                        {
                            Console.WriteLine($"{item.Name} | {item.BirthDate}");
                        }
                        break;
                    }
                case "4":
                    {
                        Console.Write("You sure you want to create 1.000.000 DB records? May be ridiculously slow\n[Y/N]: ");
                        char choice = (char)Console.Read();
                        if (choice == 'y' || choice == 'Y')
                        {
                            Console.WriteLine("Ok, generating...");
                            using (var sqliteprinter = new DbPrinter(context))
                            {
                                var emps = await Generator.Generator.Generate(1000000);
                                await sqliteprinter.PutValue(emps);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Cancelled by user");
                        }

                        break;
                    }
                case "5":
                    {
                        // Time with no indexes is about ~286 ms
                        // Time with indexes is about ~278 ms

                        Stopwatch timer = new Stopwatch();
                        
                        Console.WriteLine("Started");
                        timer.Start();
                        var emps = context.Employees.Where(x => x.Sex == 0 && x.Name.StartsWith("F"));
                        Console.WriteLine($"Found {emps.Count()} elements");
                        timer.Stop();

                        Console.WriteLine($"Elapsed time: {timer.ElapsedMilliseconds} ms");
                        break;
                    }
            }
        }
    }
}
