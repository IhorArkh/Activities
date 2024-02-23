using Application.Activities;
using Application.Comments;
using Application.Profiles;
using Domain;
using Profile = AutoMapper.Profile;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        string currentUsername = null;

        CreateMap<Activity, Activity>();

        CreateMap<Activity, ActivityDto>()
            .ForMember(x =>
                x.HostUsername, o => o.MapFrom(x =>
                x.Attendees.FirstOrDefault(s => s.IsHost).AppUser.UserName));

        CreateMap<ActivityAttendee, AttendeeDto>()
            .ForMember(x => x.DisplayName, o =>
                o.MapFrom(s => s.AppUser.DisplayName))
            .ForMember(x => x.Username, o =>
                o.MapFrom(s => s.AppUser.UserName))
            .ForMember(x => x.Bio, o =>
                o.MapFrom(s => s.AppUser.Bio))
            .ForMember(x => x.Image, o =>
                o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(x => x.FollowersCount, o =>
                o.MapFrom(s => s.AppUser.Followers.Count()))
            .ForMember(x => x.FollowingCount, o =>
                o.MapFrom(s => s.AppUser.Followings.Count()))
            .ForMember(x => x.IsFollowing, o =>
                o.MapFrom(s => s.AppUser.Followers
                    .Any(u => u.Observer.UserName == currentUsername)));

        CreateMap<AppUser, Profiles.Profile>()
            .ForMember(x => x.Image, o =>
                o.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(x => x.FollowersCount, o =>
                o.MapFrom(s => s.Followers.Count()))
            .ForMember(x => x.FollowingCount, o =>
                o.MapFrom(s => s.Followings.Count()))
            .ForMember(x => x.IsFollowing, o =>
                o.MapFrom(s => s.Followers.Any(u => u.Observer.UserName == currentUsername)));

        CreateMap<Comment, CommentDto>()
            .ForMember(x => x.DisplayName, o =>
                o.MapFrom(s => s.Author.DisplayName))
            .ForMember(x => x.UserName, o =>
                o.MapFrom(s => s.Author.UserName))
            .ForMember(x => x.Image, o =>
                o.MapFrom(s => s.Author.Photos.FirstOrDefault(p => p.IsMain).Url));

        CreateMap<ActivityAttendee, UserActivityDto>()
            .ForMember(x => x.Id, o =>
                o.MapFrom(a => a.ActivityId))
            .ForMember(x => x.Title, o =>
                o.MapFrom(a => a.Activity.Title))
            .ForMember(x => x.Category, o =>
                o.MapFrom(a => a.Activity.Category))
            .ForMember(x => x.Date, o =>
                o.MapFrom(a => a.Activity.Date))
            .ForMember(x => x.HostUsername, o =>
                o.MapFrom(a => a.Activity.Attendees
                    .FirstOrDefault(b => b.IsHost).AppUser.UserName));
    }
}