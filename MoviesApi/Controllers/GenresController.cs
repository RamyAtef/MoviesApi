
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genreService.GetAll();
            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenre(GenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            await _genreService.Add(genre);

            return Ok(genre);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] GenreDto dto)
        {
            var genre = await _genreService.GetById(id);

            if (genre is null) return NotFound($"No genre was found with ID : {id}");

            genre.Name=dto.Name;
            
            _genreService.Update(genre);

            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genreService.GetById(id);

            if (genre is null) return NotFound($"No genre was found with ID : {id}");

            _genreService.Delete(genre);

            return Ok(genre);
        }
    }
}
