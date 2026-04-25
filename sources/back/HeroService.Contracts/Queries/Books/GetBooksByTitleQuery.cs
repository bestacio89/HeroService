// Queries/Books/GetBookByTitleQuery.cs
using HeroService.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace HeroService.Contracts.Queries.Books
{
    public sealed record GetBookByTitleQuery(string Title) : IQuery<Result<IEnumerable<BookDto>>>;
}
