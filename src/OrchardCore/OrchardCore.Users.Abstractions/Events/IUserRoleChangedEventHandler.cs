using System;
using System.Collections.Generic;
using System.Text;
using OrchardCore.Users.Models;

namespace OrchardCore.Users.Events;

public interface IUserRoleChangedEventHandler
{
    Task RolesAddedAsync(IUser user, IEnumerable<string> rolesAdded);
    Task RolesRemovedAsync(IUser user, IEnumerable<string> rolesRemoved);
}
