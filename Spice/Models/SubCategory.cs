using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class SubCategory : BaseEntity
    {
        [Display(Name = "SubCategory name")] //for diplaying "Category name" instead of Name
        [Required]
        public string Name { get; set; }
        [Display(Name = "Category name")]
        [Required]
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
