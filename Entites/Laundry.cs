﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LaundryApi.Models;

namespace LaundryApi.Entites
{
    public class Laundry:ApplicationUser
    {
        [Required]
        public int NoOfEmployees { get; set; }  

        [Required ]
        public string LaundryName { get; set; }

    }
}