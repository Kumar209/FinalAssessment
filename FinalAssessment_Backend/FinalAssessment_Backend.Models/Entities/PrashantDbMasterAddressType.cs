using System;
using System.Collections.Generic;

namespace FinalAssessment_Backend.Models.Entities;

public partial class PrashantDbMasterAddressType
{
    public int Id { get; set; }

    public string AddressType { get; set; } = null!;

    public virtual ICollection<PrashantDbAddress> PrashantDbAddresses { get; set; } = new List<PrashantDbAddress>();
}
