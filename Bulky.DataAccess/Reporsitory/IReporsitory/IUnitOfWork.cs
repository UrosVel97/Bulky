using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    /* Ovaj interfejs 'IUnitOfWork' ce se koristiti da se smanji dupliranje koda i sa njim ce se
     * manipulisati bazom u svakom kontroleru. Moze da se desi da u bazi pored tabele 'Category' ima jos
     * dosta drugih tabela, i da na njima isto treba da se vrse CRUD operacije. Svi ti interfejsi poput 
     * 'ICategoryReporsitory' bi bili zaduzeni za manipulaciju tih tabela. Svaki interfejs poput 'ICategoryReporsitory' 
     * bi bio zaduzen za jednu tabelu. Svaki interfejs bi imao i metodu'Save()' i imao bi implementaciju te metode. 
     * Da bi se smanjilo dupliranje koda, tj. da ne bi svaki interfejs imao metodu 'Save()' i svaka klasa 
     * sto implementira taj interfejs ne bi morala da istu metodu na isti nacin implementira, onda se 
     * koristi 'UnitOfWork'.
     * U telu interfejsa 'IUnitOfWork' ce se nalaziti svi interfejsi koji su zaduzeni za manipulaciju baze. 
     * Takodje ce se nalaziti i metode koje bi trebalo svaki interfejs da ima.   
     */
    public interface IUnitOfWork
    {

        ICategoryReporsitory Category { get; }
        IProductReporsitory Proizvod{ get; }

        ICompanyReporsitory Kompanija { get; }

        IShoppingCartReporsitory ShoppingCart { get; }

        IApplicationUserReporsitory ApplicationUser { get; }
        IOrderDetailReporsitory OrderDetail { get; }
        IOrderHeaderReporsitory OrderHeader { get; }
        void Save();
    }
}
