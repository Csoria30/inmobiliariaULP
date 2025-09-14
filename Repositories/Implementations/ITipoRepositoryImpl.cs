using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
namespace inmobiliariaULP.Repositories.Implementations;

public class TipoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), ITipoRepository
{
    public async Task<IEnumerable<Tipo>> GetAllAsync()
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            Select id_tipo, descripcion 
            From tipos;"
        ;

        var tipos = new List<Tipo>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tipos.Add(new Tipo
            {
                TipoId = reader.GetInt32("id_tipo"),
                Descripcion = reader.GetString("descripcion")
            });
        }

        return tipos;
    }

}