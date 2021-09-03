using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato : RepositorioBase
    {

        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }
        public List<Contrato> ObtenerTodos()
        {
            var res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaCierre, c.Estado, c.InquilinoId, c.InmuebleId, c.Precio, i.Nombre, i.Apellido, inm.Direccion " +
                   $" FROM Contratos c " +
                   $"INNER JOIN Inquilinos i ON c.InquilinoId = i.Id " +
                   $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaCierre = reader.GetDateTime(2),
                            Estado = reader.GetInt32(3),
                            InquilinoId = reader.GetInt32(4),
                            InmuebleId = reader.GetInt32(5),
                            Precio = reader.GetDecimal(6),

                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(4),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
                            },

                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(5),
                                Direccion = reader.GetString(9)
                            }

                        };
                        res.Add(contrato);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int Alta(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contratos (FechaInicio, FechaCierre, Estado, InquilinoId, InmuebleId, Precio, GaranteId ) " +
                "VALUES (@fechaInicio, @fechaCierre, @estado, @inquilinoId, @inmuebleId, @precio @garanteId);" +
                "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaCierre", c.FechaCierre);
                    command.Parameters.AddWithValue("@estado", c.Estado);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@precio", c.Precio);
                    command.Parameters.AddWithValue("@precio", c.GaranteId);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    c.Id = res;
                    connection.Close();
                }
            }
            return c.Id;

        }

        public Contrato ObtenerContrato(int id)
        {
            var contrato = new Contrato();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, FechaInicio, FechaCierre, Estado, InquilinoId, InmuebleId, c.Precio, i.Nombre, i.Apellido, inm.Direccion " +
                    $" FROM Contratos c " +
                    $"INNER JOIN Inquilinos i ON c.InquilinoId = i.Id " +
                    $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id" +
                    $" WHERE c.Id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaCierre = reader.GetDateTime(2),
                            Estado = reader.GetInt32(3),
                            InquilinoId = reader.GetInt32(4),
                            InmuebleId = reader.GetInt32(5),
                            Precio = reader.GetDecimal(6),

                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(4),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
                            },

                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(5),
                                Direccion = reader.GetString(9)
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return contrato;
        }

        public int Modificar(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Contratos SET " + 
                    "FechaInicio=@fechaInicio, FechaCierre=@fechaCierre, Estado=@estado," +
                    "InquilinoId=@inquilinoId, InmuebleId=@inmuebleId, Precio=@precio " +
                    "WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaCierre", c.FechaCierre);
                    command.Parameters.AddWithValue("@estado", c.Estado);
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@precio", c.Precio);
                    command.Parameters.AddWithValue("@id",c.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();

                }
            }
            return res;
        }

        public List<Contrato> ObtenerPorInmueble(int id)
        {
            List<Contrato> lista = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.{nameof(Contrato.Id)}, {nameof(Contrato.FechaInicio)}, {nameof(Contrato.FechaCierre)}, " +
                    $"{nameof(Contrato.Estado)}, {nameof(Contrato.InmuebleId)}, {nameof(Contrato.InquilinoId)}, c.{nameof(Contrato.Precio)} " +
                    $"{nameof(Contrato.Inquilino.Nombre)}, {nameof(Contrato.Inquilino.Apellido)}, {nameof(Contrato.Inmueble.Direccion)}, " +
                    $"inm.{nameof(Contrato.Inmueble.Precio)} " +
                    $"FROM Contratos c " +
                    $"INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
                    $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
                    $"WHERE inm.Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaCierre = reader.GetDateTime(2),
                            Estado = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            InquilinoId = reader.GetInt32(5),
                            Precio = reader.GetDecimal(6),
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Precio = reader.GetDecimal(8)
                            },
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(5),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10)
                            }
                        });

                    }
                }
            }

            return lista;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Contratos WHERE Id = @id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
    }
}
