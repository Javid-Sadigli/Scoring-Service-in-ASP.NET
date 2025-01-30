using AutoMapper;
using Scoring_Service.Models.Dtos.Requests;
using Scoring_Service.Models.Entities;

namespace Scoring_Service.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerRequestDto, CustomerRequest>();
        }
    }
}
