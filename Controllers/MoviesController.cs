using System;
using System.IO;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FilmsCatalog.Models;
using FilmsCatalog.Data;

namespace FilmsCatalog.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly int _pageSize;
        private readonly int _pagingLinkNumber;


        public MoviesController(IConfiguration config, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _pageSize = config.GetValue<int>("Settings:PageSize", 10);
            _pagingLinkNumber = config.GetValue<int>("Settings:PagingLinkNumber", 3);
        }

        // GET: Movies
        public async Task<IActionResult> Index(int? page)
        {
            await FillDb();
            IQueryable<Movie> source = _context.Movie;
            int totalCount = await _context.Movie.CountAsync();
            PageInfo pageInfo = new PageInfo(totalCount, page, _pageSize, _pagingLinkNumber);
            var movies = await source
                            .Join(_context.Users,
                                s => s.CreatorIdentityName,
                                u => u.UserName,
                                (s,u) => new Movie{
                                    ID = s.ID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    CreatorName = u.FullName,
                                    ReleaseYear = s.ReleaseYear,
                                    Director = s.Director,
                                    IsCreator = s.CreatorIdentityName == User.Identity.Name,
                                })
                            .OrderByDescending(e => e.ReleaseYear)
                            .Skip((pageInfo.CurrentPage - 1) * pageInfo.PageSize)
                            .Take(pageInfo.PageSize)
                            .ToListAsync();

            IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Movies = movies };
            return View(ivm);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
                
            if (movie == null)
            {
                return NotFound();
            }
            movie.CreatorName = await CreatorName(movie.CreatorIdentityName);
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            movie = uploadPoster(movie);
            try
            {
                if (ModelState.IsValid)
                {
                    movie.CreatorIdentityName = User.Identity.Name;
                    _context.Add(movie);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            if(!IsCreator(movie.CreatorIdentityName))
                return RedirectToAction(nameof(Index));

            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.ID)
            {
                return NotFound();
            }

            if(!IsCreator((await _context.Movie.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id)).CreatorIdentityName))
                return RedirectToAction(nameof(Index));

            movie = uploadPoster(movie);
            if (ModelState.IsValid)
            {
                try
                {
                    movie.CreatorIdentityName = User.Identity.Name;
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

             if(!IsCreator(movie.CreatorIdentityName))
                return RedirectToAction(nameof(Index));

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            if(!IsCreator(movie.CreatorIdentityName))
                return RedirectToAction(nameof(Index));

            try
            {
                _context.Movie.Remove(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }

        private async Task<string> CreatorName(string identityName)
        {
            var creator = await _context.Users.FirstAsync(e => e.Email == identityName);
            return creator.FirstName + 
                (string.IsNullOrEmpty(creator.MiddleName) ? " " + creator.MiddleName : "") + 
                (string.IsNullOrEmpty(creator.LastName) ? " " + creator.LastName : "");
        }

        Movie uploadPoster(Movie movie)
        {
            if(movie.PosterFile != null)
            {
                byte[] imageData = null;
                try
                {
                    using (var binaryReader = new BinaryReader(movie.PosterFile.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)movie.PosterFile.Length);
                        movie.Poster = imageData;
                        // if (imageData.Length < 2097152)
                        // {
                        //     movie.Poster = imageData;
                        // }
                        // else
                        // {
                        //     ModelState.AddModelError("PosterFile", "???????????? ?????????????????????? ???? ???????????? ?????????????????? 2MB");
                        // }
                    } 
                }
                catch (Exception)
                {
                    ModelState.AddModelError("PosterFile", "???????????????????????????? ????????????. ???????????????????? ???????????? ????????");
                }
            }
            return movie;
        }

        private bool IsCreator(string creatorIdentityName)
        {
            return User.Identity.Name == creatorIdentityName;
        }

        // ???????????????????? ???????? ?????????????????? ??????????????
        private async Task FillDb()
        {
            if(!User.Identity.IsAuthenticated)
                return;
                
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Title == "?????????? 1");
                
            if (movie == null)
            {
                Movie m = new Movie{
                    CreatorIdentityName = User.Identity.Name,
                    Title = "??????????",
                    ReleaseYear = 1895
                };
                for (int i = 1; i <= 100; i++)
                {
                    m.ID = 0;
                    m.Title = $"?????????? {i}";
                    m.ReleaseYear += 1;
                    _context.Add(m);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
