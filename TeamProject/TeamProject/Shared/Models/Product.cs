using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject.Shared.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public List<Picture> Pictures { get; set; } = new List<Picture>();
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
        public bool Featured { get; set; } = false;
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public bool Visibility { get; set; } = true;
        public bool Deleted { get; set; } = false;
        [NotMapped]
        public bool Edit { get; set; } = false;
        [NotMapped]
        public bool New { get; set; } = false;
    }
}
