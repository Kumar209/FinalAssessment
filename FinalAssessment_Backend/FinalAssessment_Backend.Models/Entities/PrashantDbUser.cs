using System;
using System.Collections.Generic;

namespace FinalAssessment_Backend.Models.Entities;

public partial class PrashantDbUser
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string? LastName { get; set; }

    public byte[] Email { get; set; } = null!;

    public byte Gender { get; set; }

    public DateOnly DateOfJoining { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public byte[] Phone { get; set; } = null!;

    public byte[]? AlternatePhone { get; set; }

    public string? ImageUrl { get; set; }

    public string Password { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public DateOnly? ModifiedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public string? ModifiedBy { get; set; }

    public virtual ICollection<PrashantDbAddress> PrashantDbAddresses { get; set; }
}
