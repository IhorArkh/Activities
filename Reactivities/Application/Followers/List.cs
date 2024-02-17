using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Persistence;
using Profile = Application.Profiles.Profile;

namespace Application.Followers;

public class List
{
    public class Query : IRequest<Result<List<Profile>>>
    {
        public string Predicate { get; set; }

        public string UserName { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<List<Profile>>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<Profile>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profiles = new List<Profile>();

            switch (request.Predicate)
            {
                case "followers":
                    profiles = await _context.UserFollowings
                        .Where(x => x.Target.UserName == request.UserName)
                        .Select(x => x.Observer)
                        .ProjectTo<Profile>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                    break;

                case "following":
                    profiles = await _context.UserFollowings
                        .Where(x => x.Observer.UserName == request.UserName)
                        .Select(x => x.Target)
                        .ProjectTo<Profile>(_mapper.ConfigurationProvider)
                        .ToListAsync();
                    break;
            }

            return Result<List<Profile>>.Success(profiles);
        }
    }
}