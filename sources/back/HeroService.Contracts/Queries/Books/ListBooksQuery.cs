// Application/Books/Queries/ListBooksQuery.cs
using HeroService.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace HeroService.Contracts.Queries.Books;

public sealed record ListBooksQuery
    : IQuery<Result<IReadOnlyCollection<BookDto>>>;
