using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Library.Data;
using System;
using System.Linq;


namespace Library.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LibraryContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<LibraryContext>>()))
            {
                if (context.Book.Any())
                {
                    return;   // DB has been seeded
                }

                context.Book.AddRange(
                    new Book
                    {
                        Title = "Can You Make This Thing Go Faster",
                        Author = "Jeremy Clarkson",
                        Publisher = "Penguin Random House UK",
                        Date = 2020,
                        User = "",
                        Reserved = "",
                        Leased = ""
                    },

                    new Book
                    {
                        Title = "Diddly Squat - a Year on the Farm",
                        Author = "Jeremy Clarkson",
                        Publisher = "Penguin Random House UK",
                        Date = 2020,
                        User = "",
                        Reserved = "",
                        Leased = ""
                    }
                );
                context.SaveChanges();
            }
        }
    }
}