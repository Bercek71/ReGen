using System.Globalization;
using System.Windows.Controls;

namespace ReGen.Validation;

public class IntValidationRule : ValidationRule
{
    public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            return new ValidationResult(false, "Value cannot be empty");

        return int.TryParse(value.ToString(), out _)
            ? ValidationResult.ValidResult
            : new ValidationResult(false, "Only integers are allowed");
    }
}