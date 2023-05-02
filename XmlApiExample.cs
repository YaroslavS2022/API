using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using System;
using System.Globalization;
using System.Net.Cache;
using static Umbraco.Core.Constants;

XElement XmlConstructor()
{
    return new XElement("records");
}
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<MyDbContext>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basic API", Description = "Creating Basic API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});

app.MapGet("/", () => "Hello World!");

app.MapPost("/", (Person person) => { });

app.MapGet("/oops", () => "Oops! An error happened.");

app.MapGet("/people", async (MyDbContext dbContext) =>
{
    var people = await (dbContext.Persons).ToListAsync();
    return Results.Ok(people);
});

/*app.MapGet("/people/{id}", async (int id, MyDbContext dbContext) =>
{
    var book = await dbContext.Persons
            .FirstOrDefaultAsync(_ => _.Id == id);
    if (book is null)
        return Results.NotFound();
    return Results.Ok(book);
});

app.MapPost("/people", async (Person createdPerson, MyDbContext dbContext) =>
{
    if (createdPerson == null)
        dbContext.Persons.Add(new Person ());
    else
        dbContext.Persons.Add(createdPerson);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/people/{createdPerson.Id}", createdPerson);
});*/

app.Run();
class Service { }

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
}
public class MyDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }
    public string DbPath { get; }
    public MyDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "people.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}