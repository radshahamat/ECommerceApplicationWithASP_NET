﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ECommerceApplication.Models
{
    public class LoginDTO
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}