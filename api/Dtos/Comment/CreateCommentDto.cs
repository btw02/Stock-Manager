using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateCommentDto
    {

        [Required]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters")]
        [MaxLength(100, ErrorMessage = "Title must be less than 100 characters")]
        public string Title { get; set; } = string.Empty;


        
        [Required]
        [MinLength(3, ErrorMessage = "Content must be at least 3 characters")]
        [MaxLength(300, ErrorMessage = "Content must be less than 300 characters")]
        public string Content { get; set; } = string.Empty;
    }
}