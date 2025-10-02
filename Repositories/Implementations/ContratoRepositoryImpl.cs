using System.Data;
using inmobiliariaULP.Models;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Repositories.Implementations;

public class ContratoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IContratoRepository
{
    public async Task<int> AddAsync(Contrato contrato)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO contratos(
                id_inmueble, id_inquilino, id_usuario, 
                fecha_inicio,  fecha_fin, monto_mensual, 
                estado

            )VALUES(
                @IdInmueble, @IdInquilino, @IdUsuario, 
                @FechaInicio, @FechaFin, @MontoMensual, 
                @EstadoContrato
            );

            SELECT LAST_INSERT_ID();
        ";

        command.Parameters.AddWithValue("@IdInmueble", contrato.InmuebleId);
        command.Parameters.AddWithValue("@IdInquilino", contrato.InquilinoId);
        command.Parameters.AddWithValue("@IdUsuario", contrato.UsuarioId);
        command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
        command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
        command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
        command.Parameters.AddWithValue("@EstadoContrato", contrato.Estado);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<int> DeleteAsync(int contratoId)
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

    public async Task<ContratoDetalleDTO> GetByIdAsync(int contratoId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    c.id_contrato AS ContratoId,
                    -- Inmueble
                    c.id_inmueble AS InmuebleId,
                    i.direccion AS Direccion,
                    t.descripcion AS TipoInmueble,
                    i.uso AS UsoInmueble,
                    i.ambientes AS Ambientes,
                    i.coordenadas AS Coordenadas,
                    i.estado AS EstadoInmueble,
                    
                    -- Propietario
                    pro.id_propietario AS PropietarioId,
                    pro.id_persona AS PropietarioIdPersona,
                    CONCAT(p.apellido, ' ', p.nombre ) AS NombrePropietario,
                    p.email AS EmailPropietario,
                    p.telefono AS TelefonoPropietario,
                    
                    -- Inquilino
                    c.id_inquilino AS InquilinoId,
                    pi.id_persona AS InquilinoIdPersona,
                    CONCAT(pi.apellido, ' ', pi.nombre) AS NombreInquilino,
                    pi.email AS EmailInquilino,
                    pi.telefono AS TelefonoInquilino,
                    
                    -- Usuario Inicio
                    c.id_usuario AS UsuarioId,
                    CONCAT(pu.apellido, ' ', pu.nombre) AS NombreEmpleado,
                    pu.email AS EmailUsuario,
                    u.rol AS RolUsuario,
                    -- Usuario Fin
                    c.id_usuario_finaliza AS UsuarioIdFin,
                    CONCAT(puf.apellido, ' ', puf.nombre) AS NombreEmpleadoFin,
                    puf.email AS EmailUsuarioFin,
                    uf.rol AS RolUsuarioFin,
                    
                    -- Contrato
                    c.fecha_inicio AS FechaInicio,
                    c.fecha_fin AS FechaFin,
                    c.monto_mensual AS MontoMensual,
                    c.fecha_finalizacion_anticipada AS FechaAnticipada,
                    c.multa AS Multa,
                    c.estado AS EstadoContrato,
                    -- Pagos
                    (SELECT COUNT(*) FROM pagos pg WHERE pg.id_contrato = c.id_contrato AND pg.estadoPago = 'aprobado') AS PagosRealizados

                FROM contratos c
                    JOIN inmuebles i ON i.id_inmueble = c.id_inmueble
                    JOIN tipos t ON i.id_tipo = t.id_tipo
                    JOIN propietarios pro ON i.id_propietario = pro.id_propietario
                    JOIN personas p ON pro.id_persona = p.id_persona
                    JOIN inquilinos inq ON inq.id_inquilino = c.id_inquilino
                    JOIN personas pi ON inq.id_persona = pi.id_persona
                    JOIN usuarios u ON c.id_usuario = u.id_usuario
                    JOIN empleados e ON u.id_empleado = e.id_empleado
                    JOIN personas pu ON e.id_persona = pu.id_persona
                    LEFT JOIN usuarios uf ON c.id_usuario_finaliza = uf.id_usuario
                    LEFT JOIN empleados ef ON uf.id_empleado = ef.id_empleado
                    LEFT JOIN personas puf ON ef.id_persona = puf.id_persona
                WHERE c.id_contrato = @contratoId
            ";
            command.Parameters.AddWithValue("@contratoId", contratoId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ContratoDetalleDTO
                {
                    ContratoId = reader.GetInt32("ContratoId"),
                    FechaInicio = reader.GetDateTime("FechaInicio"),
                    FechaFin = reader.GetDateTime("FechaFin"),
                    MontoMensual = reader.GetDecimal("MontoMensual"),
                    FechaAnticipada = reader.IsDBNull(reader.GetOrdinal("FechaAnticipada")) ? null : reader.GetDateTime("FechaAnticipada"),
                    Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? null : reader.GetDecimal("Multa"),
                    EstadoContrato = reader.GetString("EstadoContrato"),
                    PagosRealizados = reader.GetInt32("PagosRealizados"),

                    InmuebleId = reader.GetInt32("InmuebleId"),
                    Direccion = reader.GetString("Direccion"),
                    TipoInmueble = reader.GetString("TipoInmueble"),
                    UsoInmueble = reader.GetString("UsoInmueble"),
                    Ambientes = reader.GetInt32("Ambientes"),
                    Coordenadas = reader.GetString("Coordenadas"),
                    EstadoInmueble = reader.GetBoolean("EstadoInmueble"),

                    PropietarioId = reader.GetInt32("PropietarioId"),
                    PropietarioIdPersona = reader.GetInt32("PropietarioIdPersona"),
                    NombrePropietario = reader.GetString("NombrePropietario"),
                    EmailPropietario = reader.GetString("EmailPropietario"),
                    TelefonoPropietario = reader.GetString("TelefonoPropietario"),

                    InquilinoId = reader.GetInt32("InquilinoId"),
                    InquilinoIdPersona = reader.GetInt32("InquilinoIdPersona"),
                    NombreInquilino = reader.GetString("NombreInquilino"),
                    EmailInquilino = reader.GetString("EmailInquilino"),
                    TelefonoInquilino = reader.GetString("TelefonoInquilino"),

                    UsuarioId = reader.GetInt32("UsuarioId"),
                    NombreEmpleado = reader.GetString("NombreEmpleado"),
                    EmailUsuario = reader.GetString("EmailUsuario"),
                    RolUsuario = reader.GetString("RolUsuario"),

                    UsuarioIdFin = reader.IsDBNull(reader.GetOrdinal("UsuarioIdFin")) ? null : reader.GetInt32("UsuarioIdFin"),
                    NombreEmpleadoFin = reader.IsDBNull(reader.GetOrdinal("NombreEmpleadoFin")) ? null : reader.GetString("NombreEmpleadoFin"),
                    EmailUsuarioFin = reader.IsDBNull(reader.GetOrdinal("EmailUsuarioFin")) ? null : reader.GetString("EmailUsuarioFin"),
                    RolUsuarioFin = reader.IsDBNull(reader.GetOrdinal("RolUsuarioFin")) ? null : reader.GetString("RolUsuarioFin")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener el contrato por ID", ex);
        }
    }

    public async Task<int> UpdateAsync(Contrato contrato)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE contratos
                SET 
                    id_inmueble = @IdInmueble,
                    id_inquilino = @IdInquilino,
                    id_usuario = @IdUsuario,
                    fecha_inicio = @FechaInicio,
                    fecha_fin = @FechaFin,
                    monto_mensual = @MontoMensual,
                    fecha_finalizacion_anticipada = @FechaFinalizacionAnticipada,
                    multa = @Multa,
                    estado = @EstadoContrato
                WHERE id_contrato = @ContratoId;
            ";

            command.Parameters.AddWithValue("@IdInmueble", contrato.InmuebleId);
            command.Parameters.AddWithValue("@IdInquilino", contrato.InquilinoId);
            command.Parameters.AddWithValue("@IdUsuario", contrato.UsuarioId);
            command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
            command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
            command.Parameters.AddWithValue("@MontoMensual", contrato.MontoMensual);
            command.Parameters.AddWithValue("@FechaFinalizacionAnticipada", contrato.FechaFinalizacionAnticipada);
            command.Parameters.AddWithValue("@Multa", (object?)contrato.Multa ?? DBNull.Value);
            command.Parameters.AddWithValue("@EstadoContrato", contrato.Estado);
            command.Parameters.AddWithValue("@ContratoId", contrato.ContratoId);

            return await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar el contrato", ex);
        }
    }

    public async Task<bool> ExisteContratoVigenteAsync(int inmuebleId, DateTime fechaInicio, DateTime fechaFin)
    {
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*) 
            FROM contratos 
            WHERE id_inmueble = @InmuebleId 
              AND estado = 'vigente'
              AND (
                  -- Verificar superposición de fechas
                  fecha_inicio <= @FechaFin AND fecha_fin >= @FechaInicio
              )
        ";

        command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
        command.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
        command.Parameters.AddWithValue("@FechaFin", fechaFin.Date);

        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        
        return count > 0;
    }

}