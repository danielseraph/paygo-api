using PayGo.Core.Enums;

namespace PayGo.Core.Models;

/// <summary>
/// Represents the result of a single fraud rule evaluation.
/// The fraud engine runs multiple rules and returns a list of these.
/// </summary>
public class FraudRuleResult
{
    public string RuleName { get; set; } = string.Empty;
    public bool IsTriggered { get; set; }
    public string? Reason { get; set; }
    public RiskLevel Severity { get; set; } = RiskLevel.Low;

    public FraudRuleResult() { }

    public FraudRuleResult(string ruleName, bool isTriggered, RiskLevel severity, string? reason = null)
    {
        RuleName = ruleName;
        IsTriggered = isTriggered;
        Severity = severity;
        Reason = reason;
    }

    public override string ToString() =>
        $"Rule: {RuleName} | Triggered: {IsTriggered} | Severity: {Severity}" +
        (Reason != null ? $" | Reason: {Reason}" : string.Empty);
}
