using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Movie.Models;

/// <summary>
/// To match kebab-casing for genres and other enums, a custom implementation of a model binder was required.
/// This is mostly because the URL query parameters are not JSON, so the JSON kebab case code cant be used here.
/// </summary>
/// <typeparam name="T"></typeparam>
public class KebabCaseEnumModelBinder<T> : IModelBinder
    where T : struct, Enum
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType == typeof(HashSet<T>))
        {
            return ParseAsHashSet(bindingContext);
        }

        if (bindingContext.ModelType == typeof(T))
        {
            return ParseAsSingleValue(bindingContext);
        }

        throw new InvalidOperationException("Binding ModelType not supported");
    }

    private Task ParseAsSingleValue(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        // No enum specified
        if (string.IsNullOrWhiteSpace(value.FirstValue))
        {
            bindingContext.Result = ModelBindingResult.Success(default(T));
            return Task.CompletedTask;
        }
        
        // NOTE: This is somewhat naiive, it's always going to read the first value.

        var result = ParseEnum(value.FirstValue);
        if (result == null)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(result);
        return Task.CompletedTask;
    }

    private Task ParseAsHashSet(ModelBindingContext bindingContext)
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
                bindingContext.ModelState.AddModelError(bindingContext.ModelName,
                    $"The value '{item}' is not valid for {bindingContext.ModelName}.");
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