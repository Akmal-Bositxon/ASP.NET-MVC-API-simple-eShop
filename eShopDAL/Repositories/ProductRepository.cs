using eShopDAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopDAL.Repositories
{
  public  class ProductRepository
    {
        private eShopDBEntities db;

        public ProductRepository()
        {
            db = new eShopDBEntities();
        }

        public IQueryable<Product> GetAll()
        {
            return db.Products;
        }

        public void Create(Product product)
        {
            db.Products.Add(product);
            db.SaveChanges();

        }
        public void Edit(Product EditProduct)
        {
            db.Entry(EditProduct).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            db.Products.Remove(GetById(id));
            db.SaveChanges();
        }
        public Product GetById(int Id)
        {
            return db.Products.Find(Id);
        }

    }
}
