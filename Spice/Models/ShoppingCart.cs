using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ShoppingCart : BaseEntity
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public Guid ApplicationUserId { get; set; }
        //When you don't want to add new field to DB
        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        
        public Guid MenuItemId { get; set; }
        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        [Range(1,int.MaxValue, ErrorMessage ="Please enter the value greater or equal than {1}")]
        public int Count { get; set; }

    }
}
