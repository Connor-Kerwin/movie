using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Movie.Models;

public class KebabCaseEnumModelBinder<T> : IModelBinder
    where T : struct, Enum
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        // No enum specified
        if (string.IsNullOrWhiteSpace(value.FirstValue))
        {
            bindingContext.Result = ModelBindingResult.Success(new HashSet<T>());
            return Task.CompletedTask;
        }

        var set = new HashSet<T>();
        var hasError = false;
        foreach (var item in value.Values)
        {
            var result = ParseEnum(item);
            if (result == null)
            {
                hasError = true;
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, $"The value '{item}' is not valid for {bindingContext.ModelName}.");
                continue;
            }

            set.Add(result.Value);
        }

        if (hasError)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
        
        bindingContext.Result = ModelBindingResult.Success(set);
        return Task.CompletedTask;
    }

    private T? ParseEnum(string value)
    {
        foreach (var enumName in Enum.GetNames(typeof(T)))
        {
            if (ToKebabCase(enumName).Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return Enum.Parse<T>(enumName);
            }
        }

        return null;
    }

    private string ToKebabCase(string input)
    {
        return string.Concat(input.Select((c, i) =>
            char.IsUpper(c) && i > 0 ? "-" + char.ToLower(c) : char.ToLower(c).ToString()));
    }
}