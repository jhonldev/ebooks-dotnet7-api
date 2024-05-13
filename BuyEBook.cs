using System.ComponentModel.DataAnnotations;

namespace ebooks_dotnet7_api;

/// <summary>
/// Represents an eBook entity.
/// </summary>
public class BuyEBook
{

    /// <summary>
    /// Unique identifier for the eBook.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Price of the eBook.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int QuantityEBook { get; set; }


    /// <summary>
    /// Price of the eBook.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PriceTotal { get; set; }

}
