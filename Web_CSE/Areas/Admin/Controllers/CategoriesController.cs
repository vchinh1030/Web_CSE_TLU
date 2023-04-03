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
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }
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
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x=>x.Levels == 1 && x.CatId != category.CatId), "CatId", "CatName", category.CatId);
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

        // private bool PostExists(int id)
        // {
        //   return _context.Categories.Any(e => e.CatId == id);
        // }
    }

}


   

    //     
    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> Create([Bind("CatId,Title,Contents,CreatedAt,ShortContent,AccountId,CatId,Thumb")] Post post,IFormFile fThumb)
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             post.Thumb = "default.jpg";
    //             post.Contents= Request.Form["Contents"];
    //             string pattern = "<img.*?>";
    //             string replacement = "";
    //             Regex rgx = new Regex(pattern);
    //             if (post.Contents.Length<= 150)
    //             {
    //                 post.ShortContent = rgx.Replace(post.Contents, replacement) + "...";
    //             }
    //             else
    //             {
                   
    //                 post.ShortContent = rgx.Replace(post.Contents, replacement).Substring(0, 150);
    //             }
               
    //             //post.ShortContent = post.Contents.Substring(0, 150).Trim() + "...";
    //             if (fThumb != null)
    //             {
    //                 string extension = Path.GetExtension(fThumb.FileName);
    //                 string image = Utilities.SEOUrl(post.Title) + extension;
    //                 post.Thumb = await Utilities.UploadFile(fThumb, @"Categories", image.ToLower());
    //             }
    //             post.Alias = Utilities.SEOUrl(post.Title);
    //             post.CreatedAt = DateTime.Now;
    //             _context.Add(post);
    //             await _context.SaveChangesAsync();
    //             return RedirectToAction(nameof(Index));
    //         }
    //         ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
    //         ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
    //         return View(post);
    //     }

    //     //[HttpPost]
    //     //public ActionResult UploadImage2(List<IFormFile> files)
    //     //{
    //     //    var filepath = "";
    //     //    foreach (IFormFile photo in Request.Form.Files)
    //     //        try
    //     //        {
    //     //            string serverMapPath = Path.Combine(_env.WebRootPath, "Image", photo.FileName);
    //     //            using (var stream = new FileStream(serverMapPath, FileMode.Create))
    //     //            {
    //     //                photo.CopyTo(stream);
    //     //            }
    //     //            filepath = HttpContext.Request.Host + "/images/Categories/contents/rich-text/" + photo.FileName;
    //     //        }
    //     //        catch (Exception ex)
    //     //        {
    //     //            BadRequest(ex.Message);
    //     //        }
    //     //    return Json(new { url = filepath });
    //     //}
    //     [HttpPost]
    //     public async Task<JsonResult> UploadImage()
    //     {
    //         try
    //         {
    //             var uploads = Path.Combine(_env.WebRootPath, "images/contents");
    //             var filePath = Path.Combine(uploads, "rich-text");
    //             var urls = new Dictionary<string,string>();
    //             var url = new List<string>();
    //             var random =  Utilities.GetRandomInt(5);
    //             //If folder of new key is not exist, create the folder.
    //             if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

    //             foreach (var contentFile in Request.Form.Files)
    //             {
    //                 if (contentFile != null && contentFile.Length > 0)
    //                 {
    //                     await contentFile.CopyToAsync(new FileStream($"{filePath}\\{random}{contentFile.FileName}", FileMode.Create));
    //                     urls.Add("url",$"https://{HttpContext.Request.Host}/images/contents/rich-text/{random}{contentFile.FileName}");
    //                 }
    //             }

    //             return Json(urls);
    //         }
    //         catch (Exception e)
    //         {
    //             return Json(new { error = new { message = e.Message } });
    //         }
    //     }
    //     // GET: Categories/Edit/5
    //     public async Task<IActionResult> Edit(int? id)
    //     {
    //         if (id == null || _context.Categories == null)
    //         {
    //             return NotFound();
    //         }

    //         var post = await _context.Categories.FindAsync(id);
    //         if (post == null)
    //         {
    //             return NotFound();
    //         }
    //         // ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
    //         // ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
    //         return View(post);
    //     }

    //     // POST: Categories/Edit/5
    //     // To protect from overposting attacks, enable the specific properties you want to bind to.
    //     // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //     [HttpPost]
    //     [ValidateAntiForgeryToken]
    //     public async Task<IActionResult> Edit(int id, [Bind("CatId,Title,Contents,CreatedAt,ShortContent,AccountId,CatId,Thumb")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
    //     {
    //         if (id != post.CatId)
    //         {
    //             return NotFound();
    //         }

    //         if (ModelState.IsValid)
    //         {
    //             string currentThumb = Request.Form["currentThumb"];
    //             //var currentPost = await _context.Categories.FindAsync(id);
    //             string pattern = "<img.*?>";
    //             string replacement = "";
    //             Regex rgx = new Regex(pattern);
    //             if (post.Contents.Length <= 150)
    //             {
    //                 post.ShortContent = rgx.Replace(post.Contents, replacement) + "...";
    //             }
    //             else
    //             {
                   
    //                 post.ShortContent = rgx.Replace(post.Contents, replacement).Substring(0, 150);
    //             }

    //             // post.ShortContent = post.Contents.Substring(0,150).Trim()+"...";
    //             string oldThumb = currentThumb;
    //             try
    //             {
                    
    //                 if (post.Contents == null) post.Contents = "Bi loi";
    //                 if (fThumb == null) post.Thumb = oldThumb;
    //                      else   //if (fThumb != null) 
    //                     {
    //                         string extension = Path.GetExtension(fThumb.FileName);
    //                         string image = Utilities.SEOUrl(post.Title) + extension;
    //                         post.Thumb = await Utilities.UploadFile(fThumb, @"Categories", image.ToLower());
    //                     }
                    
    //                 post.Alias = Utilities.SEOUrl(post.Title);
    //                 post.CreatedAt = DateTime.Now;

    //                 _context.Update(post);
    //                 await _context.SaveChangesAsync();
    //             }
    //             catch (DbUpdateConcurrencyException)
    //             {
    //                 // if (!PostExists(post.CatId))
    //                 // {
    //                 //     return NotFound();
    //                 // }
    //                 // else
    //                 // {
    //                 //     throw;
    //                 // }
    //             }
    //             return RedirectToAction(nameof(Index));
    //         }
    //         ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
    //         ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
    //         return View(post);
    //     }

    