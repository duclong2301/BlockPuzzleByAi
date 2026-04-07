/// <summary>
/// Something that can be cleared to an empty state.
/// Implemented by CellView.
/// </summary>
public interface IClearable
{
    /// <summary>Reset this object to its empty/default visual state.</summary>
    void Clear();

    /// <summary>True when this object is in its empty state.</summary>
    bool IsEmpty { get; }
}
