using AutoMapper;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesService _moviesService;
        private readonly IGenreService _genreService;
        private readonly IMapper _mapper;


        private List<string> _allowsExtentions = new List<string>() { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(IMoviesService moviesService, IGenreService genreService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genreService = genreService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movie = await _moviesService.GetAll();

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movie);

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie is null) return NotFound();

            var data = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(data);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movie = await _moviesService.GetAll(genreId);

            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movie);

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateMovieDto dto)
        {
            if (!_allowsExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and .jpg images are allowed!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max allowed size for poster is 1MB!");

            var isValidGenre = await _genreService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genere ID!");

            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);

            var movie = _mapper.Map<Movie>(dto);
            movie.Poster = dataStream.ToArray();
             

            await _moviesService.Add(movie);

            return Ok(movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateMovieDto dto)
        {
            var movie = await _moviesService.GetById(id);

            if (movie is null) 
                return NotFound($"No movie was found with this ID : {id}");

            var isValidGenre = await _genreService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid genre ID!");

            if (dto.Poster is not null)
            {
                if (!_allowsExtentions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and .jpg images are allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max allowed size for poster is 1MB!");
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.Title = dto.Title;
            movie.Rate = dto.Rate;
            movie.Storeline = dto.Storeline;
            movie.Year = dto.Year;
            movie.GenreId = dto.GenreId;

            _moviesService.Update(movie);

            return Ok(movie);

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _moviesService.GetById(id);

            if (movie is null) return NotFound($"No movie was found with this ID : {id}");

            _moviesService.Delete(movie);

            return Ok(movie);
        }
    }
}
