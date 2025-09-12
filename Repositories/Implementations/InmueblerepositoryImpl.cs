using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
namespace inmobiliariaULP.Repositories.Implementations;

public class InmuebleRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IInmuebleRepository
{
    public async Task<Inmueble> AddAsync(Inmueble inmueble)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO inmobiliariaulp.inmuebles
            (direccion, uso, ambientes, coordenadas, precio_base, estado, id_propietario, id_tipo)
            VALUES
            (@Direccion, @Uso, @Ambientes, @Coordenadas, @PrecioBase, @Estado, @IdPropietario, @IdTipo);
        ";

        command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
        command.Parameters.AddWithValue("@Uso", inmueble.Uso);
        command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
        command.Parameters.AddWithValue("@Coordenadas", inmueble.Coordenadas);
        command.Parameters.AddWithValue("@PrecioBase", inmueble.PrecioBase);
        command.Parameters.AddWithValue("@Estado", inmueble.Estado);
        command.Parameters.AddWithValue("@IdPropietario", inmueble.PropietarioId);
        command.Parameters.AddWithValue("@IdTipo", inmueble.TipoId);

        var result = await command.ExecuteScalarAsync();
        return inmueble;
    }


}