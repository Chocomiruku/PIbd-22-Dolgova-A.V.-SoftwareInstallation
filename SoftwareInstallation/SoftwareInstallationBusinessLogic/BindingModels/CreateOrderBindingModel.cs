﻿using System.Runtime.Serialization;

namespace SoftwareInstallationBusinessLogic.BindingModels
{
    public class CreateOrderBindingModel
    {
        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public int PackageId { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public decimal Sum { get; set; }
    }
}