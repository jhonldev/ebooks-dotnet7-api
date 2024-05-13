using System.ComponentModel.DataAnnotations;

namespace ebooks_dotnet7_api;

/// <summary>
/// Represents an eBook entity.
/// </summary>
public class UpdateStock
{

   /// <summary>
    /// Stock of the eBook.
    /// </summary>
    public required int Stock { get; set; }

}
