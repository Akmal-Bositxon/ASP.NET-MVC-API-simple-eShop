using eShopDAL.Entities;
using eShopDAL.Repositories;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using WAD.CW2_WebApp._5529.Helpers;
using WAD.CW2_WebApp._5529.Models;
using PagedList;
using PagedList.Mvc;
using CsvHelper;
using System.Globalization;

namespace WAD.CW2_WebApp._5529.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index(string category,
                   string name,
                   SortCriteria? criteria,
                   SortOrder? order,
                   int? page
                   )
        {
            ViewBag.Order = order;
            ViewBag.Name  =name ;
            ViewBag.Criteria = criteria;
            ViewBag.Category = category;
            HttpCookie authCookie = Request.Cookies.Get("UserLoginData");

            if (authCookie != null && !string.IsNullOrEmpty( authCookie.Value))
            {
                var model = new ProductsViewModel
                {
                    Category = category,
                    Name = name,
                    Criteria = criteria ?? SortCriteria.Name,
                    Order = order ?? SortOrder.ASC
                };
                var products = new ProductRepository().GetAll();
                model.Categories = products.Select(p => p.Category).Distinct().Select(c => new SelectListItem { Value = c, Text = c }).ToList();
                ViewBag.ListItems = model.Categories;
                if (!string.IsNullOrEmpty(category))
                    products = products.Where(p => p.Category.Equals(category));

                if (!string.IsNullOrEmpty(name))
                    products = products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
                if (criteria == SortCriteria.Price)
                {
                    if (order == SortOrder.DESC)
                    {
                        products = products.OrderByDescending(p => p.Price);
                    }
                    else
                    {
                        products = products.OrderBy(p => p.Price);
                    }
                }
                else
                {
                    if (order == SortOrder.DESC)
                    {
                        products = products.OrderByDescending(p => p.Name);
                    }
                    else
                    {
                        products = products.OrderBy(p => p.Name);
                    }
                }
                model.Products = products.Select(MapToModel).ToList();

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(model.Products.ToPagedList(pageNumber, pageSize));

            }
        
                return RedirectToAction("Login", "Authentication");
            
        
        }
        private Product MapFromModel(ProductViewModel model)
        {
            return new Product
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Category = model.Category
            };
        }
        private ProductViewModel MapToModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category
            };
        }
        [HttpPost]
        public ActionResult ImportProducts(FormCollection formCollection)
        {
            var csvfile = Request.Files["csvToImport"];
            var file = Request.Files["fileToImport"];
            var products = new List<ProductViewModel>();
            if (file == null && csvfile == null)
            {
                ViewBag.Result = Test.FileMissing;
                return View();
            }
            using (var sr = new StreamReader(csvfile.InputStream))
            {
                var reader = new CsvReader(sr, CultureInfo.InvariantCulture);
                reader.Configuration.HeaderValidated = null;

                //CSVReader will now read the whole file into an enumerable
                IEnumerable<ProductViewModel> records = reader.GetRecords<ProductViewModel>();

                //First 5 records in CSV file will be printed to the Output Window
                foreach (ProductViewModel product in records)
                {
                    new ProductRepository().Create(MapFromModel(product));
                    products.Add(product);
                }
            }

            using (var reader = new StreamReader(file.InputStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var tokens = line.Split('|');
                    var product = new ProductViewModel();
                    product.Name = tokens[0];
                    product.Price = Convert.ToDecimal(tokens[1]);
                    product.Category = tokens[2];

                    new ProductRepository().Create(MapFromModel(product));
                    products.Add(product);

                }
            }

            ViewBag.Products = products;
            return View();
        }
    //Get: ImportProducts
        public ActionResult ImportProducts()
        {

       
                return View();

        }




        [HttpGet]
        public ActionResult Create()
        {

        

            List<SelectListItem> Items = new List<SelectListItem>();
         

      
          
       

            var model = new ProductViewModel();

    
         

            ViewBag.ListItems = Items;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ProductViewModel model)
        {
          

            if (ModelState.IsValid)
            {
                HttpCookie authCookie = Request.Cookies.Get("UserLoginData");
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userName = ticket.Name;
                LogHelper.Info(string.Format("User {0} created a product {1} ", userName, model.Name));
                new ProductRepository().Create(MapFromModel(model));
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Model Error");
                return View(model);
            }

        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
         
          
          
      

            if (ModelState.IsValid)
            {
                var product = new ProductRepository().GetById(id);
                HttpCookie authCookie = Request.Cookies.Get("UserLoginData");
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userName = ticket.Name;
                LogHelper.Info(string.Format("User {0} edited a product {1} with Id {2} ", userName, product.Name, id));
                return View(MapToModel(new ProductRepository().GetById(id)));

            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult Edit(ProductViewModel model)
        {
           
            if (ModelState.IsValid)
            {
               
                new ProductRepository().Edit(MapFromModel(model));
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Model Error");
                return View(model);
            }

        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            
            if (ModelState.IsValid)
            {
                
                return View(MapToModel(new ProductRepository().GetById(id)));
            }
            else
            {
                return RedirectToAction("Index");
            }



        }

        [HttpPost]
        public ActionResult Delete(ProductViewModel model)
        {
         
            if (ModelState.Values.ElementAt(0).Errors.Count == 0)
            {

            
                HttpCookie authCookie = Request.Cookies.Get("UserLoginData");
                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                var userName = ticket.Name;
                LogHelper.Info(string.Format("User {0} deleted a product {1} with Id {2} ", userName, model.Name, model.Id)); ;

                new ProductRepository().Delete(model.Id);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Model Error");
                return View(model);
            }

        }

        [HttpGet]
        public ActionResult Details(int id)
        {
        
            if (ModelState.IsValid)
            {
              
                return View(MapToModel(new ProductRepository().GetById(id)));
            }
            else
            {
                return RedirectToAction("Index");
            }


        }
        public ActionResult React()
        {
            HttpCookie authCookie = Request.Cookies.Get("UserLoginData");

            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                 return View();
            }
            return RedirectToAction("Login", "Authentication");
        }
    }
}