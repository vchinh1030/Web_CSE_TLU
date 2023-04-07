using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CSE.Models;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Web_CSE.Helpers;
using PagedList.Core;

namespace Web_CSE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
     public class CategoriesController : Controller
    {
        private readonly CnttCseContext _context;
        private readonly IWebHostEnvironment _env;
        //private readonly IWebHostEnvironment _hostingEnvironment;

        public CategoriesController(CnttCseContext context, IWebHostEnvironment env/*, IWebHostEnvironment hostingEnvironment*/)
        {
            _context = context;
            _env = env;
            //_hostingEnvironment = hostingEnvironment;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            // var cnttCseContext = _context.Categories.Include(p => p.Account).Include(p => p.Cat);
            return View(await _context.Categories.ToListAsync());
        }
        
        // public IActionResult Index(int? page) {
        //     var pageNumber = page == null || page <= 0?1 : page.Value;
        //     var pageSize = Utilities.PAGE_SIZE;
        //     var IsCategories = _context.Categories
        //             .OrderByDescending(x => x.CatId);
        //     PagedList<Category> models = new PagedList<Category>(lsCategories, pageNumber, pageSize);
        //     ViewBag.CurrentPage = pageNumber;
        //     return View(models);

        // }
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(m => m.CatId == id);

            if (category == null)
            {
                return NotFound();
            }

            ViewBag.ParentCategoryName = category.ParentCategory == null ? "DANH MỤC GỐC" : category.ParentCategory.CatName;

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x=>x.Levels == 1), "CatId", "CatName");
            return View();
        }
            // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Title,Alias,Thumb,Ordering,Parent,Levels")] Category category, IFormFile fThumb) 
        {
            if(ModelState.IsValid)
            {
                category.Alias = Utilities.SEOUrl(category.CatName);
                if(category.Parent == null)
                {
                    category.Levels = 1;
                }
                else
                {
                    category.Levels = category.Parent == 0 ? 1:2;
                }
                if(fThumb !=  null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string Newname = Utilities.SEOUrl(category.CatName) + "preview_" + extension;
                    category.Thumb = await Utilities.UploadFile(fThumb,@"categories\", Newname.ToLower());
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        // GET: Categories/Edit/5
       public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories.Where(x => x.Levels == 1 && x.CatId != category.CatId).ToListAsync();
            categories.Insert(0, new Category { CatId = 0, CatName = "DANH MỤC GỐC" });
            ViewData["DanhMucGoc"] = new SelectList(categories, "CatId", "CatName", category.Parent ?? 0);
            return View(category);
        }


        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Title,Alias,Thumb,Ordering,Parent,Levels")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {   
                try{
                    category.Alias = Utilities.SEOUrl(category.CatName);
                    if(category.Parent == null)
                    {
                        category.Levels = 1;
                    }
                    else
                    {
                        category.Levels = category.Parent == 0 ? 1:2;
                    }
                    if(fThumb !=  null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string Newname = Utilities.SEOUrl(category.CatName) + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(fThumb,@"categories\", Newname.ToLower());
                    }
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException){
                    // if(!CategoryExists(category.CatId))
                    // {
                    //     return NotFound();
                    // }
                    // else{
                    //     throw;
                    // }
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            // ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'CnttCseContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
