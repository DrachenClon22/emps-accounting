using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace employee_accounting.Database
{
    public class DbPrinter : IDisposable
    {
        private DbContext _context;
        private bool disposed = false;

        public DbPrinter(DbContext context)
        {
            _context = context;
        }

        ~DbPrinter() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<bool> PutValue(object? obj)
        {
            Console.WriteLine("Writing to db...");
            if (obj is not null)
            {
                _context.Add(obj);
                await _context.SaveChangesAsync();
                Console.WriteLine("Success!");
                return true;
            } else
            {
                Console.WriteLine("Failed...");
                return false;
            }
        }

        public async Task<bool> PutValue(object[]? objs)
        {
            Console.WriteLine("Writing to db...");
            if (objs is not null)
            {
                int counter = 0;
                foreach (var item in objs)
                {
                    _context.Add(item);

                    counter++;
                    Console.WriteLine($"Progress: {counter}/{objs.Length}");

                    if (counter%100 == 0)
                    {
                        await _context.SaveChangesAsync();
                        Console.WriteLine("Packet sent");
                    }
                }
                await _context.SaveChangesAsync();
                Console.WriteLine("Success!");
                return true;
            }
            else
            {
                Console.WriteLine("Failed...");
                return false;
            }
        }
    }
}
