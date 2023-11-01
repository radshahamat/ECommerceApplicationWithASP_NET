using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceApplication.Models
{
    public class CategoryDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public int adminId { get; set; }
    }
}