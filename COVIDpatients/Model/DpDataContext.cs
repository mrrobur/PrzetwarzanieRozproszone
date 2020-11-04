﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COVIDpatients.Model
{
    public class DpDataContext : DbContext
    {
        public DpDataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Patients> Patients { get; set; }
    }
}
