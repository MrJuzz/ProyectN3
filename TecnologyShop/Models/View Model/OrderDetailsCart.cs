﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecnologyShop.Models.View_Model
{
    public class OrderDetailsCart
    {
      public List<ShoppingCart> listCart { get; set; }
      public OrderHeader OrderHeader { get; set; }
    }
}

