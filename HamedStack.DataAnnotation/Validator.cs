using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace HamedStack.DataAnnotation;

/// <summary>
/// Interface for a generic object validator.
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Validates the given model and returns a list of validation results.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <returns>A collection of <see cref="ValidationResult"/> that contains the validation results.</returns>
    ICollection<ValidationResult> Validate(object model);

    /// <summary>
    /// Determines whether the given model is valid.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <returns><c>true</c> if the model is valid, otherwise <c>false</c>.</returns>
    bool IsValid(object model);

    /// <summary>
    /// Validates the given model and outputs the validation results.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <param name="validationResult">A collection of validation results.</param>
    /// <returns><c>true</c> if the model is valid, otherwise <c>false</c>.</returns>
    bool Validate(object model, out ICollection<ValidationResult> validationResult);
}

/// <summary>
/// Interface for a generic object validator that validates a model of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of model to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the given model and returns a list of validation results.
    /// </summary>
    /// <param name="model">The object of type <typeparamref name="T"/> to validate.</param>
    /// <returns>A list of <see cref="ValidationResult"/> that contains the validation results.</returns>
    IList<ValidationResult> Validate(T model);

    /// <summary>
    /// Determines whether the given model of type <typeparamref name="T"/> is valid.
    /// </summary>
    /// <param name="model">The object of type <typeparamref name="T"/> to validate.</param>
    /// <returns><c>true</c> if the model is valid, otherwise <c>false</c>.</returns>
    bool IsValid(T model);
}

/// <summary>
/// A validator implementation that uses data annotations for validation of objects.
/// </summary>
public class Validator : IValidator
{
    /// <summary>
    /// Validates the given object using data annotations and returns a list of validation results.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <returns>A list of <see cref="ValidationResult"/> that contains the validation results.</returns>
    public ICollection<ValidationResult> Validate(object model)
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();

        // Validates the object and collects validation results.
        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

        return validationResults;
    }

    /// <summary>
    /// Determines whether the given object is valid using data annotations.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <returns><c>true</c> if the object is valid, otherwise <c>false</c>.</returns>
    public bool IsValid(object model)
    {
        var validationResults = Validate(model);
        return validationResults.Count == 0;
    }

    /// <summary>
    /// Validates the given object and outputs the validation results.
    /// </summary>
    /// <param name="model">The object to validate.</param>
    /// <param name="results">A collection of validation results.</param>
    /// <returns><c>true</c> if the object is valid, otherwise <c>false</c>.</returns>
    public bool Validate(object model, out ICollection<ValidationResult> results)
    {
        results = Validate(model);
        return results.Count == 0;
    }
}

/// <summary>
/// A generic validator implementation that uses data annotations for validation of objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the model to validate.</typeparam>
public class Validator<T> : IValidator<T>
{
    /// <summary>
    /// Validates the given model of type <typeparamref name="T"/> using data annotations and returns a list of validation results.
    /// </summary>
    /// <param name="model">The object of type <typeparamref name="T"/> to validate.</param>
    /// <returns>A list of <see cref="ValidationResult"/> that contains the validation results.</returns>
    public IList<ValidationResult> Validate(T model)
    {
        var validationResults = new List<ValidationResult>();

        if (model != null)
        {
            var validationContext = new ValidationContext(model);
            // Validates the model and collects validation results.
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
        }

        return validationResults;
    }

    /// <summary>
    /// Determines whether the given model of type <typeparamref name="T"/> is valid using data annotations.
    /// </summary>
    /// <param name="model">The object of type <typeparamref name="T"/> to validate.</param>
    /// <returns><c>true</c> if the model is valid, otherwise <c>false</c>.</returns>
    public bool IsValid(T model)
    {
        var validationResults = Validate(model);
        return validationResults.Count == 0;
    }
}

/// <summary>
/// Extension methods for registering data annotation validators in a service collection.
/// </summary>
public static class ValidatorServiceCollection
{
    /// <summary>
    /// Registers the <see cref="Validator"/> and <see cref="Validator{T}"/> as singletons in the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to register the services with.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the added data annotation validators.</returns>
    public static IServiceCollection AddValidator(this IServiceCollection services)
    {
        // Registers the Validator and Validator<T> with the DI container.
        services.AddSingleton<IValidator, Validator>();
        services.AddSingleton(typeof(IValidator<>), typeof(Validator<>));

        return services;
    }
}