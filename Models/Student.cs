using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Bắt buộc để sử dụng IFormFile

namespace MyWebApp.Models
{
    public class Student
    {
        public int Id { get; set; } // Mã sinh viên
        public string? Name { get; set; } // Họ tên
        public string? AvatarUrl { get; set; }

        // Thuộc tính nhận file upload từ form (không ánh xạ vào CSDL nếu dùng EF Core)
        [NotMapped]
        public IFormFile? AvatarFile { get; set; }

        public string? Email { get; set; } // Email
        public string? Password { get; set; } // Mật khẩu
        public Branch? Branch { get; set; } // Ngành học
        public Gender? Gender { get; set; } // Giới tính
        public bool IsRegular { get; set; } // Hệ: true - chính quy, false - phi chính quy
        public string? Address { get; set; } // Địa chỉ
        public DateTime DateOfBirth { get; set; } // Ngày sinh (đã sửa chính tả)
    }
}