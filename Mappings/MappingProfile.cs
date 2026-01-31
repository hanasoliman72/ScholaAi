using AutoMapper;
using ScholaAi.DTOs.Rating;
using ScholaAi.Models;

namespace ScholaAi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Rating mappings
            CreateMap<rating, ratingDto>();
            CreateMap<ratingCreateDto, rating>();
            CreateMap<ratingUpdateDto, rating>();
        }
    }
}
