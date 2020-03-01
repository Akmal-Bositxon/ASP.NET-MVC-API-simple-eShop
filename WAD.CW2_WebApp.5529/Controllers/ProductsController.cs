using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using eShopDAL.Entities;
using eShopDAL.Repositories;
using WAD.CW2_WebApp._5529.Helpers;
using WAD.CW2_WebApp._5529.Models;

namespace WAD.CW2_WebApp._5529.Controllers
{
    public class ProductsController : ApiController
    {
        private eShopDBEntities db = new eShopDBEntities();
        
        // GET: api/Products
        public IQueryable<Product> GetProducts(string name, string category, string criteria, string order)
        {
            var products = new ProductRepository().GetAll();
            
            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category.Equals(category));

            if (!string.IsNullOrEmpty(name))
                products = products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            if (criteria == "Price")
            {
                if (order == "DESC")
                {
                    products = products.OrderByDescending(p => p.Price);
                }
                else
                {
                    products = products.OrderBy(p => p.Price);
                }
            }
            else if(criteria=="Name")
            {
                if (order == "DESC")
                {
                    products = products.OrderByDescending(p => p.Name);
                }
                else
                {
                    products = products.OrderBy(p => p.Name);
                }
            }
            else
            {
                if (order == "DESC")
                {
                    products = products.OrderByDescending(p => p.Id);
                }
                else
                {
                    products = products.OrderBy(p => p.Id);
                }
            }

            return products;
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.Id)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
              
                
                LogHelper.Info(string.Format("User edited a product {0} with Id {1} ", product.Name, product.Id));
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LogHelper.Info(string.Format("User created a product {0} ", product.Name));
            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            LogHelper.Info(string.Format("User deleted a product {0} with Id {1} ", product.Name, product.Id));

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}