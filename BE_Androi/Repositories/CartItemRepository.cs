using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Base;
using Repositories.Models;

namespace Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>
    {
        public CartItemRepository() { }
    }
}
