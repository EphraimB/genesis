namespace Genesis.Core;

public enum CharacterDirectorInputKind { Text, SpeechReference, SelectedRegion, ReferenceImage }
public sealed record CharacterDirectorInput(CharacterDirectorInputKind Kind, string ContentReference);
public sealed record CharacterDirectorRequest(PersonId PersonId, IReadOnlyList<CharacterDirectorInput> Inputs,
    CharacterSpecification CurrentSpecification, PersonalityProfile CurrentPersonality);
public sealed record CharacterDirectionProposal(CharacterEditPlan CharacterEdits, string Status, bool RequiresUserConfirmation = true);

/// <summary>Future provider boundary. Implementations propose inspectable semantic plans and never receive engine nodes.</summary>
public interface ILocalCharacterDirector
{
    Task<CharacterDirectionProposal> ProposeAsync(CharacterDirectorRequest request, CancellationToken cancellationToken);
}
