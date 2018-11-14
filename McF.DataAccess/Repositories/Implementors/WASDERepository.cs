using System.Collections.Generic;
using System.Data;
using McF.Contracts;
using System;

namespace McF.DataAcess.Repositories
{
    public class WASDERepository 
    {
        private IDbHelper dbHelper = null;
        public WASDERepository(IDbHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }
    }
}
