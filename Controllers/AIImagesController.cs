using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WDP2024Assignment2.Data;
using WDP2024Assignment2.Models;

namespace WDP2024Assignment2.Controllers
{
    public class AIImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AIImagesController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AIImagesController(ApplicationDbContext context, ILogger<AIImagesController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: AIImages
        public async Task<IActionResult> Index()
        {
            return View(await _context.AIImage.ToListAsync());
        }

        // GET: AIImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aIImage == null)
            {
                return NotFound();
            }

            return View(aIImage);
        }

        // GET: AIImages/Create
        public IActionResult Create()
        {
            AIImage aIImage = new();
            aIImage.UploadDate = DateTime.Today;
            return View(aIImage);
        }

        // POST: AIImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Prompt,ImageGenerator,UploadDate,Filename,Like,canIncreaseLike")] AIImage aIImage, IFormFile Filename)
        {
            if (ModelState.IsValid)
            {
                if (Filename != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Filename.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        Filename.CopyTo(fileStream);
                    }

                    aIImage.Filename = "/images/" + uniqueFileName;
                }

                _context.Add(aIImage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(aIImage);
        }

        // GET: AIImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage.FindAsync(id);
            if (aIImage == null)
            {
                return NotFound();
            }
            return View(aIImage);
        }

        // POST: AIImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Prompt,ImageGenerator,UploadDate,Filename,Like,canIncreaseLike")] AIImage aIImage, IFormFile Filename)
        {
            if (id != aIImage.Id)
            {
                return NotFound();
            }

            AIImage data = new();
            data.Id = aIImage.Id;
            data.Prompt = aIImage.Prompt;
            data.ImageGenerator = aIImage.ImageGenerator;
            data.Like = aIImage.Like;
            if (Filename != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Filename.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Filename.CopyTo(fileStream);
                }

                data.Filename = "/images/" + uniqueFileName;
            }
            try
            {
                _context.Update(data);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AIImageExists(aIImage.Id))
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

        // GET: AIImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aIImage = await _context.AIImage
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aIImage == null)
            {
                return NotFound();
            }

            return View(aIImage);
        }

        // POST: AIImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var aIImage = await _context.AIImage.FindAsync(id);
            if (aIImage != null)
            {
                _context.AIImage.Remove(aIImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AIImageExists(int id)
        {
            return _context.AIImage.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Like(int Id)
        {
            try
            {
                var data = await _context.AIImage.Where(x => x.Id == Id).FirstOrDefaultAsync();
                data.Like = data.Like + 1;
                _context.AIImage.Update(data);
                await _context.SaveChangesAsync();
                ViewData["SuccessMessage"] = "GenAI Liked Succesfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ViewData["ErrorMessage"] = "Error Ocurred.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
