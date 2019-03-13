using Microsoft.AspNet.Identity;
using MyBlogProject.Models;
using MyBlogProject.Models.Domain;
using MyBlogProject.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Web.Mvc;

namespace MovieDatabase.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext DbContext;

        public BlogController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var model = DbContext.Blogs
                .Select(p => new IndexBlogViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Published = p.Published,
                    Body = p.Body,
                    DateCreated = p.DateCreated,          
                }).ToList();

            return View(model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddEditBlogViewModel formData)
        {
            return AddBlog(null, formData);
        }

        private ActionResult AddBlog(int? id, AddEditBlogViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            if (DbContext.Blogs.Any(p => p.UserId == userId &&
            p.Title == formData.Title &&
            (!id.HasValue || p.Id != id.Value)))
            {
                ModelState.AddModelError(nameof(AddEditBlogViewModel.Title),
                    "Looks like there is a Blog already with that title");

                return View();
            }

            string fileExtension;

            //Validating file upload
            if (formData.Media != null)
            {
                fileExtension = Path.GetExtension(formData.Media.FileName);

                if (!MyBlogProject.Constants.AllowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed.");

                    return View();
                }
            }

            Blog blog;

            if (!id.HasValue)
            {
                blog = new Blog();
                blog.UserId = userId;
                DbContext.Blogs.Add(blog);
            }
            else
            {
                blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id && p.UserId == userId);

                if (blog == null)
                {
                    return RedirectToAction(nameof(BlogController.Index));
                }
            }

            blog.Title = formData.Title;
            blog.Body = formData.Body;


            //Handling file upload
            if (formData.Media != null)
            {
                if (!Directory.Exists(MyBlogProject.Constants.MappedUploadFolder))
                {
                    Directory.CreateDirectory(MyBlogProject.Constants.MappedUploadFolder);
                }

                var fileName = formData.Media.FileName;
                var fullPathWithName = MyBlogProject.Constants.MappedUploadFolder + fileName;

                formData.Media.SaveAs(fullPathWithName);

                blog.MediaUrl = MyBlogProject.Constants.UploadFolder + fileName;
            }
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var userId = User.Identity.GetUserId();

            var blog = DbContext.Blogs.FirstOrDefault(
                p => p.Id == id && p.UserId == userId);

            if (blog == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }


            var model = new AddEditBlogViewModel();
            model.Title = blog.Title;
            model.DateCreated = blog.DateCreated;
            model.Published = blog.Published;
            model.Body = blog.Body;
            model.DateUpdated = DateTime.Now;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, AddEditBlogViewModel formData)
        {
            return AddBlog(id, formData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var userId = User.Identity.GetUserId();

            var blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id && p.UserId == userId);

            if (blog != null)
            {
                DbContext.Blogs.Remove(blog);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(BlogController.Index));
        }

        [HttpGet]
        public ActionResult ReadMore(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction(nameof(BlogController.Index));

            var userId = User.Identity.GetUserId();

            var blog = DbContext.Blogs.FirstOrDefault(p =>
            p.Id == id.Value &&
            p.UserId == userId);

            if (blog == null)
                return RedirectToAction(nameof(BlogController.Index));

            var model = new ReadMoreBlogViewModel();
            model.Title = blog.Title;
            model.DateCreated = blog.DateCreated;
            model.Published = blog.Published;
            model.Body = blog.Body;
            model.MediaUrl = blog.MediaUrl;

            return View(model);
        }
    }
}
