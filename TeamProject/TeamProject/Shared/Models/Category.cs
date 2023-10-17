using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject.Shared.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool Visibility { get; set; } = true;
        public bool Deleted { get; set; } = false;
        [NotMapped]
        public bool Edit { get; set; } = false;
        [NotMapped]
        public bool New { get; set; } = false;
    }
}
