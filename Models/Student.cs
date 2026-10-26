using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace MyWebApp.Models
{
    public class Student
    {
        public int Id { get; set; } // Mã sinh viên

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Họ tên tối thiểu 4 ký tự, tối đa 100 ký tự")]
        public string? Name { get; set; } // Họ tên

        [Required(ErrorMessage = "Email bắt buộc phải được nhập")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Địa chỉ email phải có đuôi gmail.com")]
        public string? Email { get; set; } // Email

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Mật khẩu từ 8 ký tự trở lên, gồm chữ hoa, chữ thường, chữ số và ký tự đặc biệt")]
        public string? Password { get; set; } // Mật khẩu

        [Required(ErrorMessage = "Vui lòng chọn ngành học")]
        public Branch? Branch { get; set; } // Ngành học

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public Gender? Gender { get; set; } // Giới tính

        public bool IsRegular { get; set; } // Hệ: true - chính quy, false - phi chính quy

        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? Address { get; set; } // Địa chỉ

        [Range(typeof(DateTime), "1/1/1963", "12/31/2005", ErrorMessage = "Ngày sinh phải nằm trong khoảng từ 1/1/1963 đến 31/12/2005")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Ngày sinh bắt buộc phải nhập")]
        public DateTime DateOfBirth { get; set; } // Ngày sinh (lưu ý đồng bộ tên này với View)

        // Bổ sung trường Điểm theo yêu cầu Mục 4
        [Required(ErrorMessage = "Điểm bắt buộc phải nhập")]
        [Range(0.0, 10.0, ErrorMessage = "Điểm phải là số thực và có giá trị từ 0.0 đến 10.0")]
        public double? Score { get; set; }

        // Ảnh đại diện
        public string? AvatarUrl { get; set; }

        [NotMapped]
        public IFormFile? AvatarFile { get; set; }
    }
}