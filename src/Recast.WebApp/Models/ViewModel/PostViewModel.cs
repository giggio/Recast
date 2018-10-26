using System;
using System.ComponentModel.DataAnnotations;

namespace Recast.WebApp.Models.ViewModel
{
    public class PostViewModel
    {
        [Required]
        public string FeedName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, RegularExpression("^[Hh][Tt][Tt][Pp][Ss]?://.+", ErrorMessage = "Must start with http or https.")]
        public string SongLink { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public DateTime PublishDate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}