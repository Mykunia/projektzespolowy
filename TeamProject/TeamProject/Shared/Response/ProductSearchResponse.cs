using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamProject.Shared.Models;

namespace TeamProject.Shared.Response
{
    public class ProductSearchResponse
    {
        public int Pages { get; set; }
        public int CPages { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
