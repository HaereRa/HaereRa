using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using HaereRa.API.Models;

namespace HaereRa.API.Services
{
    public interface INotificationService
    {
		Task<IEnumerable<NotificationMessage>> GetNotificationMessagesAsync(IEnumerable<int> affectedUserIds, NotificationType notificationType, CancellationToken cancellationToken = default);
		Task SendNotificationsAsync(NotificationMessage notificationMessage, CancellationToken cancellationToken = default);
		Task SendNotificationsAsync(IEnumerable<NotificationMessage> notificationMessages, CancellationToken cancellationToken = default);
    }
}
