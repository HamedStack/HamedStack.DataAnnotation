# HamedStack.DataAnnotation

**HamedStack.DataAnnotation** is a lightweight and flexible library that simplifies validation of objects using Data Annotations in .NET. This library provides two core interfaces and their implementations, allowing you to easily integrate data validation into your application by leveraging the power of .NET’s `System.ComponentModel.DataAnnotations` namespace.

---

## Overview

**HamedStack.DataAnnotation** provides simple object validation capabilities through the use of **Data Annotations**, allowing you to ensure your models meet the required validation criteria before performing any further operations (e.g., saving to a database, processing user input, etc.).

### Key Components
1. **IValidator**: A non-generic interface for validating any object.
2. **IValidator&lt;T&gt;**: A generic interface for validating objects of a specific type.
3. **DataAnnotationValidator**: The non-generic implementation of **IValidator**.
4. **DataAnnotationValidator&lt;T&gt;**: The generic implementation of **IValidator&lt;T&gt;**.
5. **ServiceCollectionExtensions**: Extension methods for registering the validators in the dependency injection container.

---

## Key Features

- **Data Annotations**: Leverages .NET's built-in `System.ComponentModel.DataAnnotations` for model validation.
- **Generics Support**: Both generic and non-generic validators to handle validation of any object or a specific type.
- **Dependency Injection Integration**: Easily integrates with ASP.NET Core's dependency injection system to enable automatic validation.
- **Seamless Validation**: Works directly with `ValidationResult` to collect and handle validation errors.

---

## Usage

### Creating and Using Validators

The library provides two types of validators: one for general object validation (`DataAnnotationValidator`), and another for specific types (`DataAnnotationValidator<T>`).

#### Non-Generic Validator (`IValidator` and `DataAnnotationValidator`)

You can use the non-generic validator for any object that may or may not have a specific type. This validator works well when you don’t know the type of the model ahead of time or are working with loosely typed objects.

**Example:**
```csharp
IValidator validator = new DataAnnotationValidator();
var model = new MyModel();  // Some model class with Data Annotations applied

// Validate the model
IList<ValidationResult> validationResults = validator.Validate(model);

// Check if the model is valid
bool isValid = validator.IsValid(model);
```

#### Generic Validator (`IValidator<T>` and `DataAnnotationValidator<T>`)

For scenarios where you know the type of the model upfront, you can use the generic version of the validator. This provides type safety and is more efficient for validating strongly typed models.

**Example:**
```csharp
IValidator<MyModel> validator = new DataAnnotationValidator<MyModel>();
var model = new MyModel();  // Some model class with Data Annotations applied

// Validate the model
IList<ValidationResult> validationResults = validator.Validate(model);

// Check if the model is valid
bool isValid = validator.IsValid(model);
```

### Integrating with Dependency Injection

To integrate **HamedStack.DataAnnotation** with **ASP.NET Core's Dependency Injection** system, you can register the validators in the `Startup.cs` or `Program.cs` file.

**Example:**

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register the Data Annotation Validators with Dependency Injection
        services.AddDataAnnotationValidator();
        
        // Other services...
    }
}
```

After adding the extension method `AddDataAnnotationValidator`, both the generic and non-generic validators will be available for injection into your controllers or services.

**Example:**

```csharp
public class MyService
{
    private readonly IValidator<MyModel> _validator;

    public MyService(IValidator<MyModel> validator)
    {
        _validator = validator;
    }

    public void ValidateModel(MyModel model)
    {
        if (!_validator.IsValid(model))
        {
            // Handle validation failure
            IList<ValidationResult> results = _validator.Validate(model);
            foreach (var result in results)
            {
                // Log or handle validation errors
                Console.WriteLine(result.ErrorMessage);
            }
        }
        else
        {
            // Proceed with valid model
        }
    }
}
```

---

## Example

Let's walk through a basic example using a model with data annotations.

### Model Definition

```csharp
public class User
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
    public int Age { get; set; }
}
```

### Validator Usage

```csharp
public class UserController : Controller
{
    private readonly IValidator<User> _userValidator;

    public UserController(IValidator<User> userValidator)
    {
        _userValidator = userValidator;
    }

    public IActionResult CreateUser(User user)
    {
        if (!_userValidator.IsValid(user))
        {
            var validationResults = _userValidator.Validate(user);
            foreach (var result in validationResults)
            {
                // Handle each validation error (e.g., display in UI)
                Console.WriteLine(result.ErrorMessage);
            }
            return BadRequest("User validation failed.");
        }

        // Proceed with valid user creation logic
        return Ok("User created successfully.");
    }
}
```

---