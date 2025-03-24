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
    public class NotificationRepository : GenericRepository<Notification>
    {
        public async Task<Notification> GetNotificationByIdAsync(int id)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);
        }
    }
}
