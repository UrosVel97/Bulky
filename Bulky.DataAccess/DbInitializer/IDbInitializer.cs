﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {

        public void Initialize(); //Ova metoda ce da bude zaduzena da Seed-uje nasu bazu podataka

    }
}
