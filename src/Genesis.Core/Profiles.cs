namespace Genesis.Core;

public sealed record PersonalityProfile
{
    public PersonalityProfile(double confidence = 0.5, double talkativeness = 0.5,
        double curiosity = 0.5, double empathy = 0.5, double playfulness = 0.5,
        double emotionalIntensity = 0.5, double calmness = 0.5, double leadership = 0.5,
        IEnumerable<string>? conversationalTendencies = null)
    {
        Confidence = Trait(confidence, nameof(confidence));
        Talkativeness = Trait(talkativeness, nameof(talkativeness));
        Curiosity = Trait(curiosity, nameof(curiosity));
        Empathy = Trait(empathy, nameof(empathy));
        Playfulness = Trait(playfulness, nameof(playfulness));
        EmotionalIntensity = Trait(emotionalIntensity, nameof(emotionalIntensity));
        Calmness = Trait(calmness, nameof(calmness));
        Leadership = Trait(leadership, nameof(leadership));
        ConversationalTendencies = (conversationalTendencies ?? []).Select(Clean).Where(x => x is not null).Cast<string>().Distinct(StringComparer.Ordinal).ToArray();
    }

    public double Confidence { get; }
    public double Talkativeness { get; }
    public double Curiosity { get; }
    public double Empathy { get; }
    public double Playfulness { get; }
    public double EmotionalIntensity { get; }
    public double Calmness { get; }
    public double Leadership { get; }
    public IReadOnlyList<string> ConversationalTendencies { get; }

    private static double Trait(double value, string name) =>
        double.IsFinite(value) && value is >= 0 and <= 1 ? value : throw new ArgumentOutOfRangeException(name);
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public enum VoiceDesignKind { Unspecified, DesignedSynthetic, TrainedLocal }

public sealed record VoiceProfile
{
    public VoiceProfile(Guid voiceId, string displayName, VoiceDesignKind designKind = VoiceDesignKind.Unspecified,
        string? providerId = null, string? modelReference = null, string? speakerReference = null,
        double speakingRate = 1, double pitchSemitones = 0, double volume = 1,
        string? style = null, string? description = null)
    {
        if (voiceId == Guid.Empty) throw new ArgumentException("A voice ID is required.", nameof(voiceId));
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("A voice display name is required.", nameof(displayName));
        if (!double.IsFinite(speakingRate) || speakingRate is < 0.5 or > 2) throw new ArgumentOutOfRangeException(nameof(speakingRate));
        if (!double.IsFinite(pitchSemitones) || pitchSemitones is < -12 or > 12) throw new ArgumentOutOfRangeException(nameof(pitchSemitones));
        if (!double.IsFinite(volume) || volume is < 0 or > 1) throw new ArgumentOutOfRangeException(nameof(volume));
        VoiceId = voiceId;
        DisplayName = displayName.Trim();
        DesignKind = designKind;
        ProviderId = Clean(providerId);
        ModelReference = SafeReference(modelReference);
        SpeakerReference = Clean(speakerReference);
        SpeakingRate = speakingRate;
        PitchSemitones = pitchSemitones;
        Volume = volume;
        Style = Clean(style);
        Description = Clean(description);
    }

    public Guid VoiceId { get; }
    public string DisplayName { get; }
    public VoiceDesignKind DesignKind { get; }
    public string? ProviderId { get; }
    public string? ModelReference { get; }
    public string? SpeakerReference { get; }
    public double SpeakingRate { get; }
    public double PitchSemitones { get; }
    public double Volume { get; }
    public string? Style { get; }
    public string? Description { get; }

    public static VoiceProfile CreateDefault(string personName) => new(Guid.NewGuid(), $"{personName} voice");
    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static string? SafeReference(string? value)
    {
        var cleaned = Clean(value);
        if (cleaned is null) return null;
        var normalized = cleaned.Replace('\\', '/');
        if (Path.IsPathRooted(normalized) || normalized.Split('/').Contains("..", StringComparer.Ordinal))
            throw new ArgumentException("Voice assets must use package-relative references.", nameof(value));
        return normalized;
    }
}
