namespace PayGo.Core.Models;

/// <summary>
/// A structured error detail that carries a machine-readable code
/// and a human-readable description.
/// Used inside ApiResponse.Errors to give callers actionable feedback.
/// </summary>
public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Field { get; set; } // nullable — only set on validation errors

    public ErrorDetail() { }

    public ErrorDetail(string code, string description, string? field = null)
    {
        Code = code;
        Description = description;
        Field = field;
    }

    public override string ToString() => string.IsNullOrEmpty(Field)
        ? $"[{Code}] {Description}"
        : $"[{Code}] {Field}: {Description}";
}
