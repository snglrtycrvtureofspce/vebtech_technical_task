﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using vebtech_technical_task.Data;
using vebtech_technical_task.Handlers.UserController.Post.Command;

namespace vebtech_technical_task.Handlers.UserController.Post.Handler;

/// <summary>
/// Handler for the AddUserRole method
/// </summary>
public class AddUserRoleHandler : IRequestHandler<AddUserRoleCommand, Unit>
{
    private readonly UsersDbContext _context;

    /// <summary>
    /// Constructor with params for AddUserRoleHandler
    /// </summary>
    /// <param name="context"></param>
    public AddUserRoleHandler(UsersDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(AddUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.Include(user => user.Roles)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId,
            cancellationToken: cancellationToken);

        if (user == null || role == null)
        {
            throw new Exception("User or Role not found");
        }

        user.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}