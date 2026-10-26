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

        // Khởi tạo sẵn danh sách sinh viên ban đầu (bổ sung DateOfBirth và Score để khớp Model)
        protected static List<Student> listStudents = new List<Student>()
        {
            new Student()
            {
                Id = 101,
                Name = "Hải Nam",
                Branch = Branch.IT,
                Gender = Gender.Male,
                IsRegular = true,
                Address = "A1-2018",
                Email = "nam@gmail.com",
                DateOfBirth = new DateTime(2001, 1, 1),
                Score = 8.5
            },
            new Student()
            {
                Id = 102,
                Name = "Minh Tú",
                Branch = Branch.BE,
                Gender = Gender.Female,
                IsRegular = true,
                Address = "A1-2019",
                Email = "tu@gmail.com",
                DateOfBirth = new DateTime(2002, 5, 10),
                Score = 7.0
            },
            new Student()
            {
                Id = 103,
                Name = "Hoàng Phong",
                Branch = Branch.CE,
                Gender = Gender.Male,
                IsRegular = false,
                Address = "A1-2020",
                Email = "phong@gmail.com",
                DateOfBirth = new DateTime(2001, 11, 20),
                Score = 6.5
            },
            new Student()
            {
                Id = 104,
                Name = "Xuân Mai",
                Branch = Branch.EE,
                Gender = Gender.Female,
                IsRegular = false,
                Address = "A1-2021",
                Email = "mai@gmail.com",
                DateOfBirth = new DateTime(2003, 3, 15),
                Score = 9.0
            }
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

        // 3. Nhận dữ liệu và upload ảnh đại diện (Chuẩn Mục 3 bài Lab)
        [HttpPost]
        [Route("Admin/Student/Add")]
        public async Task<IActionResult> Create(Student s)
        {
            // Kiểm tra ảnh đại diện nếu người dùng có chọn file
            if (s.AvatarFile != null && s.AvatarFile.Length > 0)
            {
                // Kiểm tra dung lượng tối đa 2MB
                if (s.AvatarFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "Dung lượng ảnh không được vượt quá 2MB.");
                }

                // Chỉ chấp nhận JPG, JPEG, PNG, GIF
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string extension = Path.GetExtension(s.AvatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("AvatarFile", "Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .gif.");
                }
            }
            else
            {
                // Không bắt buộc ảnh -> xóa lỗi xác thực nếu không upload
                ModelState.Remove("AvatarFile");
            }

            // Nếu toàn bộ dữ liệu hợp lệ (Mục 3)
            if (ModelState.IsValid)
            {
                // Lưu file ảnh nếu có upload
                if (s.AvatarFile != null && s.AvatarFile.Length > 0)
                {
                    string extension = Path.GetExtension(s.AvatarFile.FileName).ToLowerInvariant();
                    string fileName = Guid.NewGuid().ToString("N") + extension;

                    string webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                    string uploadFolder = Path.Combine(webRoot, "images", "avatars");

                    Directory.CreateDirectory(uploadFolder);

                    string filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await s.AvatarFile.CopyToAsync(stream);
                    }

                    s.AvatarUrl = "/images/avatars/" + fileName;
                }

                // Tự động tăng Id theo đúng cú pháp bài giảng: listStudents.Last<Student>().Id + 1
                s.Id = listStudents.Count > 0 ? listStudents.Last<Student>().Id + 1 : 101;
                listStudents.Add(s);

                // Chuyển hướng về trang danh sách sinh viên
                return RedirectToRoute("student_list");
            }

            // Nếu dữ liệu không hợp lệ: nạp lại ViewBag và giữ lại form để hiển thị lỗi
            NapDuLieuForm();
            return View(s);
        }

        // 4. Nạp dữ liệu cho form Create (Chuẩn Mục 3)
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