// ReSharper disable UnusedType.Global

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace HamedStack.DataAnnotation;

public sealed class NotEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        return string.IsNullOrWhiteSpace(value as string)
            ? new ValidationResult("Value cannot be empty.")
            : ValidationResult.Success;
    }
}

public sealed class IsNumericAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        return decimal.TryParse(value?.ToString(), out _)
            ? ValidationResult.Success
            : new ValidationResult("Value must be numeric.");
    }
}

public sealed class IsBooleanAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        return bool.TryParse(value?.ToString(), out _)
            ? ValidationResult.Success
            : new ValidationResult("Value must be boolean.");
    }
}

public sealed class MaxValueAttribute : ValidationAttribute
{
    private decimal Max { get; }

    public MaxValueAttribute(decimal max)
    {
        Max = max;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        return value is decimal d && d <= Max
            ? ValidationResult.Success
            : new ValidationResult($"Value must be less than or equal to {Max}.");
    }
}

public sealed class MinValueAttribute : ValidationAttribute
{
    private decimal Min { get; }
    public MinValueAttribute(decimal min) => Min = min;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        return value is decimal d && d >= Min
            ? ValidationResult.Success
            : new ValidationResult($"Value must be greater than or equal to {Min}.");
    }
}

public sealed class StartsWithAttribute : ValidationAttribute
{
    private string Prefix { get; }
    public StartsWithAttribute(string prefix) => Prefix = prefix;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s.StartsWith(Prefix)
            ? ValidationResult.Success
            : new ValidationResult($"Value must start with {Prefix}.");
}

public sealed class EndsWithAttribute : ValidationAttribute
{
    private string Suffix { get; }
    public EndsWithAttribute(string suffix) => Suffix = suffix;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s.EndsWith(Suffix)
            ? ValidationResult.Success
            : new ValidationResult($"Value must end with {Suffix}.");
}

public sealed class ExactLengthAttribute : ValidationAttribute
{
    private int Length { get; }
    public ExactLengthAttribute(int length) => Length = length;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s.Length == Length
            ? ValidationResult.Success
            : new ValidationResult($"Value must be exactly {Length} characters long.");
}

public sealed class ContainsAttribute : ValidationAttribute
{
    private string Substring { get; }
    public ContainsAttribute(string substring) => Substring = substring;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s.Contains(Substring)
            ? ValidationResult.Success
            : new ValidationResult($"Value must contain {Substring}.");
}

public sealed class EmailAttribute : ValidationAttribute
{
    private readonly Regex _regex = new(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$");

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && _regex.IsMatch(s)
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid email address.");
}

public sealed class UrlAttribute : ValidationAttribute
{
    private readonly Regex _regex =
        new(@"^https?:\/\/[\w-]+(\.[\w-]+)+([\w.,@?^=%&amp;:\/~+#-]*[\w@?^=%&amp;\/~+#-])?$");

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && _regex.IsMatch(s)
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid URL.");
}

public sealed class AlphaOnlyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s, @"^[a-zA-Z]+$")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain only alphabetic characters.");
}

public sealed class AlphaNumericAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s, @"^[a-zA-Z0-9]+$")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain only alphanumeric characters.");
}

public sealed class NoSpecialCharactersAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s, @"^[a-zA-Z0-9\s]+$")
            ? ValidationResult.Success
            : new ValidationResult("Value must not contain special characters.");
}

public sealed class GuidAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Guid.TryParse(s, out _)
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid GUID.");
}

public sealed class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is DateTime dt && dt > DateTime.Now
            ? ValidationResult.Success
            : new ValidationResult("Value must be a future date.");
}

public sealed class PastDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is DateTime dt && dt < DateTime.Now
            ? ValidationResult.Success
            : new ValidationResult("Value must be a past date.");
}

public sealed class PositiveNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is decimal and > 0
            ? ValidationResult.Success
            : new ValidationResult("Value must be a positive number.");
}

public sealed class NegativeNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is decimal and < 0
            ? ValidationResult.Success
            : new ValidationResult("Value must be a negative number.");
}

public sealed class IsUpperCaseAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s == s.ToUpper()
            ? ValidationResult.Success
            : new ValidationResult("Value must be uppercase.");
}

public sealed class IsLowerCaseAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s == s.ToLower()
            ? ValidationResult.Success
            : new ValidationResult("Value must be lowercase.");
}

public sealed class IsWeekendAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is DateTime dt && (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
            ? ValidationResult.Success
            : new ValidationResult("Date must fall on a weekend.");
}

public sealed class IsWeekdayAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is DateTime dt && (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
            ? ValidationResult.Success
            : new ValidationResult("Date must fall on a weekday.");
}

public sealed class ContainsNoWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && !s.Contains(" ")
            ? ValidationResult.Success
            : new ValidationResult("Value must not contain whitespace.");
}

public sealed class IsOddAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num % 2 != 0
            ? ValidationResult.Success
            : new ValidationResult("Value must be an odd number.");
}

public sealed class IsEvenAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num % 2 == 0
            ? ValidationResult.Success
            : new ValidationResult("Value must be an even number.");
}

public sealed class PrimeNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is int num and > 1)
        {
            for (int i = 2; i <= Math.Sqrt(num); ++i)
            {
                if (num % i == 0) return new ValidationResult("Value must be a prime number.");
            }

            return ValidationResult.Success;
        }

        return new ValidationResult("Value must be an integer greater than 1.");
    }
}

public sealed class IsPercentageAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double and >= 0 and <= 100
            ? ValidationResult.Success
            : new ValidationResult("Value must be a percentage between 0 and 100.");
}

public sealed class IsBase64Attribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is string s)
        {
            try
            {
                _ = Convert.FromBase64String(s);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("Value must be a valid Base64 string.");
            }
        }

        return new ValidationResult("Value must be a string.");
    }
}

public sealed class ExactDigitsAttribute : ValidationAttribute
{
    private int Digits { get; }
    public ExactDigitsAttribute(int digits) => Digits = digits;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num.ToString().Length == Digits
            ? ValidationResult.Success
            : new ValidationResult($"Value must have exactly {Digits} digits.");
}

public sealed class YearRangeAttribute : ValidationAttribute
{
    private int StartYear { get; }
    private int EndYear { get; }

    public YearRangeAttribute(int startYear, int endYear)
    {
        StartYear = startYear;
        EndYear = endYear;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int year && year >= StartYear && year <= EndYear
            ? ValidationResult.Success
            : new ValidationResult($"Year must be between {StartYear} and {EndYear}.");
}

public sealed class ContainsCharacterAttribute : ValidationAttribute
{
    private char Character { get; }
    public ContainsCharacterAttribute(char character) => Character = character;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && s.Contains(Character)
            ? ValidationResult.Success
            : new ValidationResult($"Value must contain the character '{Character}'.");
}

public sealed class StartsWithLetterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && char.IsLetter(s[0])
            ? ValidationResult.Success
            : new ValidationResult("Value must start with a letter.");
}

public sealed class EndsWithNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && char.IsNumber(s[^1])
            ? ValidationResult.Success
            : new ValidationResult("Value must end with a number.");
}

public sealed class IsIPv4AddressAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid IPv4 address.");
}

public sealed class IsIPv6AddressAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s,
            @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]|[1-9]|)[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]|[1-9]|)[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]|[1-9]|)[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]|[1-9]|)[0-9]))")
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid IPv6 address.");
}

public sealed class IsHexColorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string s && Regex.IsMatch(s, @"^#(?:[0-9a-fA-F]{3}){1,2}$")
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid hex color.");
}

public sealed class IsLatitudeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is >= -90.0 and <= 90.0
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid latitude.");
}

public sealed class IsLongitudeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is >= -180.0 and <= 180.0
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid longitude.");
}

public sealed class MinimumLengthAttribute : ValidationAttribute
{
    private int MinLength { get; }
    public MinimumLengthAttribute(int minLength) => MinLength = minLength;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => ((string?) value)?.Length >= MinLength
            ? ValidationResult.Success
            : new ValidationResult($"Value must have at least {MinLength} characters.");
}

public sealed class MaximumLengthAttribute : ValidationAttribute
{
    private int MaxLength { get; }
    public MaximumLengthAttribute(int maxLength) => MaxLength = maxLength;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => ((string?) value)?.Length <= MaxLength
            ? ValidationResult.Success
            : new ValidationResult($"Value must have no more than {MaxLength} characters.");
}

public sealed class IsDecimalAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => decimal.TryParse((string?) value, out _)
            ? ValidationResult.Success
            : new ValidationResult("Value must be a valid decimal.");
}

public sealed class IsNotWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => string.IsNullOrWhiteSpace((string?) value)
            ? new ValidationResult("Value must not be whitespace.")
            : ValidationResult.Success;
}

public sealed class IsEnumTypeAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public IsEnumTypeAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enumeration", nameof(enumType));
        }

        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return Enum.IsDefined(_enumType, value)
            ? ValidationResult.Success
            : new ValidationResult($"Value must be of Enum Type {_enumType.Name}");
    }
}

public sealed class IsMultipleOfAttribute : ValidationAttribute
{
    private readonly int _multipleOf;
    public IsMultipleOfAttribute(int multipleOf) => _multipleOf = multipleOf;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num % _multipleOf == 0
            ? ValidationResult.Success
            : new ValidationResult($"Value must be a multiple of {_multipleOf}.");
}

public sealed class IsPalindromeAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        string str = value as string ?? "";
        return str == new string(str.Reverse().ToArray())
            ? ValidationResult.Success
            : new ValidationResult("Value must be a palindrome.");
    }
}

public sealed class IsBinaryAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string str && str.All(c => c == '0' || c == '1')
            ? ValidationResult.Success
            : new ValidationResult("Value must be a binary string.");
}

public sealed class IsAdultAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is DateTime date)
        {
            int age = DateTime.Now.Year - date.Year;
            return age >= 18 ? ValidationResult.Success : new ValidationResult("Value must indicate an adult age.");
        }

        return new ValidationResult("Value must be a DateTime object.");
    }
}

public sealed class IsChildAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is DateTime date)
        {
            int age = DateTime.Now.Year - date.Year;
            return age < 18 ? ValidationResult.Success : new ValidationResult("Value must indicate a child's age.");
        }

        return new ValidationResult("Value must be a DateTime object.");
    }
}

public sealed class IsEqualOrGreaterThanAttribute : ValidationAttribute
{
    private readonly double _minValue;
    public IsEqualOrGreaterThanAttribute(double minValue) => _minValue = minValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double num && num >= _minValue
            ? ValidationResult.Success
            : new ValidationResult($"Value must be equal or greater than {_minValue}.");
}

public sealed class IsEqualOrLessThanAttribute : ValidationAttribute
{
    private readonly double _maxValue;
    public IsEqualOrLessThanAttribute(double maxValue) => _maxValue = maxValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double num && num <= _maxValue
            ? ValidationResult.Success
            : new ValidationResult($"Value must be equal or less than {_maxValue}.");
}

public sealed class IsSquareRootableAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double and >= 0
            ? ValidationResult.Success
            : new ValidationResult("Value must be square-rootable (non-negative).");
}

public sealed class IsDivisibleByAttribute : ValidationAttribute
{
    private readonly int _divider;
    public IsDivisibleByAttribute(int divider) => _divider = divider;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num % _divider == 0
            ? ValidationResult.Success
            : new ValidationResult($"Value must be divisible by {_divider}.");
}

public sealed class IsFibonacciNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is int num)
        {
            int a = 0, b = 1, c = 0;
            while (c < num)
            {
                c = a + b;
                a = b;
                b = c;
            }

            return c == num ? ValidationResult.Success : new ValidationResult("Value must be a Fibonacci number.");
        }

        return new ValidationResult("Value must be an integer.");
    }
}

public sealed class IsGreaterThanAttribute : ValidationAttribute
{
    private readonly double _minValue;
    public IsGreaterThanAttribute(double minValue) => _minValue = minValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double num && num > _minValue
            ? ValidationResult.Success
            : new ValidationResult($"Value must be greater than {_minValue}.");
}

public sealed class IsLessThanAttribute : ValidationAttribute
{
    private readonly double _maxValue;
    public IsLessThanAttribute(double maxValue) => _maxValue = maxValue;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is double num && num < _maxValue
            ? ValidationResult.Success
            : new ValidationResult($"Value must be less than {_maxValue}.");
}

public sealed class IsLowercaseAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string str && str.All(char.IsLower)
            ? ValidationResult.Success
            : new ValidationResult("Value must be a lowercase string.");
}

public sealed class IsUppercaseAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string str && str.All(char.IsUpper)
            ? ValidationResult.Success
            : new ValidationResult("Value must be an uppercase string.");
}

public sealed class IsLetterOrDigitAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is string str && str.All(char.IsLetterOrDigit)
            ? ValidationResult.Success
            : new ValidationResult("Value must contain only letters or digits.");
}

public sealed class HasVowelAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => Regex.IsMatch(value as string ?? "", "[aeiouAEIOU]")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain at least one vowel.");
}

public sealed class HasConsonantAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => Regex.IsMatch(value as string ?? "", "[bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ]")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain at least one consonant.");
}

public sealed class IsNotZeroAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is int num && num != 0 ? ValidationResult.Success : new ValidationResult("Value must not be zero.");
}

public sealed class IsUpperCaseFirstLetterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        var str = value as string;
        return !string.IsNullOrEmpty(str) && char.IsUpper(str[0])
            ? ValidationResult.Success
            : new ValidationResult("First letter must be uppercase.");
    }
}

public sealed class IsLowerCaseFirstLetterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        var str = value as string;
        return !string.IsNullOrEmpty(str) && char.IsLower(str[0])
            ? ValidationResult.Success
            : new ValidationResult("First letter must be lowercase.");
    }
}

public sealed class ContainsNumberAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => Regex.IsMatch(value as string ?? "", @"\d")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain at least one number.");
}

public sealed class ContainsSpecialCharacterAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => Regex.IsMatch(value as string ?? "", @"[\W_]+")
            ? ValidationResult.Success
            : new ValidationResult("Value must contain at least one special character.");
}

public sealed class DoesNotContainWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => !Regex.IsMatch(value as string ?? "", @"\s")
            ? ValidationResult.Success
            : new ValidationResult("Value must not contain whitespace.");
}

public sealed class IsNotNullOrEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => !string.IsNullOrEmpty(value as string)
            ? ValidationResult.Success
            : new ValidationResult("Value must not be null or empty.");
}

public sealed class IsAlphanumericAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => Regex.IsMatch(value as string ?? "", @"^[a-zA-Z0-9]+$")
            ? ValidationResult.Success
            : new ValidationResult("Value must be alphanumeric.");
}

public sealed class IsJsonStringAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        try
        {
            System.Text.Json.JsonDocument.Parse(value as string ?? "");
            return ValidationResult.Success;
        }
        catch
        {
            return new ValidationResult("Value must be a valid JSON string.");
        }
    }
}

public sealed class IsSameAsAttribute : ValidationAttribute
{
    private readonly string _otherProperty;
    public IsSameAsAttribute(string otherProperty) => _otherProperty = otherProperty;

    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        var instance = validationContext?.ObjectInstance;
        var otherValue = instance?.GetType().GetProperty(_otherProperty)?.GetValue(instance, null);
        return value != null && value.Equals(otherValue)
            ? ValidationResult.Success
            : new ValidationResult("Values must match.");
    }
}

public sealed class IsTrueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is true ? ValidationResult.Success : new ValidationResult("Value must be true.");
}

public sealed class IsFalseAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => value is false ? ValidationResult.Success : new ValidationResult("Value must be false.");
}