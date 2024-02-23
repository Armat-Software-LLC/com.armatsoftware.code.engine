using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ArmatSoftware.Code.Engine.Compiler.Utils;

public static class ObjectValidator
{
    public static void Validate<T>(T model)
        where T : class
    {
        var validationResults = new Collection<ValidationResult>();
        if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
        {
            throw new ValidationException($"Validation failed for {typeof(T)}: {string.Join(", ", validationResults)}");
        }
    }
}