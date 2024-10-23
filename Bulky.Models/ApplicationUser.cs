using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models
{

    /* Ova klasa nam sluzi kao ekstenzija klase 'IdentityUser'. Klasa 'IdentityUser' je preslikana na tebelu 'AspNetUsers' u bazi podataka pomocu EntityFrameworkCore-a. 
       Ta tabela vec ima kolone koje su potrebne za autentikaciju korisnika. Mi zelimo da dodamo jos neke kolone u tabelu 'AspNetUsers'.
       Zato mi kreiramo klasu 'ApplicationUser' koja nasledjuje klasu 'IdentityUser', i dodamo odgovarajuce propertije unutar klase.
    */
      public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        
        public int? CompanyId { get; set; }/*Ovaj properti moze imati null vrednost, 
                                            * jer ako korisniku nije vezan za kompaniju
                                            * onda nece imati strani kljuc koji pokazuje na kompaniju*/
        
        //Korisnik ce imati strani kljuc koji pokazuje na Kompaniju, samo ako
        //je taj korisnik asociran sa kompanijom
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company Company { get; set; }
    }
}
