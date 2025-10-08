using Microsoft.Data.Sqlite;

public record Autonomistajat
(
    int Id,
    string Nimi,
    string Osoite,
    string Puhelin,
    int Syntymavuosi
);

public class AutonOmistajatDB
{
    private string _connectionstring = "Data source = Autonomistajat.db";

    public AutonOmistajatDB()
    {
        using (var connection = new SqliteConnection(_connectionstring))
        {
            connection.Open();
            var tableCmd = connection.CreateCommand();
            tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS AutonOmistajat (
                                    Id INTEGER PRIMARY KEY,
                                    Nimi TEXT,
                                    Osoite TEXT,
                                    Puhelin TEXT,
                                    Syntymavuosi INTEGER);";
            tableCmd.ExecuteNonQuery();
        }
    }

    public async Task LisaaOmistajaAsync(string nimi, string osoite, string puhelin, int syntymavuosi)
    {
        using (var connection = new SqliteConnection(_connectionstring))
        {
            await connection.OpenAsync();

            var insertcmd = connection.CreateCommand();
            insertcmd.CommandText = @"INSERT INTO AutonOmistajat(Nimi, Osoite, Puhelin, Syntymavuosi)
                                     VALUES ($nimi, $osoite, $puhelin, $syntymavuosi);";
            insertcmd.Parameters.AddWithValue("$nimi", nimi);
            insertcmd.Parameters.AddWithValue("$osoite", osoite);
            insertcmd.Parameters.AddWithValue("$puhelin", puhelin);
            insertcmd.Parameters.AddWithValue("$syntymavuosi", syntymavuosi);

            await insertcmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Autonomistajat>> ListaaAutonOmistajatAsync()
    {
        var omistajat = new List<Autonomistajat>();

        using (var connection = new SqliteConnection(_connectionstring))
        {
            await connection.OpenAsync();

            var selectcmd = connection.CreateCommand();
            selectcmd.CommandText = @"SELECT Id, Nimi, Osoite, Puhelin, Syntymavuosi FROM AutonOmistajat;";

            using (var reader = await selectcmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var omistaja = new Autonomistajat(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4)
                    );

                    omistajat.Add(omistaja);
                }
            }
        }
        return omistajat;
    }
}