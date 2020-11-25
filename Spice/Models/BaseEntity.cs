using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// If I want that SQL server generate Id on insert
        [Key]
        public Guid Id { get; set; }//If I want that sql server will generate Id
        //public string Id { get; set; }//If I want generate Id
        public DateTimeOffset CreatedAt { get; set; }

        public BaseEntity()
        {
//            this.Id = Guid.NewGuid().ToString();//If I want generate Id
            this.CreatedAt = DateTime.Now;

        }
    }
}
