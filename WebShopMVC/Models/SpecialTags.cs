using System.ComponentModel.DataAnnotations;

namespace WebShopMVC.Models
{
    public class SpecialTags
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
