using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyBlogProject.Models.ViewModels
{
    public class EditCommentViewModel
    {
        [Required]
        public string CommentBody { get; set; }
        [Required]
        public string EditReason { get; set; }
    }
}