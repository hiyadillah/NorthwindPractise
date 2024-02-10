using Microsoft.AspNetCore.Mvc;
using NorthwindWebMvc.Basic.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NorthwindWebMvc.Basic.Models.Dto
{
    public class ProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Range(EntityConstantModel.MIN_PRICE, EntityConstantModel.MAX_PRICE)]
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public string? Photo { get; set; }
        public int CategoryId { get; set; }

        //relasi one-to-many
        public virtual CategoryDto Category { get; set; }
    }
}
