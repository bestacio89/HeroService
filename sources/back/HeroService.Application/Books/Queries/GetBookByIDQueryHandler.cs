

// Queries/Books/Handlers/GetBookByIdQueryHandler.cs
using Franz.Common.Mediator;
using Franz.Common.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using Franz.Common.Mapping.Abstractions;
using HeroService.Domain.Entities;
using Franz.Common.Business.Domain;
using HeroService.Contracts.DTOs;
using HeroService.Contracts.Queries.Books;

namespace HeroService.Application.Books.Queries
{
    public sealed class GetBookByIdQueryHandler
    : IQueryHandler<GetBookByIdQuery, Result<BookDto>>
    {
        private readonly IReadRepository<Book> _bookRepository;
    private readonly IFranzMapper _mapper;

        public GetBookByIdQueryHandler(IReadRepository<Book> bookRepository, IFranzMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<Result<BookDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetEntity(request.BookId);

            if (book is null)
                return "Book not found".ToFailure<BookDto>();

            return _mapper.Map<Book,BookDto>(book).ToResult();
        }
    }

}
