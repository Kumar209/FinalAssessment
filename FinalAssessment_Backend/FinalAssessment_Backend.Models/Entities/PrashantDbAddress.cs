using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinalAssessment_Backend.Models.Entities;

public partial class PrashantDbAddress
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public int AddressTypeId { get; set; }

    public int UserId { get; set; }

    public virtual PrashantDbMasterAddressType AddressType { get; set; } = null!;

    [JsonIgnore]
    public virtual PrashantDbUser User { get; set; } = null!;
}
