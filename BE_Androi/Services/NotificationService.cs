using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models;
using Repositories;

namespace Services
{
    public interface INotificationService
    {
        Task SendCartUpdateNotification(int userId, int itemCount);
    }

    public class NotificationService : INotificationService
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task SendCartUpdateNotification(int userId, int itemCount)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = $"Your cart now has {itemCount} item(s).",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _notificationRepository.CreateAsync(notification);
        }
    }
}
