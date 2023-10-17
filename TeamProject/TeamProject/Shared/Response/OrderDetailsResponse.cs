using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject.Shared.Response
{
    public class OrderDetailsResponse
    {
        public decimal TotalPrice { get; set; }
        public List<OrderDetailsProductResponse> Products { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailsProductResponse> ProductDetailsResponse { get; set; }
    }
}
