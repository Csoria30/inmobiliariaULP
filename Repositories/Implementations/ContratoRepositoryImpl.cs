using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Repositories.Implementations;

public class ContratoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IContratoRepository
{
    public Task<int> AddAsync(Contrato contrato)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public async Task<(IEnumerable<ContratoListadoDTO> Contratos, int Total)> GetAllAsync(int page, int pageSize, string? search = null)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        // 1. Armar el WHERE si hay búsqueda
        string where = "";
        if (!string.IsNullOrEmpty(search))
        {
            where = @"
                WHERE 
                    i.direccion LIKE @search OR 
                    t.descripcion LIKE @search OR
                    p.nombre LIKE @search OR
                    p.apellido LIKE @search OR
                    pi.nombre LIKE @search OR
                    pi.apellido LIKE @search
            ";
        }

        // 2. Obtener el total de registros (filtrado si hay búsqueda)
        int total;
        using (var countCommand = connection.CreateCommand())
        {
            countCommand.CommandText = $@"
                SELECT COUNT(*) 
                FROM contratos c
                JOIN inmuebles i ON i.id_inmueble = c.id_inmueble
                JOIN tipos t ON i.id_tipo = t.id_tipo
                JOIN propietarios pro ON i.id_propietario = pro.id_propietario
                JOIN personas p ON pro.id_persona = p.id_persona
                JOIN inquilinos inq ON inq.id_inquilino = c.id_inquilino
                JOIN personas pi ON inq.id_persona = pi.id_persona
                {where}
            ";

            if (!string.IsNullOrEmpty(search))
                countCommand.Parameters.AddWithValue("@search", $"%{search}%");

            total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
        }

        // 3. Consulta paginada y filtrada
        var contratos = new List<ContratoListadoDTO>();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = $@"
                SELECT 
                    c.id_contrato AS ContratoId,
                    i.direccion AS Direccion,
                    t.descripcion AS TipoInmueble,
                    CONCAT(p.apellido, ' ', p.nombre) AS NombrePropietario,
                    CONCAT(pi.apellido, ' ', pi.nombre) AS NombreInquilino,
                    c.fecha_inicio AS FechaInicio,
                    c.fecha_fin AS FechaFin,
                    c.monto_mensual AS MontoMensual,
                    c.estado AS EstadoContrato,
                    (SELECT COUNT(*) FROM pagos pg WHERE pg.id_contrato = c.id_contrato AND pg.estadoPago = 'aprobado') AS PagosRealizados
                FROM contratos c
                    JOIN inmuebles i ON i.id_inmueble = c.id_inmueble
                    JOIN tipos t ON i.id_tipo = t.id_tipo
                    JOIN propietarios pro ON i.id_propietario = pro.id_propietario
                    JOIN personas p ON pro.id_persona = p.id_persona
                    JOIN inquilinos inq ON inq.id_inquilino = c.id_inquilino
                    JOIN personas pi ON inq.id_persona = pi.id_persona
                    {where}
                LIMIT @PageSize OFFSET @Offset
            ";

            if (!string.IsNullOrEmpty(search))
                command.Parameters.AddWithValue("@search", $"%{search}%");

            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var contrato = new ContratoListadoDTO
                {
                    ContratoId = reader.GetInt32("ContratoId"),
                    Direccion = reader.GetString("Direccion"),
                    TipoInmueble = reader.GetString("TipoInmueble"),
                    NombrePropietario = reader.GetString("NombrePropietario"),
                    NombreInquilino = reader.GetString("NombreInquilino"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    MontoMensual = reader.GetDecimal("MontoMensual"),
                    EstadoContrato = reader.GetString("EstadoContrato"),
                    PagosRealizados = reader.GetInt32("PagosRealizados")
                };
                contratos.Add(contrato);
            }
        }

        return (contratos, total);
    }

    public Task<Contrato> GetByIdAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(int contratoId)
    {
        throw new NotImplementedException();
    }

}