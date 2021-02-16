using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItem : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Spicyness { get; set; }
        public enum ESpicy 
        {
            NA=0,
            NotSpicy=1,
            Spicy=2,
            VerySpicy=3
        }

        ////Old way--------------------------------------------------------------------------------------------
        ////see if the images is in the servers, images are on the servers, in db we store only paths
        //public string Image { get; set; }
        ////cons if server stop the docker image of the server, then all images will be gone :(
        ////---------------------------------------------------------------------------------------------------        

        //New way image store in Db, after server delete, images still be existing in Db
        public byte[] Image { get; set; }
        
        [Display(Name="Category")]
        public Guid CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Display(Name = "SubCategory")]
        public Guid SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Range(1,int.MaxValue, ErrorMessage = "Price should be greater than ${1}")]
        public double Price { get; set; }
    }
}
