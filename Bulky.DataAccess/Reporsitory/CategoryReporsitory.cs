using Bulky.DataAccess.Reporsitory.IReporsitory;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    /* Klasa 'CategoryReporsitory' treba da nasledi interfejs 'ICategoryReporsitory' i da implementira sve
     * metode tog interfejsa. Interfejs 'ICategoryReporsitory' nasledjuje templejtski interfejs 'IReporsitory<>'.
     * Templejtski interfejs 'IReporsitory<>' ima 5 metoda. Interfejs 'ICategoryReporsitory' ima u svom telu dve
     * metode i plus 5 metoda koje nasledjuje od templejtskog interfejsa, sto znaci da interfejs 'ICategoryReporsitory'
     * ima ukupno 7 metoda. Klasa 'CategoryReporsitory' bi trebalo onda da implementira 7 metoda u svom telu, ali
     * to ne mora da bude slucaj. Mi smo deklarisali i definisali templejtsku klasu 'Reporsitory<>' koja nasledjuje
     * templejtski interfejs 'IReporsitory<>' i implementira sve njegove metode. Mi mozemo da kazemo da klasa
     * 'CategoryReporsitory' nasledjuje templejtsku klasu 'Reporsitory<>' i nasledjuje interfejs 'ICategoryReporsitory',
     * na taj nacin klasa 'CategoryReporsitory' treba da implementira samo dve metode interfejsa 'ICategoryReporsitory',
     * ostalih 5 metoda nasledjuje implementaciju tih 5 metoda od templejtske klase 'Reporsitory<>'. Stim smo smanjili
     * dupliranje koda.*/
    public class CategoryReporsitory : Reporsitory<Category>, ICategoryReporsitory
    {

        /*Neophodno je ponovo deklarisati i definisati objekat klase 'ApplicationDbContext'
         * jer mora da se kreira objekat klase 'ApplicationDbContext' i dodeli konstruktoru
         * bazne klase 'Reporsitory<>'.*/
        private ApplicationDbContext _db;


        /*Kreiranje objekta klase 'ApplicationDbContext' i dodeljivanje konstruktoru 
         * bazne klase 'Reporsitory<>'*/
        public CategoryReporsitory(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }



        //Azuriranje objekta u bazi
        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
