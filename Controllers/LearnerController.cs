using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class LearnerController : Controller
    {
        private readonly SchoolContext db;

        public LearnerController(SchoolContext context)
        {
            db = context;
        }
        // GET: Learner/Create
        public IActionResult Create()
        {
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }

        // POST: Learner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                db.Learners.Add(learner);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View(learner);
        }
        // GET: Learner/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null || db.Learners == null)
            {
                return NotFound();
            }

            var learner = db.Learners.Find(id);
            if (learner == null)
            {
                return NotFound();
            }

            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        // POST: Learner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("LearnerID,FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (id != learner.LearnerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(learner);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerExists(learner.LearnerID))
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

            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        private bool LearnerExists(int id)
        {
            return (db.Learners?.Any(e => e.LearnerID == id)).GetValueOrDefault();
        }
        // 8.1.1 Hiển thị danh sách
        private int pageSize = 3;
        public IActionResult Index(int? mid)
        {
            var learners = (IQueryable<Learner>)db.Learners
                .Include(m => m.Major);

            if (mid != null)
            {
                learners = (IQueryable<Learner>)db.Learners
                    .Where(l => l.MajorID == mid)
                    .Include(m => m.Major);
            }

            // Tính số trang
            int pageNum = (int)Math.Ceiling(learners.Count() / (float)pageSize);

            // Trả số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;

            // Lấy dữ liệu trang đầu
            var result = learners.Take(pageSize).ToList();
            return View(result);
        }
        // Action LearnerFilter để phân trang kết hợp lọc và tìm kiếm dữ liệu (Mục 2)
        public IActionResult LearnerFilter(int? mid, string? keyword, int? pageIndex)
        {
            // Lấy toàn bộ learners trong dbset chuyển về IQueryable<Learner> để query
            var learners = (IQueryable<Learner>)db.Learners;

            // Lấy chỉ số trang, nếu chỉ số trang null thì gán ngầm định bằng 1
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            // Nếu có mid thì lọc learner theo mid (chuyên ngành)
            if (mid != null)
            {
                learners = learners.Where(l => l.MajorID == mid);
                // Gửi mid về view để ghi lại trên nav-phân trang
                ViewBag.mid = mid;
            }

            // Nếu có keyword thì tìm kiếm theo tên
            if (keyword != null)
            {
                learners = learners.Where(l => l.FirstMidName.ToLower()
                    .Contains(keyword.ToLower()));
                // Gửi keyword về view để ghi lại trên nav-phân trang
                ViewBag.keyword = keyword;
            }

            // Tính số trang
            int pageNum = (int)Math.Ceiling(learners.Count() / (float)pageSize);
            // Gửi số trang về view để hiển thị nav-trang
            ViewBag.pageNum = pageNum;

            // Chọn dữ liệu trong trang hiện tại
            var result = learners.Skip(pageSize * (page - 1))
                .Take(pageSize).Include(m => m.Major).ToList();

            return PartialView("LearnerTable", result);
        }
        public IActionResult LearnerByMajorID(int mid)
        {
            var learners = db.Learners
                .Where(l => l.MajorID == mid)
                .Include(m => m.Major).ToList();
            return PartialView("LearnerTable", learners);
        }
    }
}