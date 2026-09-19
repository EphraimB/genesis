namespace Genesis.Core;

public enum CharacterEditOperationKind { SetValue, AdjustNumber, LockProperty, UnlockProperty, LockCategory, UnlockCategory }

public sealed record CharacterEditOperation(
    CharacterEditOperationKind Kind,
    string? Path = null,
    SemanticValue? Value = null,
    CharacterIdentityCategory? Category = null)
{
    public static CharacterEditOperation Set(string path, SemanticValue value) => new(CharacterEditOperationKind.SetValue, path, value);
    public static CharacterEditOperation Adjust(string path, double delta) => new(CharacterEditOperationKind.AdjustNumber, path, SemanticValue.Number(delta));
    public static CharacterEditOperation Lock(string path) => new(CharacterEditOperationKind.LockProperty, path);
    public static CharacterEditOperation Unlock(string path) => new(CharacterEditOperationKind.UnlockProperty, path);
    public static CharacterEditOperation Lock(CharacterIdentityCategory category) => new(CharacterEditOperationKind.LockCategory, Category: category);
    public static CharacterEditOperation Unlock(CharacterIdentityCategory category) => new(CharacterEditOperationKind.UnlockCategory, Category: category);
}

public sealed record CharacterEditPlan
{
    public CharacterEditPlan(IEnumerable<CharacterEditOperation> operations, string? explanation = null)
        : this(Guid.NewGuid(), operations, explanation) { }

    public CharacterEditPlan(Guid transactionId, IEnumerable<CharacterEditOperation> operations, string? explanation = null)
    {
        if (transactionId == Guid.Empty) throw new ArgumentException("A transaction ID is required.", nameof(transactionId));
        TransactionId = transactionId;
        Operations = operations?.ToArray() ?? throw new ArgumentNullException(nameof(operations));
        if (Operations.Count == 0) throw new ArgumentException("At least one operation is required.", nameof(operations));
        Explanation = string.IsNullOrWhiteSpace(explanation) ? null : explanation.Trim();
    }

    public Guid TransactionId { get; }
    public IReadOnlyList<CharacterEditOperation> Operations { get; }
    public string? Explanation { get; }
}

public sealed record CharacterEditValidation(bool IsValid, bool LocksRespected, IReadOnlyList<string> Errors);
public sealed record CharacterEditResult(CharacterSpecification Specification, CharacterEditValidation Validation);
public sealed class CharacterLockException(string message) : InvalidOperationException(message);

public static class CharacterEditService
{
    public static CharacterEditResult Apply(CharacterSpecification source, CharacterEditPlan plan)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(plan);
        var candidate = source.Clone();
        var errors = new List<string>();
        var locksRespected = true;
        foreach (var operation in plan.Operations)
        {
            try { Apply(candidate, operation); }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or KeyNotFoundException or NotSupportedException)
            {
                errors.Add(exception.Message);
                if (exception is CharacterLockException) locksRespected = false;
            }
        }
        return new CharacterEditResult(errors.Count == 0 ? candidate : source.Clone(),
            new CharacterEditValidation(errors.Count == 0, locksRespected, errors));
    }

    private static void Apply(CharacterSpecification specification, CharacterEditOperation operation)
    {
        switch (operation.Kind)
        {
            case CharacterEditOperationKind.LockCategory:
            case CharacterEditOperationKind.UnlockCategory:
                specification.SetCategoryLock(operation.Category ?? throw new ArgumentException("A category is required."), operation.Kind == CharacterEditOperationKind.LockCategory);
                return;
            case CharacterEditOperationKind.LockProperty:
            case CharacterEditOperationKind.UnlockProperty:
                specification.SetPropertyLock(operation.Path ?? throw new ArgumentException("A path is required."), operation.Kind == CharacterEditOperationKind.LockProperty);
                return;
        }

        var path = operation.Path ?? throw new ArgumentException("A path is required.");
        if (specification.IsLocked(path)) throw new CharacterLockException($"Locked characteristic '{path}' was not changed.");
        var value = operation.Value ?? throw new ArgumentException("A value is required.");
        if (operation.Kind == CharacterEditOperationKind.AdjustNumber)
        {
            var current = specification.Value(path);
            if (current.Kind != SemanticValueKind.Number || value.Kind != SemanticValueKind.Number)
                throw new ArgumentException("Numeric adjustments require a numeric property and delta.");
            value = SemanticValue.Number(current.NumberValue + value.NumberValue);
        }
        specification.ApplyValidated(path, value);
    }
}

public sealed record CharacterEditTransaction(Guid TransactionId, string Label,
    CharacterSpecification Before, CharacterSpecification After);

public sealed class CharacterEditHistory
{
    private readonly Stack<CharacterEditTransaction> _undo = new();
    private readonly Stack<CharacterEditTransaction> _redo = new();

    public CharacterEditHistory(CharacterSpecification initial) => Current = initial?.Clone() ?? throw new ArgumentNullException(nameof(initial));
    public CharacterSpecification Current { get; private set; }
    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    public CharacterEditResult Apply(CharacterEditPlan plan)
    {
        var result = CharacterEditService.Apply(Current, plan);
        if (!result.Validation.IsValid) return result;
        var before = Current.Clone();
        Current = result.Specification.Clone();
        _undo.Push(new CharacterEditTransaction(plan.TransactionId, plan.Explanation?.Trim() ?? "Character edit", before, Current.Clone()));
        _redo.Clear();
        return result with { Specification = Current.Clone() };
    }

    public CharacterSpecification SetStage(GenesisStage stage, string label = "Change Genesis stage")
    {
        var before = Current.Clone();
        var after = Current.Clone();
        after.SetStage(stage);
        Current = after;
        _undo.Push(new CharacterEditTransaction(Guid.NewGuid(), label, before, after.Clone()));
        _redo.Clear();
        return Current.Clone();
    }

    public CharacterSpecification Undo()
    {
        if (_undo.TryPop(out var transaction))
        {
            _redo.Push(transaction);
            Current = transaction.Before.Clone();
        }
        return Current.Clone();
    }

    public CharacterSpecification Redo()
    {
        if (_redo.TryPop(out var transaction))
        {
            _undo.Push(transaction);
            Current = transaction.After.Clone();
        }
        return Current.Clone();
    }
}
