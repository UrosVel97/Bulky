using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Product
    {

        public Product()
        {
            
        }
        public Product(int id, string title, string descr, 
            string isbn, string author, double listprice,double price, double price50, double price100 ,int catId)
        {
            this.Author = author;
            this.Id = id;   
            this.Title = title; 
            this.Description = descr;   
            this.ListPrice = listprice;
            this.Price = price;
            this.Price50 = price50; 
            this.Price100 = price100;
            this.ISBN = isbn;
            this.CategoryId = catId;
        }

        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Naslov")]
        public string Title { get; set; }

        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Required]
        public string ISBN{ get; set; }
        [Required]
        [Display(Name = "Autor")]
        public string Author{ get; set; }

        [Required]
        [Display(Name = "Lista cena")]
        [Range(1, 1000)]
        public double ListPrice { get; set; }



        [Required]
        [Display(Name ="Cene za 1-50")]
        [Range(1,1000)]
        public double Price{ get; set; }


        [Required]
        [Display(Name = "Cene za 50+")]
        [Range(1, 1000)]
        public double Price50 { get; set; }


        [Required]
        [Display(Name = "Cene za 100+")]
        [Range(1, 1000)]
        public double Price100 { get; set; }


        public int CategoryId { get; set; } //Ova kolona u tabeli 'Products' ce predstavljati strani kljuc koji pokazuje na odgovarajuci slog u tabeli 'Category'
        [ForeignKey("CategoryId")]

        [ValidateNever]
        public Category Category { get; set; }


        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
