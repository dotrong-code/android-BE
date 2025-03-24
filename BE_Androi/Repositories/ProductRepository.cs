using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;

namespace Repositories
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository() { }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category) // Bao gồm thông tin Category
                .ToListAsync();
        }
        public async Task<List<Product>> GetAll()
        {
            var transactionList = await _context.Products.Include(t => t.Category).ToListAsync();
            return transactionList;
        }
    }
}
