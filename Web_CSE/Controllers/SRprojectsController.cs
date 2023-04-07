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
    public class SRprojectsController : Controller
    {
        private readonly CnttCseContext _context;

        public SRprojectsController(CnttCseContext context)
        {
            _context = context;
        }

        // GET: SRprojects
        public async Task<IActionResult> Index()
        {
            var cnttCseContext = _context.Posts.Where(p => p.CatId == 16);
            return View(await cnttCseContext.ToListAsync());
        }

        
    }
}
