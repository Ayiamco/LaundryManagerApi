﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfle Profile {get; set;}

       

        



    }
}
