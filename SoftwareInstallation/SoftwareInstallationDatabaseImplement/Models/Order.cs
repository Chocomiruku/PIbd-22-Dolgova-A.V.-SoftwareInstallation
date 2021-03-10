﻿using SoftwareInstallationBusinessLogic.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SoftwareInstallationDatabaseImplement.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public virtual Package Package { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public decimal Sum { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public DateTime DateCreate { get; set; }

        public DateTime? DateImplement { get; set; }
    }
}