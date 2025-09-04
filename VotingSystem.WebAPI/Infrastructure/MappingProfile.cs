using AutoMapper;
using VotingSystem.DataAccess.Models;
using VotingSystem.Shared.Models;
using VotingSystem.Shared.SignalR.Models;

namespace VotingSystem.WebAPI.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TopicRequestDto, Topic>()
                .ForMember(dest => dest.Choices,
                           opt => opt.MapFrom(src => src.Choices.Select(c => new Choice { Value = c }).ToList()))
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Votes, opt => opt.Ignore())
                .ForMember(dest => dest.VoterRecords, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<Topic, TopicResponseDto>()
                .ForMember(dest => dest.Choices,
                           opt => opt.MapFrom(src => src.Choices.Select(c => c.Value).ToList()))
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => src.Votes))
                .ForMember(dest => dest.Voters, opt => opt.MapFrom(src => src.VoterRecords.Select(r => r.User)));


            CreateMap<UserRequestDto, User>(MemberList.Source)
                .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<User, UserResponseDto>();

            CreateMap<Vote, VoteResponseDto>()
                .ForMember(dest => dest.SelectedChoices,
                    opt => opt.MapFrom(src => src.SelectedChoices.Select(c => c.Value).ToList()));

            CreateMap<Topic, TopicNotificationDto>()
                .ForMember(dest => dest.Choices, opt =>
                    opt.MapFrom(src => src.Choices
                        .Select(c => new ChoiceNotificationDto
                        {
                            Value = c.Value,
                            VoteCount = src.Votes.Count(v => v.SelectedChoices.Contains(c))
                        }).ToList()))
                .ForMember(dest => dest.TotalVotes, opt =>
                    opt.MapFrom(src => src.Votes.Count()));
        }
    }
}
