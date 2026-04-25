// Commands/Books/Handlers/UpdateBookCommandHandler.cs
using Franz.Common.EntityFramework.Repositories;
using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using HeroService.Contracts.Commands.Books;
using HeroService.Domain.Entities;
using HeroService.Domain.ValueObjects;
using HeroService.Persistence;

namespace HeroService.Application.Books.Commands
{
    public sealed class UpdateBookCommandHandler : ICommandHandler<UpdateBookCommand, Result>
    {
        private readonly EntityRepository<ApplicationDbContext, Book> _bookRepository;

        public UpdateBookCommandHandler(EntityRepository<ApplicationDbContext, Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Result> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);
            if (book is null)
                return Error.NotFound("Book", request.Id).ToFailure<Result>();

            book.UpdateDetails(
                new Title(request.Title),
                new Author(request.Author),
                new ISBN(request.Isbn),
                request.Copies
            );

            await _bookRepository.UpdateAsync(book, cancellationToken);
          

            return Result.Success();
        }
    }
}
