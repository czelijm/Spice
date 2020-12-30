using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Coupon : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType
        {
            Percent = 0,
            Dollars = 1,
        }
        [Required]
        public double Dsicount { get; set; }
        [Required]
        public double MinimumAmmount{ get; set; }
        //picture will be upload to Datebase, no on the other part of host running app
        public byte[] Picture { get; set; }
        public bool IsActive { get; set; }
    }
}
