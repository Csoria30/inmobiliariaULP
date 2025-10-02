using System.Data;
using MySql.Data.MySqlClient;
using inmobiliariaULP.Repositories.Interfaces;
using inmobiliariaULP.Models;
using inmobiliariaULP.Models.ViewModels;

namespace inmobiliariaULP.Repositories.Implementations;

public class PagoRepositoryImpl(IConfiguration configuration) : BaseRepository(configuration), IPagoRepository
{
    public async Task<(bool exito, string mensaje)> UpdateAsync(Pago pago)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE pagos 
                SET fecha_pago = @FechaPago, numero_pago = @NumeroPago, 
                    importe = @Importe, concepto = @Concepto, estadoPago = @EstadoPago
                WHERE id_pago = @IdPago";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdPago", pago.IdPago);
            command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
            command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
            command.Parameters.AddWithValue("@Importe", pago.Importe);
            command.Parameters.AddWithValue("@Concepto", pago.Concepto);
            command.Parameters.AddWithValue("@EstadoPago", pago.EstadoPago);

            var filasAfectadas = await command.ExecuteNonQueryAsync();

            return filasAfectadas > 0 
                ? (true, "Pago actualizado exitosamente") 
                : (false, "No se pudo actualizar el pago");
        }
        catch (MySqlException ex)
        {
            return (false, $"Error en base de datos: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Error inesperado: {ex.Message}");
        }
    }

    public async Task<(bool exito, string mensaje)> DeleteAsync(int id, int usuarioId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE pagos 
                SET estadoPago = 'anulado'
                WHERE id_pago = @IdPago AND estadoPago = 'aprobado'";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@IdPago", id);

            var filasAfectadas = await command.ExecuteNonQueryAsync();

            return filasAfectadas > 0 
                ? (true, "Pago anulado exitosamente") 
                : (false, "No se pudo anular el pago o ya estaba anulado");
        }
        catch (MySqlException ex)
        {
            return (false, $"Error en base de datos: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"Error inesperado: {ex.Message}");
        }
    }

    public async Task<int> CountByContratoAsync(int contratoId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM pagos WHERE id_contrato = @ContratoId AND estadoPago = 'aprobado'";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ContratoId", contratoId);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<(bool exito, string mensaje, Pago? pago)> AddAsync(Pago pago)
    {
        try
            {
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO pagos (
                        id_contrato, 
                        id_usuario, 
                        fecha_pago, 
                        numero_pago, 
                        importe, 
                        concepto, 
                        estadoPago
                    )
                    VALUES (
                        @IdContrato,
                        @IdUsuario, 
                        @FechaPago, 
                        @NumeroPago, 
                        @Importe, 
                        @Concepto, 
                        @EstadoPago);

                    SELECT LAST_INSERT_ID();
                ";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdContrato", pago.IdContrato);
                command.Parameters.AddWithValue("@IdUsuario", pago.IdUsuario);
                command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                command.Parameters.AddWithValue("@Importe", pago.Importe);
                command.Parameters.AddWithValue("@Concepto", pago.Concepto);
                command.Parameters.AddWithValue("@EstadoPago", pago.EstadoPago);

                var nuevoId = Convert.ToInt32(await command.ExecuteScalarAsync());
                pago.IdPago = nuevoId;

                return (true, "Pago registrado exitosamente", pago);
            }
            catch (MySqlException ex)
            {
                return (false, $"Error en base de datos: {ex.Message}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}", null);
            }
    }

    public async Task<string> GenerateNumeroAsync(int contratoId)
    {
        try
        {
            var totalPagos = await CountByContratoAsync(contratoId);
            var numeroConsecutivo = totalPagos + 1;
            
            return $"PAG-{contratoId:D4}-{numeroConsecutivo:D3}";
        }
        catch (Exception)
        {
            return $"PAG-{contratoId:D4}-001";
        }
    }

    public async Task<List<PagoDTO>> GetByContratoAsync(int contratoId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT p.id_pago, p.id_contrato, p.id_usuario, p.fecha_pago, p.numero_pago,
                        p.importe, p.concepto, p.estadoPago,
                        CONCAT(pu.nombre, ' ', pu.apellido) as nombre_usuario,
                        pu.email as email_usuario
                FROM pagos p
                INNER JOIN usuarios u ON p.id_usuario = u.id_usuario
                INNER JOIN empleados e ON u.id_empleado = e.id_empleado
                INNER JOIN personas pu ON e.id_persona = pu.id_persona
                WHERE p.id_contrato = @ContratoId AND p.estadoPago = 'aprobado'
                ORDER BY p.fecha_pago DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ContratoId", contratoId);

            var pagos = new List<PagoDTO>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                pagos.Add(new PagoDTO
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdUsuario = reader.GetInt32("id_usuario"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    NumeroPago = reader.GetString("numero_pago"),
                    Importe = reader.GetDecimal("importe"),
                    Concepto = reader.GetString("concepto"),
                    EstadoPago = reader.GetString("estadoPago"),
                    NombreUsuario = reader.GetString("nombre_usuario"),
                    EmailUsuario = reader.GetString("email_usuario")
                });
            }

            return pagos;
        }
        catch (Exception ex)
        {
            return new List<PagoDTO>();
        }
    }

    public async Task<(bool exito, string mensaje, Pago? pago)> GetByIdAsync(int id)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    id_pago, id_contrato, id_usuario, fecha_pago, numero_pago, 
                        importe, concepto, estadoPago
                FROM pagos 
                WHERE id_pago = @Id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var pago = new Pago
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdUsuario = reader.GetInt32("id_usuario"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    NumeroPago = reader.GetString("numero_pago"),
                    Importe = reader.GetDecimal("importe"),
                    Concepto = reader.GetString("concepto"),
                    EstadoPago = reader.GetString("estadoPago")
                };

                return (true, "Pago encontrado", pago);
            }

            return (false, "Pago no encontrado", null);
        }
        catch (MySqlException ex)
        {
            return (false, $"Error en base de datos: {ex.Message}", null);
        }
        catch (Exception ex)
        {
            return (false, $"Error inesperado: {ex.Message}", null);
        }
    }

    public async Task<(List<PagoDTO> pagos, int total)> GetAllAsync(int pagina = 1, int tamañoPagina = 10, string filtro = "")
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var whereClause = string.IsNullOrWhiteSpace(filtro) ? "" : 
                "WHERE (i.direccion LIKE @Filtro OR CONCAT(pi.nombre, ' ', pi.apellido) LIKE @Filtro OR p.numero_pago LIKE @Filtro)";

            // Consulta para obtener el total
            var countQuery = $@"
                SELECT COUNT(*) 
                FROM pagos p
                INNER JOIN contratos c ON p.id_contrato = c.id_contrato
                INNER JOIN inmuebles i ON c.id_inmueble = i.id_inmueble
                INNER JOIN inquilinos inq ON c.id_inquilino = inq.id_inquilino
                INNER JOIN personas pi ON inq.id_persona = pi.id_persona
                {whereClause}";

            using var countCommand = new MySqlCommand(countQuery, connection);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                countCommand.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
            }
            var total = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

            // Consulta para obtener los datos
            var query = $@"
                SELECT p.id_pago, p.id_contrato, p.id_usuario, p.fecha_pago, p.numero_pago,
                        p.importe, p.concepto, p.estadoPago,
                        i.direccion as direccion_inmueble,
                        CONCAT(pi.nombre, ' ', pi.apellido) as nombre_inquilino,
                        c.monto_mensual,
                        CONCAT(pu.nombre, ' ', pu.apellido) as nombre_usuario,
                        pu.email as email_usuario
                FROM pagos p
                INNER JOIN contratos c ON p.id_contrato = c.id_contrato
                INNER JOIN inmuebles i ON c.id_inmueble = i.id_inmueble
                INNER JOIN inquilinos inq ON c.id_inquilino = inq.id_inquilino
                INNER JOIN personas pi ON inq.id_persona = pi.id_persona
                INNER JOIN usuarios u ON p.id_usuario = u.id_usuario
                INNER JOIN empleados e ON u.id_empleado = e.id_empleado
                INNER JOIN personas pu ON e.id_persona = pu.id_persona
                {whereClause}
                ORDER BY p.fecha_pago DESC, p.id_pago DESC
                LIMIT @Offset, @Limit";

            using var command = new MySqlCommand(query, connection);
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                command.Parameters.AddWithValue("@Filtro", $"%{filtro}%");
            }
            command.Parameters.AddWithValue("@Offset", (pagina - 1) * tamañoPagina);
            command.Parameters.AddWithValue("@Limit", tamañoPagina);

            var pagos = new List<PagoDTO>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                pagos.Add(new PagoDTO
                {
                    IdPago = reader.GetInt32("id_pago"),
                    IdContrato = reader.GetInt32("id_contrato"),
                    IdUsuario = reader.GetInt32("id_usuario"),
                    FechaPago = reader.GetDateTime("fecha_pago"),
                    NumeroPago = reader.GetString("numero_pago"),
                    Importe = reader.GetDecimal("importe"),
                    Concepto = reader.GetString("concepto"),
                    EstadoPago = reader.GetString("estadoPago"),
                    DireccionInmueble = reader.GetString("direccion_inmueble"),
                    NombreInquilino = reader.GetString("nombre_inquilino"),
                    MontoMensualContrato = reader.GetDecimal("monto_mensual"),
                    NombreUsuario = reader.GetString("nombre_usuario"),
                    EmailUsuario = reader.GetString("email_usuario")
                });
            }

            return (pagos, total);
        }
        catch (Exception ex)
        {
            return (new List<PagoDTO>(), 0);
        }
    }

    public async Task<decimal> GetTotalPagadoAsync(int contratoId)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT COALESCE(SUM(importe), 0) FROM pagos WHERE id_contrato = @ContratoId AND estadoPago = 'aprobado'";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ContratoId", contratoId);

            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

   
}