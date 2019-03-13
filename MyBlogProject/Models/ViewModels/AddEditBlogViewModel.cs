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

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public bool Published { get; set; }

        [AllowHtml]
        public string Body { get; set; }

        public HttpPostedFileBase Media { get; set; }
    }
}