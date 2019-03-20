using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyBlogProject.Models.ViewModels
{
    public class AddEditCommentViewModel
    {
        [Required]
        public string CommentBody { get; set; }

        public string EditReason { get; set; }
    }

}