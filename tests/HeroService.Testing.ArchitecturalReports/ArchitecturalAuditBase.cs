using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace HeroService.Testing.ArchitecturalReports
{
  /// <summary>
  /// HeroService Architectural Compliance Engine (Base)
  /// -------------------------------------------------
  /// Standardized execution and forensic reporting framework.
  /// Optimized for ArchUnitNET's evaluation patterns.
  /// </summary>
  public abstract class ArchitecturalAuditBase : BaseArchitectureTest
  {
    /// <summary>
    /// Executes an architectural tribunal (audit session) and provides
    /// formatted output with severity classification and compliance summary.
    /// </summary>
    protected static void ExecuteTribunal(string tribunalName, Action<StringBuilder, Action> run)
    {
      Console.OutputEncoding = Encoding.UTF8;
      var sb = new StringBuilder();
      int violationCount = 0;
      Action markViolation = () => violationCount++;

      Console.WriteLine();
      Console.WriteLine("===============================================================");
      Console.WriteLine($" ARCHITECTURE COMPLIANCE AUDIT — {tribunalName.ToUpper()}");
      Console.WriteLine("===============================================================");

      try
      {
        run(sb, markViolation);
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] Unhandled exception during tribunal execution: {ex.Message}");
      }

      Console.WriteLine(sb.ToString());

      if (violationCount > 0)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine($" RESULT: NON-COMPLIANT — {violationCount} rule violation(s) detected.");
        Console.WriteLine(" ACTION: Review the 'VIOLATION DETAILS' and align code with HeroService standards.");
        Console.WriteLine("---------------------------------------------------------------");
        Console.ResetColor();

        Assert.Fail($"{tribunalName} detected {violationCount} architectural violation(s).");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine(" RESULT: COMPLIANT — No architectural violations detected.");
        Console.WriteLine(" STATUS: System conforms to defined architecture guidelines.");
        Console.WriteLine("---------------------------------------------------------------");
        Console.ResetColor();
      }

      Console.WriteLine();
    }

    /// <summary>
    /// OVERLOAD: Executes an ArchUnitNET rule and performs forensic analysis on failure.
    /// Handles the IEnumerable evaluation results directly to list offenders.
    /// </summary>
    protected static void ExecuteRule(
    string context,
    string summary,
    IArchRule rule,
    StringBuilder sb,
    Action markViolation)
    {
      try
      {
        rule.Check(BaseArchitecture);
        sb.AppendLine($"[PASS] {context} — {summary}");
      }
      catch (FailedArchRuleException ex)
      {
        markViolation();
        sb.AppendLine($"[FAIL] {context} — {summary}");

        sb.AppendLine("      ? VIOLATION DETAILS:");

        // If .Violations or .Details don't exist, we fallback to the Exception message,
        // which ArchUnitNET formats as a comprehensive list of all violations.
        var lines = ex.Message.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines.Take(10)) // Take the first 10 to avoid log bloat
        {
          sb.AppendLine($"         ??  {line.Trim()}");
        }

        if (lines.Length > 10)
        {
          sb.AppendLine("         ??  ... (and more)");
        }
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] {context} — Unexpected error: {ex.Message}");
      }
    }
    /// <summary>
    /// OVERLOAD: Executes a manual assertion lambda.
    /// Useful for checking assembly presence or non-ArchUnit constraints.
    /// </summary>
    protected static void ExecuteRule(
        string context,
        string summary,
        Action manualAssertion,
        StringBuilder sb,
        Action markViolation)
    {
      try
      {
        manualAssertion();
        sb.AppendLine($"[PASS] {context} — {summary}");
      }
      catch (XunitException ex)
      {
        markViolation();
        sb.AppendLine($"[FAIL] {context} — {summary} (Assertion failure: {ex.Message})");
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] {context} — Unexpected error: {ex.Message}");
      }
    }
  }
}
