using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

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
        /// <returns>A list of <see cref="ValidationResult"/> that contains the validation results.</returns>
        IList<ValidationResult> Validate(object model);

        /// <summary>
        /// Determines whether the given model is valid.
        /// </summary>
        /// <param name="model">The object to validate.</param>
        /// <returns><c>true</c> if the model is valid, otherwise <c>false</c>.</returns>
        bool IsValid(object model);
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
    public class DataAnnotationValidator : IValidator
    {
        /// <summary>
        /// Validates the given object using data annotations and returns a list of validation results.
        /// </summary>
        /// <param name="model">The object to validate.</param>
        /// <returns>A list of <see cref="ValidationResult"/> that contains the validation results.</returns>
        public IList<ValidationResult> Validate(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Validates the object and collects validation results.
            Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

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
    }

    /// <summary>
    /// A generic validator implementation that uses data annotations for validation of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the model to validate.</typeparam>
    public class DataAnnotationValidator<T> : IValidator<T>
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
                Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
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
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the <see cref="DataAnnotationValidator"/> and <see cref="DataAnnotationValidator{T}"/> as singletons in the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to register the services with.</param>
        /// <returns>The <see cref="IServiceCollection"/> with the added data annotation validators.</returns>
        public static IServiceCollection AddDataAnnotationValidator(this IServiceCollection services)
        {
            // Registers the DataAnnotationValidator and DataAnnotationValidator<T> with the DI container.
            services.AddSingleton<IValidator, DataAnnotationValidator>();
            services.AddSingleton(typeof(IValidator<>), typeof(DataAnnotationValidator<>));

            return services;
        }
    }