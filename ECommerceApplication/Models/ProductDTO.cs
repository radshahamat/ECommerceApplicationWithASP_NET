using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceApplication.Models
{
    public class ProductDTO
    {
        [Required]
        public string name { get; set; }
        [Required]
        public int? Quantity { get; set; }
        [Required]
        public int? price { get; set; }
        [Required]
        public int? categoryId { get; set; }
        public int? adminId { get; set; }
    }
}