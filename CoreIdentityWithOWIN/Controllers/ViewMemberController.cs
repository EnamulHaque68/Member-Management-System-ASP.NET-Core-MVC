using CoreIdentityWithOWIN.DTOS;
using CoreIdentityWithOWIN.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreIdentityWithOWIN.Controllers
{
    [Authorize] 
    public class ViewMemberController : Controller
    {
        private readonly AppDbContext _db;

        public ViewMemberController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string searchEnrolled, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ActiveSortParm"] = sortOrder == "IsActive" ? "actve_desc" : "IsActive";

            if (searchString != null || searchEnrolled != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["ActiveFilter"] = searchEnrolled;

            var studentQuery = _db.Members
                .Include(s => s.Transactions)
                .Include(c => c.MemberType)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                studentQuery = studentQuery.Where(s => s.MemberName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(searchEnrolled))
            {
                bool IsActive = searchEnrolled == "true";
                studentQuery = studentQuery.Where(s => s.IsActive == IsActive);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    studentQuery = studentQuery.OrderByDescending(s => s.MemberName);
                    break;
                case "Actived":
                    studentQuery = studentQuery.OrderBy(s => s.IsActive);
                    break;
                case "actve_desc":
                    studentQuery = studentQuery.OrderByDescending(s => s.IsActive);
                    break;
                default:
                    studentQuery = studentQuery.OrderBy(s => s.MemberName);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Member>.CreateAsync(studentQuery, pageNumber ?? 1, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}