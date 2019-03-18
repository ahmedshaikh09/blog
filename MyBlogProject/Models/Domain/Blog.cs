using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBlogProject.Models.Domain
{
    public class Blog
    {
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public string Title { get; set; }
        public bool Published { get; set; }
        public string Body { get; set; }
        public string MediaUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public Blog()
        {
            DateCreated = DateTime.Now;
        }
    }
}