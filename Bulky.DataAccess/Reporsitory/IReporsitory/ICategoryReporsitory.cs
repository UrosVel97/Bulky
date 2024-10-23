using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Reporsitory.IReporsitory
{
    //Interfejs 'ICategoryReporsitory' nasledjuje metode interfejsa 'IReporsitory'
    public interface ICategoryReporsitory : IReporsitory<Category>
    {
        void Update(Category obj);// Metoda za azuiranje sloga u tabeli

    }
}
