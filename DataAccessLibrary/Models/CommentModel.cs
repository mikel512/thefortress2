using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Models
{
    public class CommentModel
    {
        [Key] public int CommentId { get; set; }
        [Key] public int EventId { get; set; }
        [Key] public int? ParentCommentId { get; set; }
        
        [Display(Name = "Write your comment:")]
        [Required(ErrorMessage = "Comment required")]
        [MaxLength(2048, ErrorMessage ="Message too long")]
        public string Content { get; set; }
        
        [Required] 
        public DateTime DateStamp { get; set; }
        public int Upvotes { get; set; }
        [Required] 
        public string UserId { get; set; }
        public string UserName { get; set; }

        // ChildNodeId, ChildNodeComment key value pairs
        public List<CommentModel> Children { get; set; }
        // Node height
        public int Height { get; set; }

    }
}