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
            List<Contrato> lista = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaCierre, c.Estado, c.InmuebleId, c.InquilinoId, " +
                   $"c.Precio, c.GaranteId, inm.Direccion, i.Nombre, i.Apellido, g.Nombre " +
                   $"FROM Contratos c " +
                   $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
                   $"INNER JOIN Inquilinos i ON c.InquilinoId = i.Id " +
                   $"INNER JOIN Garantes g ON c.GaranteId = g.Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
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
                            GaranteId = reader.GetInt32(7),

                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(8)
                            },

                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(5),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10)
                            },

                            Garante = new Garante
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(11)
                            }
                        });
                    }
                }
            }

            return lista;
        }

        public int Alta(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contratos(FechaInicio, FechaCierre, Estado, InmuebleId, InquilinoId, Precio, GaranteId) " +
                "VALUES (@fechaInicio, @fechaCierre, @estado, @inmuebleId, @inquilinoId, @precio, @garanteId);" +
                "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaInicio", c.FechaInicio);
                    command.Parameters.AddWithValue("@fechaCierre", c.FechaCierre);
                    command.Parameters.AddWithValue("@estado", c.Estado);
                    command.Parameters.AddWithValue("@inmuebleId", c.InmuebleId);
                    command.Parameters.AddWithValue("@inquilinoId", c.InquilinoId);
                    command.Parameters.AddWithValue("@precio", c.Precio);
                    command.Parameters.AddWithValue("@garanteId", c.GaranteId);

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
            Contrato contrato = new Contrato();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, FechaInicio, FechaCierre, Estado, InmuebleId, InquilinoId, c.Precio, GaranteId, inm.Direccion, i.Nombre, i.Apellido, g.Nombre " +
                    $" FROM Contratos c " +
                    $"INNER JOIN Inquilinos i ON c.InquilinoId = i.Id " +
                    $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
                    $"INNER JOIN Garantes g ON c.GaranteId = g.Id" +
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
                            InmuebleId = reader.GetInt32(4),
                            InquilinoId = reader.GetInt32(5),
                            Precio = reader.GetDecimal(6),
                            GaranteId = reader.GetInt32(7),

                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(8)
                            },

                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(5),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10)
                            },

                            Garante = new Garante
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(11)
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
                    "InmuebleId=@inmuebleId, InquilinoId=@inquilinoId, Precio=@precio, GaranteId=@garanteId " +
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
                    command.Parameters.AddWithValue("@garanteId", c.GaranteId);
                    command.Parameters.AddWithValue("@id",c.Id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();

                }
            }
            return res;
        }

        public int Renovar(Contrato c)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Contratos SET " +
                    "FechaCierre=@fechaCierre, " +
                    "Precio=@precio " +
                    "WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaCierre", c.FechaCierre);
                    command.Parameters.AddWithValue("@precio", c.Precio);
                    command.Parameters.AddWithValue("@id", c.Id);
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
                    $"{nameof(Contrato.Estado)}, {nameof(Contrato.InmuebleId)}, {nameof(Contrato.InquilinoId)}, c.{nameof(Contrato.Precio)}, c.{nameof(Contrato.GaranteId)}, " +
                    $"inm.{nameof(Contrato.Inmueble.Direccion)}, " +
                    $"i.{nameof(Contrato.Inquilino.Nombre)}, i.{nameof(Contrato.Inquilino.Apellido)},  " +
                    $"g.{nameof(Contrato.Garante.Nombre)} " +
                    $"FROM Contratos c " +
                    $"INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
                    $"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
                    $"INNER JOIN Garantes g ON c.InmuebleId = g.Id " +
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
                            GaranteId = reader.GetInt32(7),

                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(8)
                            },

                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(5),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10)
                            },

                            Garante = new Garante
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(11)
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
