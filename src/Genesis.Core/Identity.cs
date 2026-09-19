namespace Genesis.Core;

public readonly record struct PersonId
{
    public PersonId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("A person ID cannot be empty.", nameof(value));
        Value = value;
    }

    public Guid Value { get; }
    public static PersonId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString("D");
}

public sealed record PersonIdentity
{
    public PersonIdentity(PersonId personId, string displayName, int revisionNumber = 1,
        Guid? revisionId = null, Guid? parentRevisionId = null,
        DateTimeOffset? createdUtc = null, DateTimeOffset? modifiedUtc = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("A display name is required.", nameof(displayName));
        if (revisionNumber < 1) throw new ArgumentOutOfRangeException(nameof(revisionNumber));
        PersonId = personId;
        DisplayName = displayName.Trim();
        RevisionNumber = revisionNumber;
        RevisionId = !revisionId.HasValue || revisionId.Value == Guid.Empty ? Guid.NewGuid() : revisionId.Value;
        ParentRevisionId = parentRevisionId;
        CreatedUtc = createdUtc ?? DateTimeOffset.UtcNow;
        ModifiedUtc = modifiedUtc ?? CreatedUtc;
    }

    public PersonId PersonId { get; }
    public string DisplayName { get; }
    public int RevisionNumber { get; }
    public Guid RevisionId { get; }
    public Guid? ParentRevisionId { get; }
    public DateTimeOffset CreatedUtc { get; }
    public DateTimeOffset ModifiedUtc { get; }

    public PersonIdentity NextRevision(string? displayName = null) => new(
        PersonId, displayName ?? DisplayName, RevisionNumber + 1, Guid.NewGuid(), RevisionId,
        CreatedUtc, DateTimeOffset.UtcNow);
}

public enum DominantHand { Left, Right, Ambidextrous }

public sealed record PhysicalFacts
{
    public const double MinimumHeightMeters = 1.0;
    public const double MaximumHeightMeters = 2.5;
    public const double MinimumWeightKilograms = 25;
    public const double MaximumWeightKilograms = 350;
    public const int MinimumAgeYears = 1;
    public const int MaximumAgeYears = 125;

    public PhysicalFacts(double heightMeters, double weightKilograms, int ageYears,
        DominantHand dominantHand = DominantHand.Right)
    {
        if (!double.IsFinite(heightMeters) || heightMeters is < MinimumHeightMeters or > MaximumHeightMeters)
            throw new ArgumentOutOfRangeException(nameof(heightMeters));
        if (!double.IsFinite(weightKilograms) || weightKilograms is < MinimumWeightKilograms or > MaximumWeightKilograms)
            throw new ArgumentOutOfRangeException(nameof(weightKilograms));
        if (ageYears is < MinimumAgeYears or > MaximumAgeYears)
            throw new ArgumentOutOfRangeException(nameof(ageYears));
        if (!Enum.IsDefined(dominantHand)) throw new ArgumentOutOfRangeException(nameof(dominantHand));
        HeightMeters = heightMeters;
        WeightKilograms = weightKilograms;
        AgeYears = ageYears;
        DominantHand = dominantHand;
    }

    public double HeightMeters { get; }
    public double WeightKilograms { get; }
    public int AgeYears { get; }
    public DominantHand DominantHand { get; }
}
