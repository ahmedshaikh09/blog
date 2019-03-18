using Microsoft.AspNet.Identity;
using MyBlogProject.Models;
using MyBlogProject.Models.Domain;
using MyBlogProject.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System;
using System.Web.Mvc;
using System.Web;

namespace MyBlogProject.Controllers
{
    public class BlogController : Controller
    {
        private List<string> AllowedExtenions = new List<string>
                { ".jpeg", ".jpg", ".gif", ".png" };

        private ApplicationDbContext DbContext { get; set; }

        public BlogController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var blogs = DbContext.Blogs
                .Select(p => new IndexBlogViewModel
                {
                    Id = p.Id,
                    Body = p.Body,
                    MediaUrl = p.MediaUrl,
                    Title = p.Title,
                    DateCreated = p.DateCreated,
                    Published = p.Published,
                    UserEmail = p.User.Email
                }).ToList();

            return View(blogs);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Blog/Index")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
         [Route("Blog/{title}")]
        public ActionResult Add(string title, AddEditBlogViewModel formData)
        {

            title = formData.Title.Replace(" ", "-");

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (formData.FileUpload != null)
            {
                var fileExtension = Path.GetExtension(formData.FileUpload.FileName).ToLower();

                if (!AllowedExtenions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed");
                    return View();
                }
            }

            var userId = User.Identity.GetUserId();

            var blog = new Blog();           
            blog.UserId = userId;
            blog.Body = formData.Body;
            blog.Title = formData.Title;
            blog.Published = formData.Published;
            blog.MediaUrl = UploadFile(formData.FileUpload);

            DbContext.Blogs.Add(blog);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id.Value);

            if (blog == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new AddEditBlogViewModel();
            model.Body = blog.Body;
            model.MedialUrl = blog.MediaUrl;
            model.Title = blog.Title;
            model.Published = blog.Published;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id, AddEditBlogViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (model.FileUpload != null)
            {
                var fileExtension = Path.GetExtension(model.FileUpload.FileName).ToLower();

                if (!AllowedExtenions.Contains(fileExtension))
                {
                    ModelState.AddModelError("", "File extension is not allowed");
                    return View();
                }
            }
            var blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id.Value);

            blog.Published = model.Published;
            blog.Title = model.Title;
            blog.Body = model.Body;
            blog.DateUpdated = DateTime.Now;

            if (model.FileUpload != null)
            {
                blog.MediaUrl = UploadFile(model.FileUpload);
            }

            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id.Value);

            if (blog == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            DbContext.Blogs.Remove(blog);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }

        [HttpGet]
        [Route("Blog/{title}")]
        public ActionResult ReadMore(string title)
        {

            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var blog = DbContext.Blogs.FirstOrDefault(p => p.Title == title);

            if (blog == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new ReadMoreBlogViewModel();
            model.Body = blog.Body;
            model.DateCreated = blog.DateCreated;
            model.DateUpdated = blog.DateUpdated;
            model.Title = blog.Title;
            model.MediaUrl = blog.MediaUrl;

            return View("ReadMore" , model);
        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload/";
                var mappedFolder = Server.MapPath(uploadFolder);

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file.FileName);

                return uploadFolder + file.FileName;
            }


            return null;
        }
    }
}
