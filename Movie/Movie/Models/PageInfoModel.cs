namespace Movie.Models;

public class PageInfoModel
{
    /// <summary>
    /// The total number of records that the pagination is traversing (full set).
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// The current page that is being viewed.
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// The full size of the current page.
    /// This is not the number of items that were returned for this page.
    /// </summary>
    public int PageSize { get; set; }
}