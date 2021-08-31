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

		public List<Pago> ObtenerTodos()
		{
			var res = new List<Pago>();
			
			using (SqlConnection connection = new SqlConnection(connectionString))
			{

				string sql = "SELECT p.Id, Fecha, Importe, ContratoId, " +
					"c.InmuebleId, c.InquilinoId"+
					" FROM Pagos p INNER JOIN Contratos c ON p.ContratoId = c.Id ";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pago entidad = new Pago

						{
							Id = reader.GetInt32(0),
							Fecha = reader.GetDateTime(1),
							Importe = reader.GetDecimal(2),
							ContratoId = reader.GetInt32(3),
							Contrato = new Contrato
							{
								Id = reader.GetInt32(3),
								InmuebleId = reader.GetInt32(4),
								InquilinoId = reader.GetInt32(5),
							},

						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Pago ObtenerPorId(int id)
		{
			Pago entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT p.Id, Fecha, Importe, ContratoId, " + $"c.InmuebleId, c.InquilinoId" +
					$" FROM Pagos p INNER JOIN Contratos c ON p.ContratoId = c.Id " +
					$" WHERE p.Id=@id";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
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
								InmuebleId = reader.GetInt32(4),
								InquilinoId = reader.GetInt32(5),
							},
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}
	}
}
