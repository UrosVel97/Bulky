using Bulky.DataAccess.Reporsitory.IReporsitory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory
{
    public class Reporsitory<T> : IReporsitory<T> where T : class
    {
        private readonly ApplicationDbContext _db; /*Necemo moci da koristimo '_db' objekat za manipulaciju sa bazom,
                                                    * umesto toga koristicemo objekat klase 'DbSet<>'. */

        internal DbSet<T> dbSet;

        public Reporsitory(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet=_db.Set<T>();/*Dodelili smo objektu 'dbSet' klase 'DbSet<>' model (tabelu) sa kojom cemo 
                                     * raditi, posto necemo moci da koristimo objekat '_db' klase 'ApplicationDbContext' za manipulaciju baze */

            
            /*Ovde smo rekli da kada objekat klase 'ApplicationDbContext' vrati objekat tipa 'Product', onda treba
            isto i da popuni njegov navigation properti koji je tipa 'Category'. Navigation properti su propertiji
            koji se ponasaju kao strani kljucevi u tabeli baze podataka, i pokazuju na neki slog neke druge tabele 
            u bazi. U slucaju da imamo vise stranih kljuceva tj. navigation propertija unutar klase 'Product', mi 
            mozemo da naglasimo objektu klase 'ApplicationDbContext' da ih sve popuni kada bude vracao objekat klase
            'Product'. To radimo na sledeci nacin: '_db.Products.Include(u => u.Category).Include(u=>u.DrugiModel).Include(u=>u.TreciModel). */
            _db.Products.Include(u => u.Category);
            _db.ShoppingCarts.Include(u=>u.Product);
            _db.OrderHeaders.Include(u=>u.ApplicationUser);
        }

        public void Add(T entitet)
        {
            //Dodavanje objekta u tabelu
            dbSet.Add(entitet);
        }

        /* Kao parametar prosledjujemo nazive navigaton propertija. Mi mozemo u jednom 
         * parametru tipa string da navedemo vise navigation propertija i sve da ih odvojimo zarezom.
         * Primer: "Category,Book".
         */
        public T Get(Expression<Func<T, bool>>? filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> querry;

            if(tracked)
            {
                //Opcija za pracenje objekta od strane EntityFramework Core-a je ukljucena
                querry = dbSet;
            }
            else
            {
                //Opcija za pracenje objekta od strane EntityFramework Core-a je iskljucena
                querry = dbSet.AsNoTracking();
            }

            querry=querry.Where(filter);/* Linq izraz koji smo prosledili kao parametar metode 'Get()' cemo iskoristiti
                                         * za pronalazenje tacno odredjenog objekta iz tabele i vratiti taj objekat. */



            if (!string.IsNullOrEmpty(includeProperties))
            {
                /* U slucaju da smo naveli vise navigation propertija u string, mi moramo da ih sve
                 * podelimo. Posto je u pitanju string koji ima vise navigation propertija i svi oni su
                 * podeljeni zarezom, onda mi moramo da izvucemo naziv svakog navigation propertija i da naglasimo
                 * da nam je zarez delimeter.*/
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    /*Ova metoda 'Include()' je zaduzena za instanciranje navigation propertija.
                      Kao parametar dobija samo naziv navigation propertija.*/
                    querry = querry.Include(includeProp);
                }
            }

            return querry.FirstOrDefault();//Vracamo objekat koji smo nasli pomocu Linq izraza(filtera).
        }

        /* Kao parametar prosledjujemo nazive navigaton propertija. Mi mozemo u jednom 
         * parametru tipa string da navedemo vise navigation propertija i sve da ih odvojimo zarezom.
         * Primer: "Category,Book".
         */
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null)
        {
            IQueryable<T> querry = dbSet;
            if (filter != null)
            {
                querry = querry.Where(filter);
            }

            if(!string.IsNullOrEmpty(includeProperties))
            {

                /* U slucaju da smo naveli vise navigation propertija u string, mi moramo da ih sve
                 * podelimo. Posto je u pitanju string koji ima vise navigation propertija i svi oni su
                 * podeljeni zarezom, onda mi moramo da izvucemo naziv svakog navigation propertija i da naglasimo
                 * da nam je zarez delimeter.*/
                foreach(var includeProp in includeProperties
                    .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    /*Ova metoda 'Include()' je zaduzena za instanciranje navigation propertija.
                      Kao parametar dobija samo naziv navigation propertija. Metoda u pozadini
                      uzima svaki element iz 'Querry-ja' i nalazi njegov odgovarajuci navigation propertiji.*/
                    querry=querry.Include(includeProp);
                }
            }

            return querry.ToList(); //Moramo da konvertujemo tip 'IQuerryable' u listu
        }

        public void Remove(T entitet)
        {
            //Da bi mogao da izbrises slog iz baze, mora da bude ukljuceno pracenje kod Entity Framework Core-a
            dbSet.Remove(entitet); //Brisemo jedan objekat iz tabele
        }

        public void RemoveRange(IEnumerable<T> entiteti)
        {
            dbSet.RemoveRange(entiteti);//Brisemo kolekciju objekata iz tabele
        }
    }
}
