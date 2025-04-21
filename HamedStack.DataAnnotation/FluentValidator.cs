using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace HamedStack.DataAnnotation;

/// <summary>
/// Interface for fluent validation operations on a specific type.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public interface IFluentValidator<in T> where T : class
{
    /// <summary>
    /// Validates the specified instance and returns all validation errors.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>A collection of validation results containing any errors.</returns>
    ICollection<ValidationResult> Validate(T instance);

    /// <summary>
    /// Determines whether the specified instance is valid.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    bool IsValid(T instance);

    /// <summary>
    /// Validates the specified instance and outputs all validation errors.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="validationResults">When this method returns, contains the validation results.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    bool Validate(T instance, out ICollection<ValidationResult> validationResults);
}

/// <summary>
/// Base class for implementing fluent validators for a specific type.
/// </summary>
/// <typeparam name="T">The type of object to validate.</typeparam>
public abstract class FluentValidator<T> : IFluentValidator<T> where T : class
{
    private readonly List<IPropertyValidator<T>> _validators = new();

    /// <summary>
    /// Helper method to define validation rules for a property in the constructor.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="expression">Expression pointing to the property to validate.</param>
    /// <returns>A property rule for chaining validation rules.</returns>
    protected PropertyRule<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var propertyRule = new PropertyRule<T, TProperty>(expression);
        _validators.Add(propertyRule);
        return propertyRule;
    }

    /// <summary>
    /// Validates the specified instance and returns all validation errors.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>A collection of validation results containing any errors.</returns>
    public ICollection<ValidationResult> Validate(T instance)
    {
        var errors = new List<ValidationResult>();

        foreach (var validator in _validators)
        {
            var results = validator.Validate(instance);
            errors.AddRange(results);
        }

        return errors;
    }

    /// <summary>
    /// Determines whether the specified instance is valid.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    public bool IsValid(T instance)
    {
        return Validate(instance).Count == 0;
    }

    /// <summary>
    /// Validates the specified instance and outputs all validation errors.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <param name="validationResults">When this method returns, contains the validation results.</param>
    /// <returns>True if the instance is valid; otherwise, false.</returns>
    public bool Validate(T instance, out ICollection<ValidationResult> validationResults)
    {
        validationResults = Validate(instance);
        return validationResults.Count == 0;
    }
}

/// <summary>
/// Interface for property validators that can validate a specific property of an object.
/// </summary>
/// <typeparam name="T">The type of object containing the property to validate.</typeparam>
public interface IPropertyValidator<in T> where T : class
{
    /// <summary>
    /// Validates a property on the specified instance.
    /// </summary>
    /// <param name="instance">The instance containing the property to validate.</param>
    /// <returns>A collection of validation results containing any errors.</returns>
    ICollection<ValidationResult> Validate(T instance);
}

/// <summary>
/// Represents a rule for validating a property of an object.
/// </summary>
/// <typeparam name="T">The type of object containing the property.</typeparam>
/// <typeparam name="TProperty">The type of the property being validated.</typeparam>
public class PropertyRule<T, TProperty> : IPropertyValidator<T> where T : class
{
    private readonly Expression<Func<T, TProperty>> _propertyExpression;
    private readonly List<ValidationRule> _validationRules = new();
    private readonly string _propertyName;

    /// <summary>
    /// Initializes a new instance of the PropertyRule class.
    /// </summary>
    /// <param name="propertyExpression">Expression pointing to the property to validate.</param>
    public PropertyRule(Expression<Func<T, TProperty>> propertyExpression)
    {
        _propertyExpression = propertyExpression;
        _propertyName = GetPropertyName(propertyExpression);
    }

    /// <summary>
    /// Extracts the property name from the property expression.
    /// </summary>
    /// <param name="expression">Expression pointing to the property.</param>
    /// <returns>The name of the property.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a member expression.</exception>
    private string GetPropertyName(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expression must be a member expression");
    }

    /// <summary>
    /// Gets the name of the property being validated.
    /// </summary>
    /// <returns>The name of the property.</returns>
    public string GetPropertyName()
    {
        return _propertyName;
    }

    /// <summary>
    /// Specifies that the property is required and cannot be null.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> IsRequired(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new RequiredAttribute { ErrorMessage = errorMessage ?? $"{_propertyName} is required." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies the maximum length allowed for a string property.
    /// </summary>
    /// <param name="length">The maximum allowed length.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> MaxLength(int length, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new MaxLengthAttribute(length) { ErrorMessage = errorMessage ?? $"{_propertyName} cannot exceed {length} characters." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies the minimum length required for a string property.
    /// </summary>
    /// <param name="length">The minimum required length.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> MinLength(int length, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new MinLengthAttribute(length) { ErrorMessage = errorMessage ?? $"{_propertyName} must be at least {length} characters." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must match a regular expression pattern.
    /// </summary>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Matches(string pattern, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new RegularExpressionAttribute(pattern) { ErrorMessage = errorMessage ?? $"{_propertyName} has an invalid format." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies a numeric range within which the property value must fall.
    /// </summary>
    /// <param name="minimum">The minimum allowed value.</param>
    /// <param name="maximum">The maximum allowed value.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Range(double minimum, double maximum, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new RangeAttribute(minimum, maximum) { ErrorMessage = errorMessage ?? $"{_propertyName} must be between {minimum} and {maximum}." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property cannot be empty.
    /// Works for strings, collections, and other types.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> NotEmpty(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value == null) return new ValidationResult(errorMessage ?? $"{_propertyName} cannot be empty.");
                if (value is string str && string.IsNullOrWhiteSpace(str)) return new ValidationResult(errorMessage ?? $"{_propertyName} cannot be empty.");
                if (value is ICollection { Count: 0 }) return new ValidationResult(errorMessage ?? $"{_propertyName} cannot be empty.");
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the comparable property value must be within the specified inclusive range.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="from">The minimum allowed value (inclusive).</param>
    /// <param name="to">The maximum allowed value (inclusive).</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> InclusiveBetween<TComparable>(TComparable from, TComparable to, string? errorMessage = null) where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value is TComparable comparable)
                {
                    if (comparable.CompareTo(from) < 0 || comparable.CompareTo(to) > 0)
                    {
                        return new ValidationResult(errorMessage ?? $"{_propertyName} must be between {from} and {to}.");
                    }
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must equal the expected value.
    /// </summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Equal(TProperty expected, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (!Equals(value, expected))
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be equal to {expected}.");
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must not equal the given value.
    /// </summary>
    /// <param name="notExpected">The value that is not expected.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> NotEqual(TProperty notExpected, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (Equals(value, notExpected))
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must not be equal to {notExpected}.");
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies the length range for a string property.
    /// </summary>
    /// <param name="min">The minimum allowed length.</param>
    /// <param name="max">The maximum allowed length.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Length(int min, int max, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new StringLengthAttribute(max)
            {
                MinimumLength = min,
                ErrorMessage = errorMessage ?? $"{_propertyName} must be between {min} and {max} characters."
            }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be a valid credit card number.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> CreditCard(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CreditCardAttribute { ErrorMessage = errorMessage ?? $"{_propertyName} must be a valid credit card number." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be a valid phone number.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Phone(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new PhoneAttribute { ErrorMessage = errorMessage ?? $"{_propertyName} must be a valid phone number." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value cannot be null.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> NotNull(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new RequiredAttribute { ErrorMessage = errorMessage ?? $"{_propertyName} cannot be null." }
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be less than the specified value.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="value">The value that the property must be less than.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> LessThan<TComparable>(TComparable value, string? errorMessage = null) where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((actual, _) =>
            {
                if (actual is TComparable comparable && comparable.CompareTo(value) >= 0)
                {
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be less than {value}.");
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be less than or equal to the specified value.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="value">The value that the property must be less than or equal to.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> LessThanOrEqual<TComparable>(TComparable value, string? errorMessage = null) where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((actual, _) =>
            {
                if (actual is TComparable comparable && comparable.CompareTo(value) > 0)
                {
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be less than or equal to {value}.");
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be greater than the specified value.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="value">The value that the property must be greater than.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> GreaterThan<TComparable>(TComparable value, string? errorMessage = null) where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((actual, _) =>
            {
                if (actual is TComparable comparable && comparable.CompareTo(value) <= 0)
                {
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be greater than {value}.");
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be greater than or equal to the specified value.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="value">The value that the property must be greater than or equal to.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> GreaterThanOrEqual<TComparable>(TComparable value, string? errorMessage = null) where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((actual, _) =>
            {
                if (actual is TComparable comparable && comparable.CompareTo(value) < 0)
                {
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be greater than or equal to {value}.");
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be a valid enum value.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> IsEnum(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value == null || !Enum.IsDefined(typeof(TProperty), value))
                {
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be a valid enum value.");
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be a valid enum name.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="ignoreCase">Whether to ignore case when comparing enum names.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> IsEnumName<TEnum>(bool ignoreCase = false, string? errorMessage = null) where TEnum : struct, Enum
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value is string name)
                {
                    if (Enum.TryParse<TEnum>(name, ignoreCase, out var _))
                    {
                        return ValidationResult.Success;
                    }
                }
                return new ValidationResult(errorMessage ?? $"{_propertyName} must be a valid name of enum {typeof(TEnum).Name}.");
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies precision and scale constraints for decimal values.
    /// </summary>
    /// <param name="precision">The maximum total number of digits.</param>
    /// <param name="scale">The maximum number of decimal places.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> PrecisionScale(int precision, int scale, string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value == null) return ValidationResult.Success;

                if (value is decimal decimalValue)
                {
                    var strValue = decimalValue.ToString(CultureInfo.InvariantCulture);
                    var parts = strValue.Split('.');
                    var intPart = parts[0].Replace("-", "");
                    var fractionPart = parts.Length > 1 ? parts[1] : "";

                    var totalDigits = intPart.Length + fractionPart.Length;
                    var fractionDigits = fractionPart.Length;

                    if (totalDigits > precision || fractionDigits > scale)
                    {
                        return new ValidationResult(errorMessage ?? $"{_propertyName} must have precision {precision} and scale {scale}.");
                    }
                }

                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be empty.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Empty(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value is string str && !string.IsNullOrWhiteSpace(str))
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be empty.");
                if (value is ICollection { Count: > 0 })
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be empty.");
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be null.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Null(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value != null)
                    return new ValidationResult(errorMessage ?? $"{_propertyName} must be null.");
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be exclusively between the specified values.
    /// </summary>
    /// <typeparam name="TComparable">The comparable type.</typeparam>
    /// <param name="from">The exclusive minimum value.</param>
    /// <param name="to">The exclusive maximum value.</param>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> ExclusiveBetween<TComparable>(TComparable from, TComparable to, string? errorMessage = null)
        where TComparable : IComparable
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value is TComparable comparable)
                {
                    if (comparable.CompareTo(from) <= 0 || comparable.CompareTo(to) >= 0)
                    {
                        return new ValidationResult(errorMessage ?? $"{_propertyName} must be exclusively between {from} and {to}.");
                    }
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies that the property value must be a valid email address.
    /// </summary>
    /// <param name="errorMessage">Optional custom error message.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Email(string? errorMessage = null)
    {
        _validationRules.Add(new ValidationRule(
            new EmailAddressAttribute { ErrorMessage = errorMessage ?? $"{_propertyName} must be a valid email address." }
        ));
        return this;
    }

    /// <summary>
    /// Adds a custom validation attribute to the property rule.
    /// </summary>
    /// <param name="validationAttribute">The validation attribute to add.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> WithCustomValidation(ValidationAttribute validationAttribute)
    {
        _validationRules.Add(new ValidationRule(validationAttribute));
        return this;
    }

    /// <summary>
    /// Specifies a custom validation condition that the property value must satisfy.
    /// </summary>
    /// <param name="predicate">The predicate that must be satisfied.</param>
    /// <param name="errorMessage">The error message to display if validation fails.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Must(Func<TProperty, bool> predicate, string errorMessage)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, _) =>
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                if (!predicate((TProperty)value))
                {
                    return new ValidationResult(errorMessage, [_propertyName]);
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Specifies a custom validation condition that the property value must satisfy,
    /// with access to the complete object being validated.
    /// </summary>
    /// <param name="predicate">The predicate that must be satisfied.</param>
    /// <param name="errorMessage">The error message to display if validation fails.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Must(Func<T, TProperty, bool> predicate, string errorMessage)
    {
        _validationRules.Add(new ValidationRule(
            new CustomValidationAttribute((value, context) =>
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                var instance = (T)context.ObjectInstance;
                if (!predicate(instance, (TProperty)value))
                {
                    return new ValidationResult(errorMessage, [_propertyName]);
                }
                return ValidationResult.Success;
            })
        ));
        return this;
    }

    /// <summary>
    /// Applies validation rules conditionally based on a condition.
    /// </summary>
    /// <param name="condition">The condition that must be true for the validation rules to apply.</param>
    /// <param name="action">Action that configures the conditional validation rules.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> When(Func<T, bool> condition, Action<PropertyRule<T, TProperty>> action)
    {
        var conditionalRule = new PropertyRule<T, TProperty>(_propertyExpression);
        action(conditionalRule);

        foreach (var rule in conditionalRule._validationRules)
        {
            _validationRules.Add(new ValidationRule(
                rule.Attribute,
                condition
            ));
        }

        return this;
    }

    /// <summary>
    /// Applies validation rules unless a condition is met.
    /// </summary>
    /// <param name="condition">The condition that must be false for the validation rules to apply.</param>
    /// <param name="action">Action that configures the conditional validation rules.</param>
    /// <returns>The property rule for chaining validation rules.</returns>
    public PropertyRule<T, TProperty> Unless(Func<T, bool> condition, Action<PropertyRule<T, TProperty>> action)
    {
        return When(instance => !condition(instance), action);
    }

    /// <summary>
    /// Validates the specified instance according to all the defined rules.
    /// </summary>
    /// <param name="instance">The instance to validate.</param>
    /// <returns>A collection of validation results containing any errors.</returns>
    public ICollection<ValidationResult> Validate(T instance)
    {
        var errors = new List<ValidationResult>();
        var propertyFunc = _propertyExpression.Compile();
        var value = propertyFunc(instance);

        foreach (var rule in _validationRules)
        {
            // Check if the condition applies
            if (rule.Condition != null && !rule.Condition(instance))
            {
                continue;
            }

            var validationContext = new ValidationContext(instance)
            {
                MemberName = _propertyName
            };

            var validationResults = new List<ValidationResult>();

            if (!System.ComponentModel.DataAnnotations.Validator.TryValidateValue(value, validationContext, validationResults, [rule.Attribute]))
            {
                errors.AddRange(validationResults);
            }
        }

        return errors;
    }

    /// <summary>
    /// Inner class to hold validation attribute and condition.
    /// </summary>
    private class ValidationRule
    {
        /// <summary>
        /// Gets the validation attribute for this rule.
        /// </summary>
        public ValidationAttribute Attribute { get; }

        /// <summary>
        /// Gets the condition that determines if this rule should be applied.
        /// </summary>
        public Func<T, bool>? Condition { get; }

        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="attribute">The validation attribute for this rule.</param>
        /// <param name="condition">Optional condition that determines if this rule should be applied.</param>
        public ValidationRule(ValidationAttribute attribute, Func<T, bool>? condition = null)
        {
            Attribute = attribute;
            Condition = condition;
        }
    }
}

/// <summary>
/// Custom validation attribute that allows for dynamic validation logic.
/// </summary>
public class CustomValidationAttribute : ValidationAttribute
{
    private readonly Func<object?, ValidationContext, ValidationResult?> _validator;

    /// <summary>
    /// Initializes a new instance of the CustomValidationAttribute class.
    /// </summary>
    /// <param name="validator">A function that performs the validation logic.</param>
    public CustomValidationAttribute(Func<object?, ValidationContext, ValidationResult?> validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// Validates the specified value with the validation context.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>A ValidationResult if validation fails; otherwise, null.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return _validator(value, validationContext);
    }
}

/// <summary>
/// Extension methods for registering fluent validators with the dependency injection service collection.
/// </summary>
public static class FluentValidatorServiceCollection
{
    /// <summary>
    /// Adds all fluent validators from the specified assemblies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add validators to.</param>
    /// <param name="assemblies">The assemblies to scan for validators. If none are specified, all loaded assemblies are scanned.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddFluentValidator(this IServiceCollection services, params Assembly[] assemblies)
    {
        var assembliesToScan = assemblies.Length > 0
            ? assemblies
            : AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assembliesToScan)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false } && IsValidator(t))
                .ToList();

            foreach (var validatorType in validatorTypes)
            {
                // Find what type this validator validates
                var entityType = GetEntityType(validatorType);
                if (entityType != null)
                {
                    // Create the generic interface type for this validator
                    var interfaceType = typeof(IFluentValidator<>).MakeGenericType(entityType);

                    // Register the validator with DI
                    services.AddScoped(interfaceType, validatorType);
                }
            }
        }

        return services;
    }

    /// <summary>
    /// Determines whether a type implements the IFluentValidator interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type implements IFluentValidator; otherwise, false.</returns>
    private static bool IsValidator(Type type)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFluentValidator<>));
    }

    /// <summary>
    /// Gets the entity type that a validator validates.
    /// </summary>
    /// <param name="validatorType">The validator type.</param>
    /// <returns>The entity type, or null if not found.</returns>
    private static Type? GetEntityType(Type validatorType)
    {
        var interfaceType = validatorType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFluentValidator<>));

        return interfaceType?.GetGenericArguments().FirstOrDefault();
    }
}