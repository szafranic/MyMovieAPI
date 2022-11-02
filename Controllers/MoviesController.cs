using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMovieAPI.Models;

namespace MyMovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MoviesContext _context;

        public MoviesController(MoviesContext context)
        {
            _context = context;
        }


		[HttpGet("SearchByTitle/{title}")]
		public async Task<ActionResult<IEnumerable<Movie>>> SearchByTitle(string title)
		{
			return await _context.Movies.Where(m => m.Title.Contains(title)).ToListAsync();
		}

		[HttpGet("SearchByGenre/{genre}")]
		public async Task<ActionResult<IEnumerable<Movie>>> SearchByGenre(string genre)
		{
			return await _context.Movies.Where(m => m.Genre.Contains(genre)).ToListAsync();
		}

		[HttpGet("RandomMovie")]
		public async Task<ActionResult<IEnumerable<Movie>>> RandomMovie()
        {
			return await _context.Movies.Where(m => m.Id == RandomInDB()).ToListAsync();
		}

        private int RandomInDB()
        {
			int idToFind = -1;
			Movie last = _context.Movies.OrderByDescending(x => x.Id).First();
			int lastID = last.Id;

			Random r = new Random();
			int idToTry;

			do
			{
				idToTry = r.Next(0, (lastID+1));
                try
                {
                    if (!(_context.Movies.Find(idToTry).Id == null))
                    {
                        idToFind = idToTry;
                    }
                }
                catch (NullReferenceException)
                {
                }
			} while (idToFind == -1);
            return idToFind;
		}

		[HttpGet("RandomByGenre/{searchGenre}")]
		public async Task<ActionResult<IEnumerable<Movie>>> RandomByGenre(string searchGenre)
		{
			return await _context.Movies.Where(m => m.Id == RandomInDBbyGenre(searchGenre)).ToListAsync();
		}
        // there has to be a better reuse of this code yea??
		private int RandomInDBbyGenre(string searchGenre)
		{
			int idToFind = -1;
			Movie last = _context.Movies.OrderByDescending(x => x.Id).First();
			int lastID = last.Id;

			Random r = new Random();
			int idToTry;

			do
			{
				idToTry = r.Next(0, (lastID + 1));
				try
				{
                    List<Movie> list = _context.Movies.Where(m => m.Genre.Contains(searchGenre) && m.Id == idToTry).ToList();
                    list.First();
                    if (!(list.First().Runtime == null)) {
                        idToFind = idToTry;
                    }
				}
				catch (Exception)
				{
				}
			} while (idToFind == -1);
			return idToFind;
		}

		// GET: api/Movies
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _context.Movies.ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
