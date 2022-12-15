using System.ComponentModel.DataAnnotations.Schema;

namespace WebShopMVC.Models
{
    public class ProductSelectedForAppoinment
    {
        public int Id { get; set; }

        public int AppoinmentId { get; set; }

        [ForeignKey("AppoinmentId")]
        public virtual Appointments Appointments { get; set; }


        public int ProductId { get; set; }


        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }
    }
}
