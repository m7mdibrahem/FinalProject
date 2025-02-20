﻿using DataAccess.Repos.IRepos;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repos
{
    public class SeatRepo : Repo<Seat>, ISeatRepo
    {
        public SeatRepo(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
