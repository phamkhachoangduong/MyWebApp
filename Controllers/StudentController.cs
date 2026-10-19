using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApp.Controllers
{
    public class StudentController : Controller
    {
        private readonly IWebHostEnvironment _env;

        // Khởi tạo sẵn danh sách sinh viên ban đầu
        protected static List<Student> listStudents = new List<Student>()
        {
            new Student() { Id = 101, Name = "Hải Nam", Branch = Branch.IT, Gender = Gender.Male, IsRegular = true, Address = "A1-2018", Email = "nam@g.com" },
            new Student() { Id = 102, Name = "Minh Tú", Branch = Branch.BE, Gender = Gender.Female, IsRegular = true, Address = "A1-2019", Email = "tu@g.com" },
            new Student() { Id = 103, Name = "Hoàng Phong", Branch = Branch.CE, Gender = Gender.Male, IsRegular = false, Address = "A1-2020", Email = "phong@g.com" },
            new Student() { Id = 104, Name = "Xuân Mai", Branch = Branch.EE, Gender = Gender.Female, IsRegular = false, Address = "A1-2021", Email = "mai@g.com" }
        };

        // Constructor
        public StudentController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // 1. Hiển thị danh sách sinh viên
        [HttpGet]
        [Route("Admin/Student/List", Name = "student_list")]
        public IActionResult Index()
        {
            return View(listStudents);
        }

        // 2. Hiển thị form thêm sinh viên
        [HttpGet]
        [Route("Admin/Student/Add", Name = "student_add")]
        public IActionResult Create()
        {
            NapDuLieuForm();
            return View();
        }

        // 3. Nhận dữ liệu và upload ảnh đại diện
        [HttpPost]
        [Route("Admin/Student/Add")]
        public async Task<IActionResult> Create(Student student)
        {
            // Kiểm tra ảnh đại diện nếu người dùng đã chọn ảnh
            if (student.AvatarFile != null && student.AvatarFile.Length > 0)
            {
                // Kiểm tra dung lượng tối đa 2MB
                if (student.AvatarFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Dung lượng ảnh không được vượt quá 2MB.");
                }

                // Chỉ chấp nhận JPG, JPEG, PNG, GIF
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string extension = Path.GetExtension(student.AvatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .gif.");
                }
            }
            else
            {
                // Nếu không upload ảnh, bỏ qua xác thực trường AvatarFile
                ModelState.Remove("AvatarFile");
            }

            // Nếu dữ liệu không hợp lệ, quay lại form
            if (!ModelState.IsValid)
            {
                NapDuLieuForm();
                return View(student);
            }

            // Lưu ảnh nếu người dùng đã chọn ảnh
            if (student.AvatarFile != null && student.AvatarFile.Length > 0)
            {
                string extension = Path.GetExtension(student.AvatarFile.FileName).ToLowerInvariant();
                string fileName = Guid.NewGuid().ToString("N") + extension;

                string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                string uploadFolder = Path.Combine(webRoot, "images", "avatars");

                Directory.CreateDirectory(uploadFolder);

                string filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await student.AvatarFile.CopyToAsync(stream);
                }

                student.AvatarUrl = "/images/avatars/" + fileName;
            }

            // Tạo mã sinh viên mới
            student.Id = listStudents.Count > 0 ? listStudents.Max(s => s.Id) + 1 : 101;

            // Thêm sinh viên vào danh sách
            listStudents.Add(student);

            // Chuyển về trang danh sách theo đúng Route name
            return RedirectToRoute("student_list");
        }

        // 4. Nạp dữ liệu cho form Create
        private void NapDuLieuForm()
        {
            ViewBag.AllGenders = Enum.GetValues(typeof(Gender))
                .Cast<Gender>()
                .ToList();

            ViewBag.AllBranches = new List<SelectListItem>()
            {
                new SelectListItem { Text = "IT", Value = "IT" },
                new SelectListItem { Text = "BE", Value = "BE" },
                new SelectListItem { Text = "CE", Value = "CE" },
                new SelectListItem { Text = "EE", Value = "EE" }
            };
        }
    }
}