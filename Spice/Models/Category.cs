using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
