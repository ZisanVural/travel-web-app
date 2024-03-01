using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelApp.ViewModel;
using TravelApplication.Data;
using WebApplication3.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace TravelApp.Controllers
{
    [Authorize]
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;
        public AboutController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Abouts
        public async Task<IActionResult> Index()
        {
            return _context.Abouts != null ?
                        View(await _context.Abouts.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Abouts'  is null.");
        }

        // GET: Abouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Abouts == null)
            {
                return NotFound();
            }

            var about = await _context.Abouts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (about == null)
            {
                return NotFound();
            }

            return View(about);
        }

        // GET: Abouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Abouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ImageFile")] AboutVM about)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (about.ImageFile != null && about.ImageFile.Length > 0)
                    {
                        var filename = Guid.NewGuid() + "_" + about.ImageFile.FileName ;//tekil isim olsun diye guid eklendi.

                        var filePath = Path.Combine(hostingEnvironment.ContentRootPath, "uploads\\" + filename);//dosyayı uploads klasörüne kayıt yapılır.

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await about.ImageFile.CopyToAsync(stream);

                            var record = new About
                            {
                                Description = about.Description,
                                Title = about.Title,
                                ImagePath = "uploads/" + filename
							};
                            _context.Add(record);

                            var result = await _context.SaveChangesAsync();
                        }

                        return RedirectToAction(nameof(Index));
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(about);

        }



        // GET: Abouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Abouts == null)
            {
                return NotFound();
            }

            var about = await _context.Abouts.FindAsync(id);
            if (about == null)
            {
                return NotFound();
            }
            var vm = new AboutVM
            {
                Id = about.Id,
                Title = about.Title,
                Description = about.Description

            };
            return View(vm);
        }

        // POST: Abouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ImageFile")] AboutVM about)
        {
            if (id != about.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (about.ImageFile != null && about.ImageFile.Length > 0)
                    {
                        var filename = Guid.NewGuid()+"_"+ about.ImageFile.FileName ;

                        var filePath = Path.Combine(hostingEnvironment.ContentRootPath, "uploads\\" + filename);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await about.ImageFile.CopyToAsync(stream);

                            var record = new About
                            {
                                Id = id,
                                Description = about.Description,
                                Title = about.Title,
                                ImagePath = "uploads/" + filename
							};
                            _context.Update(record);

                            var result = await _context.SaveChangesAsync();
                        }

                       
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutExists(about.Id))
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
            return View(about);
        }

        // GET: Abouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Abouts == null)
            {
                return NotFound();
            }

            var about = await _context.Abouts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (about == null)
            {
                return NotFound();
            }

            return View(about);
        }

        // POST: Abouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Abouts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Abouts'  is null.");
            }
            var about = await _context.Abouts.FindAsync(id);
            if (about != null)
            {
				var filePath = Path.Combine(hostingEnvironment.ContentRootPath, about.ImagePath.Replace("/","\\"));
				_context.Abouts.Remove(about);
                if(System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutExists(int id)
        {
            return (_context.Abouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
