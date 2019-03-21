using Microsoft.AspNet.Identity;
using MyBlogProject.Models;
using MyBlogProject.Models.Domain;
using MyBlogProject.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Web.Mvc;
using System.Web;
using System.Text.RegularExpressions;

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

        public ActionResult Index(string search)
        {
            if (search != null)
            {
                var blogs = DbContext.Blogs
                .Where(p => p.Title.Contains(search) ||
                                p.Body.Contains(search) ||
                                p.Slug.Contains(search))
                .Select(p => new IndexBlogViewModel
                {
                    Id = p.Id,
                    Body = p.Body,
                    MediaUrl = p.MediaUrl,
                    Title = p.Title,
                    DateCreated = p.DateCreated,
                    Published = p.Published,
                    UserEmail = p.User.Email,
                    Slug = p.Slug,
                    Comments = p.Comment,
                }).ToList();

                return View(blogs);
            }
            else
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
                        UserEmail = p.User.Email,
                        Slug = p.Slug,
                        Comments = p.Comment,
                    }).ToList();

                return View(blogs);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("blog/View")]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Route("blog/{slug}")]
        [Authorize(Roles = "Admin")]

        public ActionResult Add(string slug, AddEditBlogViewModel formData)
        {
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

            string str = formData.Title.ToLower();
            str = str.Replace("-", " ");
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");

            var blog = new Blog();
            blog.UserId = userId;
            blog.Body = formData.Body;
            blog.Title = formData.Title;
            blog.Slug = str;
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
        [Route("blog/{slug}")]
        public ActionResult ReadMore(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var blog = DbContext.Blogs.FirstOrDefault(p => p.Slug == slug);

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

            return View("ReadMore", model);
        }

        [HttpGet]
        public ActionResult AddComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int id, AddEditCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var comment = new Comment();

            var userId = User.Identity.GetUserId();
            Blog blog;
            blog = DbContext.Blogs.FirstOrDefault(p => p.Id == id);
            comment.BlogId = blog.Id;
            comment.UserId = userId;
            comment.CommentBody = model.CommentBody;

            DbContext.Comments.Add(comment);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }

        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult EditComment(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == id.Value);

            if (comment == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var model = new AddEditCommentViewModel();
            model.CommentBody = comment.CommentBody;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult EditComment(int? id, AddEditCommentViewModel model)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == id.Value);

            comment.CommentBody = model.CommentBody;
            comment.EditReason = model.EditReason;
            comment.DateChanged = DateTime.Now;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
        }
        [Authorize(Roles = "Admin , Moderator")]
        public ActionResult DeleteComment(int? Id)
        {
            if (!Id.HasValue)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            var comment = DbContext.Comments.FirstOrDefault(p => p.Id == Id.Value);

            if (comment == null)
            {
                return RedirectToAction(nameof(BlogController.Index));
            }

            DbContext.Comments.Remove(comment);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(BlogController.Index));
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
