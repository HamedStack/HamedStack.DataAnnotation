# HamedStack.DataAnnotation User Guide

This comprehensive guide details how to use the HamedStack.DataAnnotation library for validation in .NET applications. The library provides two main validation approaches: traditional data annotation validation and a fluent validation API.

## Basic Validation

### Overview

The basic validation functionality uses standard .NET `DataAnnotations` under the hood but provides a more convenient API through the `IValidator` and `IValidator<T>` interfaces.

### Setup

First, register the validator services in your application's dependency injection container:

```csharp
// In Program.cs or Startup.cs
services.AddValidator();
```

### Usage

There are two ways to use the basic validator:

```csharp
public class User
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Range(18, 120)]
    public int Age { get; set; }
}
```

#### 1. Using the non-generic `IValidator`

```csharp
public class UserService
{
    private readonly IValidator _validator;
    
    public UserService(IValidator validator)
    {
        _validator = validator;
    }
    
    public void CreateUser(User user)
    {
        if (_validator.IsValid(user))
        {
            // Proceed with user creation
        }
        else
        {
            var errors = _validator.Validate(user);
            // Handle validation errors
        }
    }
}
```

#### 2. Using the generic `IValidator<T>`

```csharp
public class ProductService
{
    private readonly IValidator<Product> _validator;
    
    public ProductService(IValidator<Product> validator)
    {
        _validator = validator;
    }
    
    public void AddProduct(Product product)
    {
        if (_validator.IsValid(product))
        {
            // Proceed with adding product
        }
        else
        {
            var errors = _validator.Validate(product);
            // Handle validation errors
        }
    }
}
```

## Fluent Validation

### Overview

The fluent validation API offers a more expressive way to define validation rules for your models without using data annotation attributes.

### Creating a Validator

To create a validator for your model:

1. Create a class that inherits from `FluentValidator<T>`
2. Override or implement the constructor to define validation rules using the `RuleFor` method

```csharp
public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public string Password { get; set; }
}

public class UserValidator : FluentValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty("Username cannot be empty")
            .Length(3, 20, "Username must be between 3 and 20 characters");
            
        RuleFor(u => u.Email)
            .NotEmpty()
            .Email("Please provide a valid email address");
            
        RuleFor(u => u.Age)
            .GreaterThanOrEqual(18, "Users must be at least 18 years old");
            
        RuleFor(u => u.Password)
            .NotEmpty()
            .MinLength(8, "Password must be at least 8 characters long")
            .Matches(@"^(?=.*[A-Z])(?=.*\d)(?=.*[^\w\d\s]).*$", 
                "Password must contain at least one uppercase letter, one number, and one special character");
    }
}
```

### Registering Fluent Validators

Register all fluent validators in your application by calling:

```csharp
// Register validators from specific assemblies
services.AddFluentValidator(typeof(UserValidator).Assembly);

// Or register validators from all loaded assemblies
services.AddFluentValidator();
```

### Using Fluent Validators

```csharp
public class UserRegistrationService
{
    private readonly IFluentValidator<User> _validator;
    
    public UserRegistrationService(IFluentValidator<User> validator)
    {
        _validator = validator;
    }
    
    public void RegisterUser(User user)
    {
        if (_validator.IsValid(user))
        {
            // Register the user
        }
        else
        {
            var validationResults = _validator.Validate(user);
            // Handle validation errors
        }
    }
}
```

## Available Validation Rules

Here are the key validation rules you can use with `RuleFor()`:

| Rule | Description |
|------|-------------|
| `IsRequired` | Validates the property is not null |
| `NotEmpty` | Validates the property is not null, empty string, or empty collection |
| `MaxLength` | Validates string or collection does not exceed max length |
| `MinLength` | Validates string or collection meets minimum length |
| `Length` | Validates string or collection length is within range |
| `Matches` | Validates string matches a regex pattern |
| `Range` | Validates numeric value is within range |
| `Email` | Validates string is a valid email address |
| `CreditCard` | Validates string is a valid credit card number |
| `Phone` | Validates string is a valid phone number |
| `NotNull` | Validates property is not null |
| `Null` | Validates property is null |
| `Empty` | Validates property is empty |
| `GreaterThan` | Validates value is greater than specified value |
| `GreaterThanOrEqual` | Validates value is greater than or equal to specified value |
| `LessThan` | Validates value is less than specified value |
| `LessThanOrEqual` | Validates value is less than or equal to specified value |
| `Equal` | Validates value equals specified value |
| `NotEqual` | Validates value does not equal specified value |
| `InclusiveBetween` | Validates value is between specified values (inclusive) |
| `ExclusiveBetween` | Validates value is between specified values (exclusive) |
| `IsEnum` | Validates value is valid enum |
| `IsEnumName` | Validates string is valid enum name |
| `PrecisionScale` | Validates decimal has specified precision and scale |

## Dependency Injection

### Basic Validator Registration

To use the basic validators, register them in your DI container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddValidator();
    
    // Other service registrations
}
```

### Fluent Validator Registration

To use fluent validators, register them with:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register validators from specific assemblies
    services.AddFluentValidator(
        typeof(UserValidator).Assembly, 
        typeof(ProductValidator).Assembly
    );
    
    // Or scan all assemblies for validators
    services.AddFluentValidator();
    
    // Other service registrations
}
```

## Creating Custom Validation Rules

### Using `Must` for Custom Logic

The simplest way to add custom validation logic is using the `Must` method:

```csharp
RuleFor(u => u.Username)
    .Must(username => !username.Contains("admin"), "Username cannot contain 'admin'");
```

For validation that needs access to the whole model:

```csharp
RuleFor(u => u.ConfirmPassword)
    .Must((user, confirmPassword) => confirmPassword == user.Password, 
        "Password and confirmation password must match");
```

### Creating a Custom PropertyRule Extension Method

To create reusable validation rules, extend the `PropertyRule<T, TProperty>` class:

```csharp
public static class PropertyRuleExtensions
{
    public static PropertyRule<T, string> IsValidUrl<T>(
        this PropertyRule<T, string> rule, 
        string? errorMessage = null) where T : class
    {
        return rule.Must(url => 
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && 
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }, errorMessage ?? $"{rule.GetPropertyName()} must be a valid URL");
    }
}
```

Usage:

```csharp
public class Website
{
    public string Url { get; set; }
}

public class WebsiteValidator : FluentValidator<Website>
{
    public WebsiteValidator()
    {
        RuleFor(w => w.Url)
            .NotEmpty()
            .IsValidUrl();  // Using our custom extension
    }
}
```

### Using `WithCustomValidation`

For more complex validation using .NET's `ValidationAttribute`:

```csharp
public class CustomUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string url || string.IsNullOrEmpty(url))
            return ValidationResult.Success;
            
        bool isValid = Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && 
                      (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                      
        return isValid 
            ? ValidationResult.Success 
            : new ValidationResult($"{validationContext.MemberName} must be a valid URL");
    }
}

// In validator
RuleFor(w => w.Url).WithCustomValidation(new CustomUrlAttribute());
```

## Conditional Validation

The library supports conditional validation with `When` and `Unless`:

```csharp
public class Account
{
    public bool IsBusinessAccount { get; set; }
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
}

public class AccountValidator : FluentValidator<Account>
{
    public AccountValidator()
    {
        RuleFor(a => a.CompanyName)
            .When(a => a.IsBusinessAccount, rule => {
                rule.NotEmpty("Company name is required for business accounts")
                    .MaxLength(100);
            });
            
        RuleFor(a => a.TaxId)
            .When(a => a.IsBusinessAccount, rule => {
                rule.NotEmpty("Tax ID is required for business accounts")
                    .Length(9, 9, "Tax ID must be exactly 9 characters");
            });
    }
}
```

The `Unless` method works inversely to `When`:

```csharp
RuleFor(a => a.PersonalDetails)
    .Unless(a => a.IsBusinessAccount, rule => {
        rule.NotNull("Personal details are required for individual accounts");
    });
```