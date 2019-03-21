using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBlogProject.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }

        public virtual Blog Blog { get; set; }
        public int BlogId { get; set; }
        public string CommentBody { get; set; }
        public string EditReason { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public Comment()
        {
            DateAdded = DateTime.Now;
        }
    }
}