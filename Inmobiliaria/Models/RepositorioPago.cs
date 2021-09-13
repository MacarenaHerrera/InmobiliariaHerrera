using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {

        }
		public int Alta(Pago entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "INSERT INTO Pagos(Fecha, Importe, ContratoId) " +
					$"VALUES(@fecha, @importe, @contratoId);" +
					"SELECT SCOPE_IDENTITY();";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@fecha", entidad.Fecha);
					command.Parameters.AddWithValue("@importe", entidad.Importe);
					command.Parameters.AddWithValue("@contratoId", entidad.ContratoId);
				
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;
					connection.Close();
				}
			}
			return entidad.Id;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Pagos WHERE {nameof(Pago.Id)} = @id;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					//command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificar(Pago entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Pagos SET " +
					"Fecha=@fecha, Importe=@importe, ContratoId=@contratoId " +
					"WHERE Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@fecha", entidad.Fecha);
					command.Parameters.AddWithValue("@importe", entidad.Importe);
					command.Parameters.AddWithValue("@contratoId", entidad.ContratoId);
					command.Parameters.AddWithValue("@id", entidad.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public IList<Pago> ObtenerPorContrato(int id)
		{
			IList<Pago> lista = new List<Pago>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT p.{nameof(Pago.Id)}, {nameof(Pago.Fecha)}, " +
					$"{nameof(Pago.Importe)}, {nameof(Pago.ContratoId)}, " +
					$"{nameof(Pago.Contrato.Inquilino.Apellido)}, {nameof(Pago.Contrato.Inquilino.Nombre)}, " +
					$"{nameof(Pago.Contrato.Inmueble.Direccion)} " +
					$"FROM Pagos p " +
					$"INNER JOIN Contratos c ON p.ContratoId = c.Id " +
					$"INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					$"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
					$"WHERE p.{nameof(Pago.ContratoId)}=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						lista.Add(new Pago
						{
							Id = reader.GetInt32(0),
							Fecha = reader.GetDateTime(1),
							Importe = reader.GetDecimal(2),
							ContratoId = reader.GetInt32(3),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(3),
								Inquilino = new Inquilino
								{
									Apellido = reader.GetString(4),
									Nombre = reader.GetString(5)
								},
								Inmueble = new Inmueble
								{
									Direccion = reader.GetString(6)
								}
							}
						});
					}
				}
			}

			return lista;
		}
		public List<Pago> ObtenerTodos()
		{
			var lista = new List<Pago>();
			
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = $"SELECT p.{nameof(Pago.Id)}, {nameof(Pago.Fecha)}, " +
					$"{nameof(Pago.Importe)}, {nameof(Pago.ContratoId)}, " +
					$"{nameof(Pago.Contrato.Inquilino.Apellido)}, {nameof(Pago.Contrato.Inquilino.Nombre)}, " +
					$"{nameof(Pago.Contrato.Inmueble.Direccion)} " +
					$"FROM Pagos p " +
					$"INNER JOIN Contratos c ON p.ContratoId = c.Id " +
					$"INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					$"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						lista.Add(new Pago
						{
							Id = reader.GetInt32(0),
							Fecha = reader.GetDateTime(1),
							Importe = reader.GetDecimal(2),
							ContratoId = reader.GetInt32(3),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(3),
								Inquilino = new Inquilino
								{
									Apellido = reader.GetString(4),
									Nombre = reader.GetString(5)
								},
								Inmueble = new Inmueble
								{
									Direccion = reader.GetString(6)
								}
							}
						});
					}
				}
			}

			return lista;
		}

		public Pago ObtenerPorId(int id)
		{
			Pago entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT p.{nameof(Pago.Id)}, {nameof(Pago.Fecha)}, " +
					$"{nameof(Pago.Importe)}, {nameof(Pago.ContratoId)}, " +
					$"{nameof(Pago.Contrato.Inquilino.Apellido)}, {nameof(Pago.Contrato.Inquilino.Nombre)}, " +
					$"{nameof(Pago.Contrato.Inmueble.Direccion)}, {nameof(Pago.Contrato.FechaInicio)} " +
					$"FROM Pagos p " +
					$"INNER JOIN Contratos c ON p.ContratoId = c.Id " +
					$"INNER JOIN Inquilinos inq ON c.InquilinoId = inq.Id " +
					$"INNER JOIN Inmuebles inm ON c.InmuebleId = inm.Id " +
					$"WHERE p.{nameof(Pago.Id)}=@id;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Pago
						{
							Id = reader.GetInt32(0),
							Fecha = reader.GetDateTime(1),
							Importe = reader.GetDecimal(2),
							ContratoId = reader.GetInt32(3),

							Contrato = new Contrato
							{
								Id = reader.GetInt32(3),

							Inquilino = new Inquilino
							{
									Apellido = reader.GetString(4),
									Nombre = reader.GetString(5)
							},
							Inmueble = new Inmueble
							{
							Direccion = reader.GetString(6)
							},
							FechaInicio = reader.GetDateTime(7)
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}
	}
}
