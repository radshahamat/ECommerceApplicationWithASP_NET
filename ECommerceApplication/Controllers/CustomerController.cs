using ECommerceApplication.Auth;
using ECommerceApplication.DB;
using ECommerceApplication.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceApplication.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        ECommerceEntities1 db = new ECommerceEntities1();
        public ActionResult Index()
        {
            var CutomerList = db.Customers;
            return View(CutomerList);
        }
        [CustomerAuth]
        public ActionResult Product()
        {
            var productList = db.Products;
            return View(productList);
        }
        [CustomerAuth]
        [HttpGet]
        public ActionResult Cart()
        {
            var cartList = new List<cartDTO>();
            if (Session["cart"] != null)
            {
                var sessionCart = (List<cartDTO>)Session["cart"];
                cartList.AddRange(sessionCart);
            }
            var orderList = convert(cartList);
            return View(orderList);
        }
        [CustomerAuth]
        [HttpPost]
        public ActionResult Cart(cartDTO Obj)
        {
            var cartList = new List<cartDTO>() { Obj };
            if (Session["cart"] != null)
            {
                var sessionCart = (List<cartDTO>)Session["cart"];
                if (sessionCart.Where(s => s.productId.Equals(Obj.productId)).Count() == 0)
                {
                    cartList.AddRange(sessionCart);
                }
                else
                {
                    sessionCart.Where(s => s.productId.Equals(Obj.productId)).ForEach(s => s.quantity += Obj.quantity);
                    cartList = sessionCart;
                }
            }
            Session["cart"] = cartList;
            TempData["Message"] = "[" +cartList.Count()+ "] New Item is added to cart";
            return RedirectToAction("Product");
        }
        [CustomerAuth]
        [HttpGet]
        public ActionResult ClearCart()
        {
            Session["cart"] = null; 
            TempData["Message"] = "Cart is cleared";
            return RedirectToAction("Cart");
        }
        [CustomerAuth]
        [HttpGet]
        public ActionResult Order()
        {
            int customerId = (int) Session["Id"];
            var orderList = db.Orders.Where(o => o.customerId.Equals(customerId));
            return View(orderList);
        }
        [CustomerAuth]
        [HttpGet]
        public ActionResult OrderDetails(int id)
        {
            var OrderObj = db.Orders.Find(id).productOrders;
            return View(OrderObj);
        }
        [CustomerAuth]
        [HttpGet]
        public ActionResult PlaceOrder()
        {
            if(Session["cart"] == null)
            {
                TempData["Message"] = "Order can not be places as cart is empty";
                return RedirectToAction("Cart");
            }
            var cartList = new List<cartDTO>();
            var sessionCart = (List<cartDTO>)Session["cart"];
            cartList.AddRange(sessionCart);

            bool isAllProductAvailable = true;
            string Message = "";

            foreach(cartDTO cart in cartList)
            {
                var product = db.Products.Find(cart.productId);
                if(product.Quantity < cart.quantity)
                {
                    isAllProductAvailable = false;
                    Message += cart.quantity.ToString() + " " + product.name + " is not availabe\n";
                }
                else
                {
                    product.Quantity -= cart.quantity;
                }
            }
            if(isAllProductAvailable == false)
            {
                TempData["Message"] = Message;
                return RedirectToAction("Cart");
            }
            var OrderObj = new Order()
            {
                customerId = (int) Session["Id"],
                orderDate = DateTime.Now.Date
            };
            db.Orders.Add(OrderObj);

            var orderList = convert(cartList, OrderObj.id);
            db.productOrders.AddRange(orderList);
            db.SaveChanges();

            Session["cart"] = null;
            TempData["Message"] = "Order is placed successfully";
            return RedirectToAction("Order");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginDTO Obj)
        {
            var customerObj = db.Customers
                .Where(a =>
                    a.username.Equals(Obj.username) &&
                    a.password.Equals(Obj.password)
                ).SingleOrDefault();
            if (customerObj == null)
            {
                TempData["Message"] = "Wrong credential";
                return View(Obj);
            }
            Session["usertype"] = "Customer";
            Session["Id"] = customerObj.id;
            Session["username"] = customerObj.username;
            TempData["Message"] = "Customer login is successful";
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
        public ActionResult Registraion()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Registraion(LoginDTO Obj)
        {
            var ExAdmin = db.Admins.Where(a => a.username.Equals(Obj.username));
            var ExCustomer = db.Customers.Where(c => c.username.Equals(Obj.username));
            if (ExAdmin.Count() > 0 || ExCustomer.Count() > 0)
            {
                TempData["Message"] = "User is exists already";
                return View(Obj);
            }
            var customerObj = new Customer()
            {
                username = Obj.username,
                password = Obj.password,
            };
            db.Customers.Add(customerObj); 
            db.SaveChanges();
            
            TempData["Message"] = "Customer Registraion is successful";
            return RedirectToAction("Login");
        }
        private productOrder convert(cartDTO Obj, int orderId = 0)
        {
            return new productOrder()
            {
                productId = Obj.productId,
                orderId = orderId,
                quantity = Obj.quantity,
                price = db.Products.Find(Obj.productId).price,
                Product = db.Products.Find(Obj.productId)
            };
        }
        private List<productOrder> convert(List<cartDTO> cartList, int orderId = 0)
        {
            var poList = new List<productOrder>();
            //poList.AddRange(convert(cartList, orderId));
            foreach(var cart in cartList)
            {
                poList.Add(convert(cart, orderId));
            }
            return poList;
        }
    }
}