using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CSE.Models;

namespace Web_CSE.Controllers
{
    public class ITController : Controller
    {
        private readonly CnttCseContext _context;

        public ITController(CnttCseContext context)
        {
            _context = context;
        }

        // GET: IT
        public async Task<IActionResult> Index()
        {
            var cnttCseContext = _context.Posts.Where(p => p.CatId == 30);
            return View(await cnttCseContext.ToListAsync());
        }
    }
}
