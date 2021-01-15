using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// If I want that SQL server generate Id on insert
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public string OrderTotalOriginal { get; set; }
        [Required]
        [Display(Name ="Order Total")]
        //[DisplayFormat(DataFormatString ="{0:C}")]
        public string OrderTotalDiscount { get; set; }
        [Required]
        [Display(Name = "Pickup Time")]
        public DateTime PickupTime { get; set; }
        [Required]
        [NotMapped] //No add to Database
        public DateTime PickupDate { get; set; }
        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; }
        public string CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        [Display(Name = "Pickup Name")]
        public string PickupName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string TransactionId { get; set; }
    
    }   
}
