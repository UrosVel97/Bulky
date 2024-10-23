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
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } //Id korisnika koji je prijavljen na sajt i koji pokusava da naruci proizvode
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }


        public DateTime OrderDate { get; set; }//Datum kada je naruceno
        public DateTime ShippingDate { get; set; }//Datum kada treba da se posalje

        public double OrderTotal {  get; set; } //Ukupna cena


        public string? OrderStatus { get; set; } //Status porudzbine
        public string? PaymentStatus { get; set; }//Status da li je placeno ili nije
        public string? TrackingNumber {  get; set; } //Broj za pracenje posiljke
        public string? Carrier {  get; set; } //Naziv kurirske sluzbe

        public DateTime PaymentDate { get; set; }//Datum kada je placeno
        public DateOnly PaymentDueDate { get; set; }//Ovaj tip 'DateOnly' Postoji samo u '.Net verzija 8.0.0'. Ovaj properti cuva datum do kada mora da se plati, ovo je samo za firme koje imaju rok za placanje 30 dana


        public string? SessionId { get; set; }//Id Sesije za placanje preko Stripe-a
        public string? PaymentIntentId {  get; set; }//Ovo 'Stripe' generise Id za placanje, ali tek kada je sesija za placanje bila uspesna!



        //Sledeci propertiji su za podatke o korisniku koji treba da primi porudzbinu 
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string Name { get; set; }



    }
}
