﻿using FluentValidation;
using vebtech_technical_task.Handlers.UserController.Post.Command;

namespace vebtech_technical_task.Handlers.UserController.Validator;

/// <summary>
/// Data validator for the user creation method
/// </summary>
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    /// <inheritdoc />
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(command => command.Age).GreaterThan(0)
            .WithMessage("Age must be a positive number.");
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}