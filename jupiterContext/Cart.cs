using System;
using System.Collections.Generic;

namespace jupiterCore.jupiterContext
{
    public partial class Cart
    {
        public Cart()
        {
            CartProd = new HashSet<CartProd>();
        }

        public int CartId { get; set; }
        public decimal? Price { get; set; }
        public string Location { get; set; }
        public DateTime? EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public string TradingTime { get; set; }
        public byte? IsActivate { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? ContactId { get; set; }

        public int? UserId { get; set; }

        public decimal? DeliveryFee { get; set; }
        public decimal? DepositFee { get; set; }

        public byte? IsEmailSend { get; set; }
        public byte? IsPickup { get; set; }
        public string Region { get; set; }
        public byte? IsPay { get; set; }
        public byte? IsExpired { get; set; }
        public int? CartStatusId { get; set; }
        public decimal? RentalPaidFee { get; set; }
        public decimal? DepositPaidFee { get; set; }

        public virtual CartStatus CartStatus { get; set; }
        public virtual User User { get; set; }
        public virtual Contact Contact { get; set; }
        public virtual ICollection<CartProd> CartProd { get; set; }
    }
}
