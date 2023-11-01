using ECommerceApplication.Auth;
using ECommerceApplication.DB;
using ECommerceApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceApplication.Controllers
{
    public class AdminController : Controller
    {
        private ECommerceEntities1 db = new ECommerceEntities1();
        public ActionResult Index()
        {
            var adminList = db.Admins;
            return View(adminList);
        }
        [AdminAuth]
        public ActionResult Product()
        {
            var ProductList = db.Products.ToList();
            return View(ProductList);
        }
        [AdminAuth]
        public ActionResult CreateProduct()
        {
            ViewData["categoryId"] = getCategoryIdList();
            return View();
        }
        [AdminAuth]
        [HttpPost]
        public ActionResult CreateProduct(ProductDTO Obj)
        {
            Obj.adminId = (int)Session["Id"];
            if (ModelState.IsValid)
            {
                var newObj = new Product()
                {
                    name = Obj.name,
                    Quantity = (int)Obj.Quantity,
                    price = (int)Obj.price,
                    categoryId = (int)Obj.categoryId,
                    adminId = (int)Obj.adminId
                };
                db.Products.Add(newObj);
                db.SaveChanges();
                TempData["Message"] = "Product is created successfully";
                return RedirectToAction("Product");
            }
            var li = getCategoryIdList();
            ViewData["categoryId"] = li;
            return View(Obj);
        }
        private List<SelectListItem> getCategoryIdList()
        {
            var SelectList = new List<SelectListItem>
            {
                new SelectListItem()
                {
                    Selected = true,
                    Disabled = true,
                    Text = "Select a Product",
                    Value = ""
                }
            };
            var CategoryIdList = db.Categories.Select(p => new SelectListItem()
            {
                Value = p.id.ToString(),
                Text = p.name
            });
            SelectList.AddRange(CategoryIdList);
            return SelectList;
        }
        [AdminAuth]
        public ActionResult Category()
        {
            var CategoryList = db.Categories.ToList();
            return View(CategoryList);
        }
        [AdminAuth]
        public ActionResult CreateCategory()
        {
            return View();
        }
        [AdminAuth]
        [HttpPost]
        public ActionResult CreateCategory(CategoryDTO Obj)
        {
            Obj.adminId = (int) Session["Id"];
            if (ModelState.IsValid)
            {
                var newObj = new Category()
                {
                    name = Obj.name,
                    adminId = Obj.adminId
                };
                db.Categories.Add(newObj);
                db.SaveChanges();
                TempData["Message"] = "Category is created successfylly";
                return RedirectToAction("Category");
            }
            return View(Obj);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginDTO Obj)
        {
            var adminObj = db.Admins
                .Where(a =>
                    a.username.Equals(Obj.username) &&
                    a.password.Equals(Obj.password)
                ).SingleOrDefault();
            if (adminObj == null)
            {
                TempData["Message"] = "Wrong credential";
                return View(Obj);
            }
            Session["usertype"] = "Admin";
            Session["Id"] = adminObj.id;
            Session["username"] = adminObj.username;
            TempData["Message"] = "Admin Login is successful";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            TempData["Message"] = "Successfully Logout";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registration(LoginDTO Obj)
        {
            var ExAdmin = db.Admins.Where(a => a.username.Equals(Obj.username));
            var ExCustomer = db.Customers.Where(c => c.username.Equals(Obj.username));
            if(ExAdmin.Count() > 0 || ExCustomer.Count() > 0)
            {
                TempData["Message"] = "User is exists already";
                return View(Obj);
            }
            var adminObj = new Admin()
            {
                username = Obj.username,
                password = Obj.password,
            };
            db.Admins.Add(adminObj);
            db.SaveChanges();

            TempData["Message"] = "Registraion is successful";
            return RedirectToAction("Login");
        }
    }
}