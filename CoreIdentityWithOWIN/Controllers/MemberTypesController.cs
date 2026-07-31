using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoreIdentityWithOWIN.Models;
using CoreIdentityWithOWIN.DTOS;

namespace CoreIdentityWithOWIN.Controllers
{
    public class MemberTypesController : Controller
    {
        private readonly AppDbContext _context;

        public MemberTypesController(AppDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> Index()
        {
            return View(await _context.MemberTypes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberType = await _context.MemberTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (memberType == null)
            {
                return NotFound();
            }

            return View(memberType);
        }

        // GET: MemberTypes/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TypeId,Title")] MemberType memberType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(memberType);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberType = await _context.MemberTypes.FindAsync(id);
            if (memberType == null)
            {
                return NotFound();
            }
            return View(memberType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TypeId,Title")] MemberType memberType)
        {
            if (id != memberType.TypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberTypeExists(memberType.TypeId))
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
            return View(memberType);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberType = await _context.MemberTypes
                .FirstOrDefaultAsync(m => m.TypeId == id);
            if (memberType == null)
            {
                return NotFound();
            }

            return View(memberType);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberType = await _context.MemberTypes.FindAsync(id);
            if (memberType != null)
            {
                _context.MemberTypes.Remove(memberType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberTypeExists(int id)
        {
            return _context.MemberTypes.Any(e => e.TypeId == id);
        }
    }
}
