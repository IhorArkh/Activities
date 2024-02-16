using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
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
                o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(p => p.IsMain).Url));

        CreateMap<AppUser, Profiles.Profile>()
            .ForMember(x => x.Image, o =>
                o.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain).Url));

        CreateMap<Comment, CommentDto>()
            .ForMember(x => x.DisplayName, o =>
                o.MapFrom(s => s.Author.DisplayName))
            .ForMember(x => x.UserName, o =>
                o.MapFrom(s => s.Author.UserName))
            .ForMember(x => x.Image, o =>
                o.MapFrom(s => s.Author.Photos.FirstOrDefault(p => p.IsMain).Url));
    }
}