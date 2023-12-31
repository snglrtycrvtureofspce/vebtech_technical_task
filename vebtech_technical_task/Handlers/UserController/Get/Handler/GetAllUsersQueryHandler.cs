﻿using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using vebtech_technical_task.Data;
using vebtech_technical_task.Handlers.UserController.Get.Query;
using vebtech_technical_task.Models;
using vebtech_technical_task.ViewModels;

namespace vebtech_technical_task.Handlers.UserController.Get.Handler;

/// <summary>
/// Handler for the GetAllUsersQuery method
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserViewModelSummary>>
{
    private readonly UsersDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor with params for GetAllUsersQueryHandler
    /// </summary>
    /// <param name="context"></param>
    /// <param name="mapper"></param>
    public GetAllUsersQueryHandler(UsersDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserViewModelSummary>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.Roles)
            .AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(request.Filter))
        {
            query = query.Where(u => u.Name.Contains(request.Filter) || u.Email.Contains(request.Filter) ||
                                     u.Roles.Any(r => r.Name.Contains(request.Filter)));
        }
        
        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var sortLambda = GetSortLambda<User>(request.SortBy);
            query = query.OrderBy(sortLambda);
        }
        
        query = query.Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
        
        return await query
            .Select(u => _mapper.Map<UserViewModelSummary>(u))
            .ToListAsync(cancellationToken);
    }
    
    private static Expression<Func<TEntity, object>> GetSortLambda<TEntity>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), parameter);
        return lambda;
    }
}