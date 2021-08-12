using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository( ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Products.FirstOrDefault(s => s.Id == product.Id);
            if (objFromDb != null)
            {
                if(product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }
                objFromDb.Title = product.Title;
                objFromDb.Author = product.Author;
                objFromDb.Description = product.Description;
                objFromDb.price100 = product.price100;
                objFromDb.Price50 = product.Price50;
                objFromDb.Price = product.Price;
                objFromDb.ListPrice = product.ListPrice;
                objFromDb.ISBN = product.ISBN;
                

                
            }
        }
    }
}
