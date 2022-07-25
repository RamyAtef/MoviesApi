using AutoMapper;

namespace MoviesApi.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDetailsDto>().ReverseMap();
            CreateMap<Movie, CreateMovieDto>().ReverseMap()
                .ForMember(src=>src.Poster,dest=>dest.Ignore());
            CreateMap<Movie, MovieBaseDto>().ReverseMap();
        }
    }
}
