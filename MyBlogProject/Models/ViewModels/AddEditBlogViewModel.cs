using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyBlogProject.Models.ViewModels
{
    public class AddEditBlogViewModel
    {
        [Required]
        public string Title { get; set; }

        public bool Published { get; set; }

        [Required]
        [AllowHtml]
        public string Body { get; set; }

        public HttpPostedFileBase FileUpload { get; set; }

        public string MedialUrl { get; set; }
    }
}