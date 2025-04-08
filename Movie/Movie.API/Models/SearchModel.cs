using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Movie.API.Utility;

namespace Movie.API.Models;

public class SearchModel
{
    /// <summary>
    /// The zero-based page index to get.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    [FromQuery(Name = "page")]
    public int Page { get; set; }

    /// <summary>
    /// The number of items to return in a page. This is limited to 64.
    /// </summary>
    [Required]
    [Range(1, 64)]
    [FromQuery(Name = "pagesize")]
    public int PageSize { get; set; }

    /// <summary>
    /// The field to sort by.
    /// </summary>
    [FromQuery(Name = "sortby")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<SortBy>))]
    public SortBy SortBy { get; set; } = SortBy.Title;

    /// <summary>
    /// The order to return results in.
    /// </summary>
    [FromQuery(Name = "orderby")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<OrderBy>))]
    public OrderBy OrderBy { get; set; } = OrderBy.Asc;

    /// <summary>
    /// A set of genres to filter by. You can choose more than one to filter by.
    /// </summary>
    [MaxLength(32)]
    [FromQuery(Name = "genres")]
    [ModelBinder(BinderType = typeof(KebabCaseEnumModelBinder<MovieGenre>))]
    public HashSet<MovieGenre>? Genres { get; set; }
}

public enum SortBy
{
    Title,
    ReleaseDate
}

public enum OrderBy
{
    Asc,
    Desc
}