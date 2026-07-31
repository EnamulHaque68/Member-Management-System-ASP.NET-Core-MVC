using CoreIdentityWithOWIN.DTOS;
using CoreIdentityWithOWIN.Models;
using CoreIdentityWithOWIN.Models.ViewModels;
using CoreIdentityWithOWIN.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoreIdentityWithOWIN.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class MembersController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IMemberRepository _repo;
        private readonly IWebHostEnvironment _web;

        public MembersController(AppDbContext db, IMemberRepository repo, IWebHostEnvironment web)
        {
            _db = db;
            _repo = repo;
            _web = web;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string searchActived, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["ActiveSortParm"] = sortOrder == "IsActive" ? "actve_desc" : "IsActive";

            if (searchString != null || searchActived != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["ActiveFilter"] = searchActived;

            var MemberQuery = _db.Members
                .Include(s => s.Transactions)
                .Include(c => c.MemberType)
                .AsNoTracking();

            if (!String.IsNullOrEmpty(searchString))
            {
                MemberQuery = MemberQuery.Where(s => s.MemberName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(searchActived))
            {
                bool IsActive = searchActived == "true";
                MemberQuery = MemberQuery.Where(s => s.IsActive == IsActive);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    MemberQuery = MemberQuery.OrderByDescending(s => s.MemberName);
                    break;
                case "Actived":
                    MemberQuery = MemberQuery.OrderBy(s => s.IsActive);
                    break;
                case "acte_desc":
                    MemberQuery = MemberQuery.OrderByDescending(s => s.IsActive);
                    break;
                default:
                    MemberQuery = MemberQuery.OrderBy(s => s.MemberName);
                    break;
            }

            int pageSize = 3;
            return View(await PaginatedList<Member>.CreateAsync(MemberQuery, pageNumber ?? 1, pageSize));
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var member = new MemberViewModel();
            member.MemberTypes = _repo.GetMemberTypes().ToList();
            member.Transactions = new List<Transaction>();
            member.JointDate = DateTime.Today;
            member.IsActive = true;
            member.Transactions.Add(new Transaction() { BookName = "", Duration = 0 });

            return View(member);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreatePartial()
        {
            var member = new MemberViewModel();
            member.MemberTypes = _repo.GetMemberTypes().ToList();
            member.Transactions = new List<Transaction>();
            member.JointDate = DateTime.Today;
            member.IsActive = true;
            member.Transactions.Add(new Transaction() { BookName = "", Duration = 0 });

            return PartialView("_CreateMemberPartial", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult CreateMember([FromForm] MemberViewModel vobj)
        {
            try
            {
                if (vobj.Transactions != null && vobj.Transactions.Any())
                {
                    vobj.Transactions = vobj.Transactions
                        .Where(t => t != null && !string.IsNullOrWhiteSpace(t.BookName))
                        .ToList();
                }
                else
                {
                    vobj.Transactions = new List<Transaction>();
                }

                if (vobj.JointDate == DateTime.MinValue || vobj.JointDate.Year < 1900)
                {
                    vobj.JointDate = DateTime.Today;
                }

                ModelState.Remove("Transactions");
                ModelState.Remove("MemberTypes");
                ModelState.Remove("ImageUrl");
                ModelState.Remove("Title");

                if (!ModelState.IsValid)
                {
                    vobj.MemberTypes = _repo.GetMemberTypes().ToList();
                    return Json(new { success = false, errors = GetModelStateErrors(ModelState), redirectUrl = "" });
                }

                Member member = new Member
                {
                    MemberName = vobj.MemberName,
                    JointDate = vobj.JointDate,
                    MobileNo = vobj.MobileNo,
                    TypeId = vobj.TypeId,
                    IsActive = vobj.IsActive,
                    RegFee = vobj.RegFee,
                    Transactions = vobj.Transactions
                };

                if (vobj.ProfileFile != null && vobj.ProfileFile.Length > 0)
                {
                    string uniqueFileName = GetFileName(vobj.ProfileFile);
                    member.ImageUrl = uniqueFileName;
                }
                else
                {
                    member.ImageUrl = "noimages.png";
                }

                _repo.AddMember(member);
                return Json(new { success = true, redirectUrl = Url.Action("Index") });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, errors = new[] { innerException }, redirectUrl = "" });
            }
        }

        private string GetFileName(IFormFile profileFile)
        {
            string uniqueFileName = null;
            if (profileFile != null && profileFile.Length > 0)
            {
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(profileFile.FileName);
                var uploadFolder = Path.Combine(_web.WebRootPath, "Images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    profileFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private object GetModelStateErrors(ModelStateDictionary modelState)
        {
            return modelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteMember(int id)
        {
            try
            {
                Member member = _repo.GetStudentByMemberId(id);
                if (member != null)
                {
                    _repo.DeleteTransactionByMemberId(id);
                    _repo.DeleteStudentByMemberId(id);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Member not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditPartial(int id)
        {
            var student = _repo.GetStudentByMemberId(id);
            if (student == null)
            {
                return NotFound();
            }

            var vObj = new MemberViewModel
            {
                MemberId = student.MemberId,
                MemberName = student.MemberName,
                JointDate = student.JointDate,
                MobileNo = student.MobileNo,
                TypeId = student.TypeId,
                IsActive = student.IsActive,
                ImageUrl = student.ImageUrl,
                RegFee = student.RegFee,
                Transactions = student.Transactions?.ToList() ?? new List<Transaction>(),
                MemberTypes = _repo.GetMemberTypes().ToList()
            };
            return PartialView("_EditMemberPartial", vObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public JsonResult EditMember([FromForm] MemberViewModel vobj, string OldImageUrl)
        {
            try
            {
                if (vobj.Transactions != null && vobj.Transactions.Any())
                {
                    vobj.Transactions = vobj.Transactions
                        .Where(t => t != null && !string.IsNullOrWhiteSpace(t.BookName))
                        .ToList();
                }
                else
                {
                    vobj.Transactions = new List<Transaction>();
                }

                if (vobj.JointDate == DateTime.MinValue || vobj.JointDate.Year < 1900)
                {
                    vobj.JointDate = DateTime.Today;
                }

                ModelState.Remove("Transactions");
                ModelState.Remove("MemberTypes");
                ModelState.Remove("ImageUrl");
                ModelState.Remove("Title");

                if (!ModelState.IsValid)
                {
                    vobj.MemberTypes = _repo.GetMemberTypes().ToList();
                    return Json(new { success = false, errors = GetModelStateErrors(ModelState) });
                }

                Member obj = _repo.GetStudentByMemberId(vobj.MemberId);
                if (obj != null)
                {
                    obj.MemberName = vobj.MemberName;
                    obj.TypeId = vobj.TypeId;
                    obj.MobileNo = vobj.MobileNo;
                    obj.IsActive = vobj.IsActive;
                    obj.JointDate = vobj.JointDate;
                    obj.RegFee = vobj.RegFee;

                    if (vobj.ProfileFile != null && vobj.ProfileFile.Length > 0)
                    {
                        string uniqueFileName = GetFileName(vobj.ProfileFile);
                        obj.ImageUrl = uniqueFileName;
                    }
                    else
                    {
                        obj.ImageUrl = OldImageUrl;
                    }

                    _repo.DeleteTransactionByMemberId(vobj.MemberId);
                    if (vobj.Transactions.Any())
                    {
                        _repo.AddTransactionByMemberId(vobj.MemberId, vobj.Transactions);
                    }
                    _repo.UpdateMember(obj);

                    return Json(new { success = true, redirectUrl = Url.Action("Index") });
                }
                return Json(new { success = false, errors = new[] { "Member not found." } });
            }
            catch (Exception ex)
            {
                var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, errors = new[] { innerException } });
            }
        }
    }
}