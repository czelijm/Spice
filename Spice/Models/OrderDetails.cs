using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderDetails : BaseEntity
    {
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderHeader Order { get; set; }
        [Required]
        public Guid MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        [Range(minimum:1,int.MaxValue)]
        public int Count { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Price { get; set; }

    }
}
