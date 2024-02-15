using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles;

public class Details
{
    public class Query : IRequest<Result<Profile>>
    {
        public string UserName { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<Profile>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .ProjectTo<Profile>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Username == request.UserName);

            if (user is null)
                return null;
            // not necessary, this case already handled in HandleResult() method. But it is for better understanding.

            return Result<Profile>.Success(user);
        }
    }
}