﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryApp.Core.Models;

public class OrderWithProduct
{
	public Order Order { get; set; }
	public ProductDTO Product { get; set; }
}

