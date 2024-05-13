using System.Collections.Specialized;
using ebooks_dotnet7_api;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("ebooks"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var ebooks = app.MapGroup("api/ebook");

// TODO: Add more routes
ebooks.MapPost("/", CreateEBookAsync);
ebooks.MapGet("/", GetEBookAsync);
ebooks.MapPut("/{id}", PutEBookAsync);
ebooks.MapPut("/{id}/change-availability", PutAvailableAsync);
ebooks.MapPut("/{id}/increment-stock", PutStockAsync);
ebooks.MapPost("purchase", PostPurchaseAsync);
ebooks.MapDelete("/{id}", DeleteEBookAsync);

app.Run();

// TODO: Add more methods
async Task<IResult> CreateEBookAsync(DataContext context, EBookReal ebook)
{
    if (await context.EBooks.AnyAsync(e => e.Title == ebook.Title && e.Author == ebook.Author))
    {
        return Results.BadRequest("La combinación de libro y autor ya existe");
    }

    var ebookNew = new EBook
    {
        Title = ebook.Title,
        Author = ebook.Author,
        Genre = ebook.Genre,
        Format = ebook.Format,
        IsAvailable = ebook.IsAvailable,
        Price = ebook.Price,
        Stock = ebook.Stock,
    };

    await context.AddAsync(ebookNew);
    await context.SaveChangesAsync();
    return Results.Ok("El libro se creó correctamente");
}


async Task<IResult> GetEBookAsync(DataContext context, string ? genre, string ? author, string ? format){
    
    var ebooks = await context.EBooks.OrderBy(e => e.Title).ToListAsync();

    if(genre != null && author != null && format != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Genre == genre && d.Author == author && d.Format == format).ToListAsync();
    }else if(author != null && genre != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Genre == genre && d.Author == author).ToListAsync();
    }else if(format != null && genre != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Genre == genre && d.Format == format).ToListAsync();
    }else if(format != null && author != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Format == format && d.Author == author).ToListAsync();
    }else if(genre != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Genre == genre).ToListAsync();
    }else if(author != null){
        ebooks = await context.EBooks.OrderBy(e => e.Title).Where(d => d.Author == author).ToListAsync();
    }else if(format != null){
        ebooks= await context.EBooks.OrderBy(e => e.Title).Where(d => d.Format == format).ToListAsync();
    }
    return Results.Ok(ebooks);
}

async Task<IResult> PutEBookAsync(DataContext context, UpdateEBook InfoEBook, int id){
    var ebooks = await context.EBooks.FindAsync(id);

    if(ebooks == null){
        return Results.BadRequest("El libro no Existe");
    }

    ebooks.Title = InfoEBook.Title ?? ebooks.Title;
    ebooks.Author = InfoEBook.Author ?? ebooks.Author;
    ebooks.Genre = InfoEBook.Genre ?? ebooks.Genre;
    ebooks.Format = InfoEBook.Format ?? ebooks.Format;
    ebooks.Price = InfoEBook.Price ?? ebooks.Price;

    await context.SaveChangesAsync();
    return Results.Ok();
}

async Task<IResult> PutAvailableAsync(DataContext context, int id){
    var ebooks = await context.EBooks.FindAsync(id);

    if(ebooks == null){
        return Results.BadRequest("El libro no Existe");
    }

    ebooks.IsAvailable = !ebooks.IsAvailable;

    await context.SaveChangesAsync(); 
    return Results.Ok();
}

async Task<IResult> PutStockAsync(DataContext context, int id,UpdateStock stock){
    var ebooks = await context.EBooks.FindAsync(id);

    if(ebooks == null){
        return Results.BadRequest("El libro no Existe");
    }

    ebooks.Stock = stock.Stock;

    await context.SaveChangesAsync();
    return Results.Ok();
}

async Task<IResult> PostPurchaseAsync(DataContext context, BuyEBook buyEBooks){
    var ebooks = await context.EBooks.FindAsync(buyEBooks.Id);

    if(ebooks == null){
        return Results.BadRequest("El libro no Existe");
    }

    if(!ebooks.IsAvailable){
        return Results.BadRequest("El libro no esta disponible");
    }

    if((buyEBooks.PriceTotal*buyEBooks.QuantityEBook) != (ebooks.Price*buyEBooks.QuantityEBook)){
        return Results.BadRequest("El precio a pagar no es valido");
    }

    ebooks.Stock -= buyEBooks.QuantityEBook;
    await context.SaveChangesAsync();
    return Results.Ok();
}

async Task<IResult> DeleteEBookAsync(DataContext context, int id){
    var ebooks = await context.EBooks.FindAsync(id);

    if(ebooks == null){
        return Results.BadRequest("El libro no Existe");
    }
    
    context.EBooks.Remove(ebooks);
    await context.SaveChangesAsync();

    return Results.Ok();
}