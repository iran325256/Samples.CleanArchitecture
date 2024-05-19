﻿using Application.Common.Models;
using Domain.ToDoListDomain.Entities;
using Domain.ToDoListDomain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.TodoItems.Commands.CreateTodoItem
{
    public record CreateTodoItemCommand : IRequest<BaseResult<long>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public DateTime TimeTodo { get; set; }
    }

    public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, BaseResult<long>>
    {
        private readonly IApplicationDbContext _context;

        public CreateTodoItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaseResult<long>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new TodoItem(
                new TitleValueObject(request.Title),
                new DescriptionValueObject(request.Description),
                new ColorValueObject(request.Color),
                new TimeTodoValueObject(request.TimeTodo)
                );
            _context.TodoItems.Add(entity);

            await _context.PushNotifications();

            await _context.SaveChangesAsync(cancellationToken);

            return new BaseResult<long>().Ok(entity.Id);
        }
    }
}
