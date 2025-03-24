using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;

namespace Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>
    {
        public CartItemRepository() { }

        public async Task<List<CartItem>> GetAllAsync2()
        {
            return await _context.CartItems.Include(x => x.Cart).Include(x => x.Product).ToListAsync();
        }
    }
}
