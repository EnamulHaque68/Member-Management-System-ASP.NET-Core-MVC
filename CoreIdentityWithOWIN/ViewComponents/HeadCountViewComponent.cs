using CoreIdentityWithOWIN.DTOS;
using CoreIdentityWithOWIN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CoreIdentityWithOWIN.ViewComponents
{
    public class HeadCountViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;
        public HeadCountViewComponent(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courseCount = await _db.Members.Include(c => c.MemberType).GroupBy(s => new { s.TypeId, s.MemberType.Title }).Select(g => new CourseHeadCount
            {
                TypeId = g.Key.TypeId,
                Title = g.Key.Title,
                Count = g.Count(),
                TotalCollection = g.Sum(s => s.RegFee)
            }).ToListAsync();
            return View(courseCount);
        }
    }
}
