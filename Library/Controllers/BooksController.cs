using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.Models;
using Library.Areas.Identity.Pages.Account.Manage;
using Microsoft.AspNetCore.Identity;
using Library.Areas.Identity.Data;
using System.Security.Claims;

namespace Library.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            expireReservations();
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Publisher,Author,Date,User,Reserved,Leased")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Publisher,Author,Date,User,Reserved,Leased")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Reserve(int id)
        {
            if (_context.Book == null)
            {
               return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                DateTime date = DateTime.Today;
                DateTime newDate = date.AddDays(1);
                book.Reserved = newDate.ToShortDateString();
                var name = User.FindFirst(ClaimTypes.Name).Value;
                book.User = name;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Lease(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                DateTime date = DateTime.Today;
                book.Leased = date.ToShortDateString();
                book.Reserved = "";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexReservedBooks));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return _context.Book.Any(e => e.Id == id);
        }

        private void expireReservations()
        {
            string todaysDate = DateTime.Today.ToShortDateString();
            foreach(var book in _context.Book)
            {
                if (string.Compare(book.Reserved, todaysDate) < 0)
                {
                    book.Reserved = "";
                    book.User = "";
                }
            }
            return;
        }

        public async Task<IActionResult> IndexMyBooks()
        {
            expireReservations();
            var myBooks = from s in _context.Book.Where(a => a.User == this.User.Identity.Name && a.Reserved != "") select s;
            return View(await myBooks.ToListAsync());
        }

        public async Task<IActionResult> IndexReservedBooks()
        {
            expireReservations();
            var reservedBooks = from s in _context.Book.Where(a => a.Reserved != "") select s;
            return View(await reservedBooks.ToListAsync());
        }

        public async Task<IActionResult> IndexLeasedBooks()
        {
            expireReservations();
            var leasedBooks = from s in _context.Book.Where(a => a.Leased != "") select s;
            return View(await leasedBooks.ToListAsync());
        }

        

        public async Task<IActionResult> MakeAvailable(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                book.Leased = "";
                book.User = "";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexLeasedBooks));
        }

        public async Task<IActionResult> CancelReservation(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                book.Reserved = "";
                book.User = "";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexMyBooks));
        }

    }
}
