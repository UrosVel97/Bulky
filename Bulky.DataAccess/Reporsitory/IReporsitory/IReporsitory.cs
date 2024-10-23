using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    /*Ovaj interfejs nam sluzi da smanjimo dupliranje koda kada manipulisemo bazom,
     * za vracanje, dodavanje i brisanje elemenata iz baze.*/
    public interface IReporsitory<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null); //Vraca sve slogove(redove) iz tabele. Vraca sve elemente date klase.
        T Get(Expression<Func<T, bool>>? filter, string? includeProperties = null,bool tracked=false);/* Metoda koja pronalazi element na osnovu zadatog filtera. 
                                                 * To moze biti pomocu id-ja, neke vrednosti nekog propertija ili drugo.
                                                 * Koristice se 'Linq' izrazi za pronalaznje elemenata. Drugi parametar je da se navedu nazivi svih navigation
                                                 * propertija kojio treba takodje da se vrate iz baze. Treci parametar je da naglasimo da li zelimo
                                                 da opcija za pracenje objekta, od strane EntityFramework Core-a, bude ukljucena*/
        void Add(T entitet);// Dodavanje objekta u tabelu
        void Remove(T entitet);// Brise jedan objekat iz tabele
        void RemoveRange(IEnumerable<T> entiteti);// Brise kolekciju objekta iz table. Vise slogova brise iz tabele
                                                 
    }
}
