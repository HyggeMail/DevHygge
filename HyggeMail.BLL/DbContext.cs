﻿
using HyggeMail.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL
{
    public class DbContext : IDisposable
    {
        /// <summary>
        /// protected, so it only visible for inherited class
        /// </summary>
        [ThreadStatic]
        protected static HyggeMailEntities Context;

        /// <summary>
        /// Initialize Db Context
        /// </summary>
        public DbContext()
        {
            if (Context == null)
            {
                Context = new HyggeMailEntities();
            }
        }

       
        /// <summary>
        /// Dispose the context
        /// </summary>
        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();

                Context = null;
            }
        }

        /// <summary>
        /// Reinitiate the database context.
        /// </summary>
        public void ReinitiateContext()
        {
            Dispose();
            Context = new HyggeMailEntities();
        }
    }
}
