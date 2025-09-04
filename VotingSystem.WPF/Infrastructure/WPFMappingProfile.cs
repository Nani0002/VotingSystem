using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using VotingSystem.Shared.Models;
using VotingSystem.WPF.ViewModel;

namespace VotingSystem.WPF.Infrastructure
{
    public class WPFMappingProfile : Profile
    {
        public WPFMappingProfile()
        {
            CreateMap<LoginViewModel, LoginRequestDto>(MemberList.Source);
            CreateMap<RegisterViewModel, UserRequestDto>(MemberList.Source)
                .ForSourceMember(src => src.PasswordConfirmation, opt => opt.DoNotValidate());

            CreateMap<TopicViewModel, TopicRequestDto>()
                .ForMember(dest => dest.Choices, opt => opt.MapFrom(src => src.Choices.Select(c => c.Value!.ToString()).ToList()));
            CreateMap<TopicResponseDto, TopicViewModel>()
                .ForMember(dest => dest.Choices,
                    opt => opt.MapFrom(src =>
                        src.Choices
                            .GroupBy(c => c)
                            .Select(g => new ChoiceViewModel
                            {
                                Value = g.Key,
                                VoteCount = src.Votes.Count(v => v.SelectedChoices.Contains(g.Key))
                            }).ToList()))
                .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => src.Votes))
                .ForMember(dest => dest.Voters, opt => opt.MapFrom(src => src.Voters));

            CreateMap<VoteResponseDto, VoteViewModel>();

            CreateMap<UserResponseDto, UserViewModel>();
        }
    }
}
