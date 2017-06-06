using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyMovies.Models;

namespace MyMovies.Controllers
{
    public class MoviesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index(int pageNumber = 1)
        {
            int maxItems = 5;


            ViewBag.MoviesCount = db.Movies.Count();
            ViewBag.pageNumber = pageNumber;
            ViewBag.maxItems = maxItems;
            var pages = ViewBag.MoviesCount / ViewBag.maxItems;
            if (ViewBag.MoviesCount % ViewBag.maxItems > 0)
            {
                pages++;
            }
            ViewBag.Pages = pages;

            ViewBag.User = User;
            return View(db.Movies.ToList().Skip((pageNumber - 1) * maxItems).Take(maxItems));
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }


        public ActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Year,Producer,Author")] Movie movie)
        {
            var Img = HttpContext.Request.Files.Count > 0 ? HttpContext.Request.Files[0] : null;
            if (Img != null)
            {
                byte[] Poster = new byte[Img.InputStream.Length];
                Img.InputStream.Read(Poster, 0, Poster.Length);
                movie.Poster = Poster;
            }



            if (!User.Identity.IsAuthenticated)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            if (ModelState.IsValid)
            {
                movie.Author = User.Identity.Name;
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name != movie.Author)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Year,Producer,Author")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                Movie _movie = db.Movies.Where(m => m.Id == movie.Id).FirstOrDefault();
                if (User.Identity.Name != movie.Author)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }
                movie.Author = User.Identity.Name;

                var Img = HttpContext.Request.Files.Count > 0 ? HttpContext.Request.Files[0] : null;
                if (Img != null)
                {
                    byte[] Poster = new byte[Img.InputStream.Length];
                    Img.InputStream.Read(Poster, 0, Poster.Length);
                    movie.Poster = Poster;
                }
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }

            if (User.Identity.Name != movie.Author)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movie movie = db.Movies.Find(id);

            if (User.Identity.Name != movie.Author)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("Poster")]
        public ActionResult GetPoster(int id)
        {
            Movie movie = db.Movies.Where(m => m.Id == id).FirstOrDefault();
            if (movie == null||movie.Poster==null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return File(movie.Poster, System.Net.Mime.MediaTypeNames.Application.Octet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
