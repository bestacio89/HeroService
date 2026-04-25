using HeroService.Domain.Entities;
using HeroService.Domain.ValueObjects;
using Franz.Common.Mediator.Handlers;
using Franz.Common.EntityFramework.Repositories;
using HeroService.Persistence;
using HeroService.Contracts.Commands.Books;

namespace HeroService.Application.Books.Commands;

public sealed class AddBookCommandHandler : ICommandHandler<AddBookCommand, int>
{
    private readonly EntityRepository<ApplicationDbContext, Book> _bookRepository;

    public AddBookCommandHandler(EntityRepository<ApplicationDbContext, Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<int> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book(
            new ISBN(request.Isbn),
            new Title(request.Title),
            new Author(request.Author),
            request.Datepublished,
            request.Copies

        );

        await _bookRepository.AddAsync(book, cancellationToken);
        return book.Id;
    }
}
