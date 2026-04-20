using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using OrchardCore.Users.Events;
using OrchardCore.Users.Models;

namespace OrchardCore.Users.Handlers;

// Implements the logic that should occur when a user's roles change
public class UserRoleChangedEventHandler : IUserRoleChangedEventHandler
{
    private readonly UserManager<IUser> _userManager;

    public UserRoleChangedEventHandler(UserManager<IUser> userManager)
    {
        _userManager = userManager;
    }

    // Called when roles are added to a user.
    public async Task RolesAddedAsync(IUser user, IEnumerable<string> rolesAdded)
    {
        // Invalidate all existing sessions by updating the security stamp.
        await _userManager.UpdateSecurityStampAsync(user);
        if (user is User u)
        {
            u.RoleChanged = true;
            // Persist the updated user record.
            await _userManager.UpdateAsync(user);
        }
    }

    // Called when roles are removed from a user.
    public async Task RolesRemovedAsync(IUser user, IEnumerable<string> rolesRemoved)
    {
        // Invalidate all existing sessions by updating the security stamp.
        await _userManager.UpdateSecurityStampAsync(user);
        if (user is User u)
        {
            u.RoleChanged = true;
            // Persist the updated user record.
            await _userManager.UpdateAsync(user);
        }
    }

}
