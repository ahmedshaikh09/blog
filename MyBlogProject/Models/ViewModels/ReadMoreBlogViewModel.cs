using MyBlogProject.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBlogProject.Models.ViewModels
{
    public class ReadMoreBlogViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}