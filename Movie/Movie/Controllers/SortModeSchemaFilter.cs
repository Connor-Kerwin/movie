// using Microsoft.OpenApi.Any;
// using Microsoft.OpenApi.Models;
// using Movie.Models;
// using Swashbuckle.AspNetCore.SwaggerGen;
//
// namespace Movie.Controllers;
//
// public class SortModeSchemaFilter : ISchemaFilter
// {
//     public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//     {
//         if (context.Type == typeof(SortMode))
//         {
//             schema.Enum = new List<IOpenApiAny>()
//             {
//                 new OpenApiString(nameof(SortMode.Title).ToLower()),
//                 new OpenApiString(nameof(SortMode.ReleaseDate).ToLower()),
//             };
//         }
//     }
// }
//
// public class MovieGenreSchemaFilter : ISchemaFilter
// {
//     public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//     {
//         if (context.Type == typeof(MovieGenre))
//         {
//             var enumNames = Enum.GetNames(typeof(MovieGenre));
//             schema.Type = "array";
//             schema.Items = new OpenApiSchema
//             {
//                 Type = "string",
//                 Enum = enumNames
//                     .Select(name => new Microsoft.OpenApi.Any.OpenApiString(name))
//                     .Cast<Microsoft.OpenApi.Any.IOpenApiAny>()
//                     .ToList()
//             };
//         }
//     }
// }