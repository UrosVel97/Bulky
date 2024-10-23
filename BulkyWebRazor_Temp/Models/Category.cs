using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWebRazor_Temp.Models
{
    public class Category
    {

            public Category()
            {
                this.Id = 0;
                this.Name = "";
                this.DisplayOrder = 0;

            }
            public Category(int id, string name, int displayOrder)
            {
                this.Id = id;
                this.Name = name;
                this.DisplayOrder = displayOrder;
            }

            [Key]
            public int Id { get; set; }



            [Required]
            [DisplayName("Naziv Kategorije")] /*Ova anotacija podataka sluzi da naglasi kako ce se properti 'public string Name' prikazati krajnjem korisniku.
        Posto elemenat 'label' u 'Create.cshtml' fajlu treba da ima neki tekst u telu elementa, mi mozemo da koristimo
        'asp-for' tag i koji ce kao vrednost imati naziv propertija klase 'Category'. Kada smo to odradili onda nije potrebno da stavljamo neki 
        tekst u telu elementa da bi naglasili naziv labele, vec ce naziv labele biti naziv propertija klase ili  ovom slucaju posto smo koristili 
        anotaciju podataka 'DisplayName("Naziv Kategorije")' onda ce tekst: 'Naziv Kategorije' biti naziv labele. */
            [MaxLength(30, ErrorMessage = "Naziv Kategorije ne sme imati vise od 30 karaktera!")]/*Ova anotacija podataka sluzi za validaciju propertija na strani servera.
                        Ako kranji korisnik unese string gde je broj karaktera veci od 30, onda 
                        objekat nece biti validan.*/
            public string Name { get; set; }



            [DisplayName("Redosled prikazivanja")]
            [Range(1, 100, ErrorMessage = "Vrednost mora biti izmedju 1 i 100")]/*Ova anotacija podataka sluzi za validaciju propertija na strani servera.
                       *Ako krajnji korisnik unese broj koji nije izmedju 1 i 100, onda objekat nece 
                       *biti validan.*/
            public int DisplayOrder { get; set; }// Ovaj property odredjuje redosled kako ce se objekti tipa 'Category' prikazati krajnjem korisniku 
        }
    }

