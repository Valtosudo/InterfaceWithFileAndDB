var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Autonomistaja tietokannan olio, jonka kautta saadaan tieto autonomistajista
var autonomistajat = new AutonOmistajatDB();
//Pyöränomistajat tiedoston sijainti
var pyoranomistajat = "pyoranomistajat.txt";

app.MapGet("/", () => "Hello World!");

app.MapGet("/omistajat", async () =>
{
    var omistajat = await autonomistajat.ListaaAutonOmistajatAsync();
    return Results.Ok(omistajat);
});

app.MapGet("/kaikki", async () =>
{
    var omistajat = await KaikkiOmistajat(pyoranomistajat, autonomistajat).ToListAsync();
    return Results.Ok(omistajat);
});

app.MapPost("/omistajat", async (Autonomistajat uusiOmistaja) =>
{
    await autonomistajat.LisaaOmistajaAsync(uusiOmistaja.Nimi, uusiOmistaja.Osoite, uusiOmistaja.Puhelin, uusiOmistaja.Syntymavuosi);
    return Results.Created($"/AutonOmistajatDB/{uusiOmistaja.Nimi}", uusiOmistaja);
});

app.Run();

static async IAsyncEnumerable<string> KaikkiOmistajat(string tiedosto, AutonOmistajatDB db)
{
    if (File.Exists(tiedosto))
    {
        var lines = await File.ReadAllLinesAsync(tiedosto);
        foreach (string nimi in lines)
        {
            yield return nimi;
        }

        foreach (var omistaja in await db.ListaaAutonOmistajatAsync())
        {
            yield return omistaja.Nimi;
        }
    }
}
