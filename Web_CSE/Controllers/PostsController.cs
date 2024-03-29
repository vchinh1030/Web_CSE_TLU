﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CSE.Helpers;
using Web_CSE.Models;

namespace Web_CSE.Controllers
{
    public class PostsController : Controller
    {
        private readonly CnttCseContext _context;
        private readonly IWebHostEnvironment _env;
        //private readonly IWebHostEnvironment _hostingEnvironment;

        public PostsController(CnttCseContext context, IWebHostEnvironment env/*, IWebHostEnvironment hostingEnvironment*/)
        {
            _context = context;
            _env = env;
            //_hostingEnvironment = hostingEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var cnttCseContext = _context.Posts.Include(p => p.Account).Include(p => p.Cat);
            return View(await cnttCseContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName");
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Contents,CreatedAt,ShortContent,AccountId,CatId,Thumb")] Post post,IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                post.Thumb = "default.jpg";
                post.Contents= Request.Form["Contents"];
                string pattern = "<img.*?>";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                if (post.Contents.Length<= 150)
                {
                    post.ShortContent = rgx.Replace(post.Contents, replacement) + "...";
                }
                else
                {
                   
                    post.ShortContent = rgx.Replace(post.Contents, replacement).Substring(0, 150);
                }
               
                //post.ShortContent = post.Contents.Substring(0, 150).Trim() + "...";
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(post.Title) + extension;
                    post.Thumb = await Utilities.UploadFile(fThumb, @"posts", image.ToLower());
                }
                post.Alias = Utilities.SEOUrl(post.Title);
                post.CreatedAt = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        //[HttpPost]
        //public ActionResult UploadImage2(List<IFormFile> files)
        //{
        //    var filepath = "";
        //    foreach (IFormFile photo in Request.Form.Files)
        //        try
        //        {
        //            string serverMapPath = Path.Combine(_env.WebRootPath, "Image", photo.FileName);
        //            using (var stream = new FileStream(serverMapPath, FileMode.Create))
        //            {
        //                photo.CopyTo(stream);
        //            }
        //            filepath = HttpContext.Request.Host + "/images/posts/contents/rich-text/" + photo.FileName;
        //        }
        //        catch (Exception ex)
        //        {
        //            BadRequest(ex.Message);
        //        }
        //    return Json(new { url = filepath });
        //}
        [HttpPost]
        public async Task<JsonResult> UploadImage()
        {
            try
            {
                var uploads = Path.Combine(_env.WebRootPath, "images/contents");
                var filePath = Path.Combine(uploads, "rich-text");
                var urls = new Dictionary<string,string>();
                var url = new List<string>();
                var random =  Utilities.GetRandomInt(5);
                //If folder of new key is not exist, create the folder.
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

                foreach (var contentFile in Request.Form.Files)
                {
                    if (contentFile != null && contentFile.Length > 0)
                    {
                        await contentFile.CopyToAsync(new FileStream($"{filePath}\\{random}{contentFile.FileName}", FileMode.Create));
                        urls.Add("url",$"https://{HttpContext.Request.Host}/images/contents/rich-text/{random}{contentFile.FileName}");
                    }
                }

                return Json(urls);
            }
            catch (Exception e)
            {
                return Json(new { error = new { message = e.Message } });
            }
        }
        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Contents,CreatedAt,ShortContent,AccountId,CatId,Thumb")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string currentThumb = Request.Form["currentThumb"];
                //var currentPost = await _context.Posts.FindAsync(id);
                string pattern = "<img.*?>";
                string replacement = "";
                Regex rgx = new Regex(pattern);
                if (post.Contents.Length <= 150)
                {
                    post.ShortContent = rgx.Replace(post.Contents, replacement) + "...";
                }
                else
                {
                   
                    post.ShortContent = rgx.Replace(post.Contents, replacement).Substring(0, 150);
                }

                // post.ShortContent = post.Contents.Substring(0,150).Trim()+"...";
                string oldThumb = currentThumb;
                try
                {
                    
                    if (post.Contents == null) post.Contents = "Trang hiện đang lỗi, vui lòng thử lại sau.";
                    if (fThumb == null) post.Thumb = oldThumb;
                         else   //if (fThumb != null) 
                        {
                            string extension = Path.GetExtension(fThumb.FileName);
                            string image = Utilities.SEOUrl(post.Title) + extension;
                            post.Thumb = await Utilities.UploadFile(fThumb, @"posts", image.ToLower());
                        }
                    
                    post.Alias = Utilities.SEOUrl(post.Title);
                    post.CreatedAt = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'CnttCseContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
