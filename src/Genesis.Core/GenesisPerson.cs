namespace Genesis.Core;

public sealed record GenesisPerson
{
    public GenesisPerson(PersonIdentity identity, CharacterSpecification character,
        PersonalityProfile personality, VoiceProfile voice,
        IReadOnlyDictionary<string, string>? metadata = null)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Character = character?.Clone() ?? throw new ArgumentNullException(nameof(character));
        Personality = personality ?? throw new ArgumentNullException(nameof(personality));
        Voice = voice ?? throw new ArgumentNullException(nameof(voice));
        if (Character.PersonId != Identity.PersonId)
            throw new ArgumentException("Character specification must belong to the same person.", nameof(character));
        Metadata = new Dictionary<string, string>(metadata ?? new Dictionary<string, string>(), StringComparer.Ordinal);
    }

    public PersonIdentity Identity { get; }
    public CharacterSpecification Character { get; }
    public PersonalityProfile Personality { get; }
    public VoiceProfile Voice { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }

    public static GenesisPerson Create(string displayName, PhysicalFacts? facts = null)
    {
        var identity = new PersonIdentity(PersonId.New(), displayName);
        return new GenesisPerson(identity, CharacterSpecification.CreateDefault(identity.PersonId, facts),
            new PersonalityProfile(), VoiceProfile.CreateDefault(displayName));
    }

    public GenesisPerson WithDisplayName(string displayName) => new(
        Identity.NextRevision(displayName), Character, Personality, Voice, Metadata);

    public GenesisPerson WithCharacter(CharacterSpecification character) => new(
        Identity.NextRevision(), character, Personality, Voice, Metadata);
}
