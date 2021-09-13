using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{
            
        }

		public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "INSERT INTO Inmuebles(Direccion, Ambientes, Superficie, PropietarioId, Tipo, Precio, Disponible) " +
					$"VALUES(@direccion, @ambientes, @superficie, @propietarioId, @tipoInmueble, @precio, @disponible);" +
					"SELECT SCOPE_IDENTITY();";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@tipoInmueble", entidad.TipoInmueble);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					command.Parameters.AddWithValue("@disponible", entidad.Disponible ? 1 : 0);
					
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
				string sql = $"DELETE FROM Inmuebles WHERE {nameof(Inmueble.Id)} = @id;";
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
		public int Modificar(Inmueble entidad)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmuebles SET " +
					"Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, PropietarioId=@propietarioId, Tipo=@tipo, Precio=@precio, Disponible=@disponible " +
					"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@tipo", entidad.TipoInmueble);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					command.Parameters.AddWithValue("@disponible", entidad.Disponible ? 1 : 0);
					command.Parameters.AddWithValue("@id", entidad.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public List<Inmueble> ObtenerTodos()
		{
			var res = new List<Inmueble>();
			//IList<Inmueble> res = new List<Inmueble>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				
				string sql = "SELECT i.Id, Direccion, Ambientes, Superficie, PropietarioId, Tipo, i.Precio, Disponible, " +
					"p.Nombre, p.Apellido" +
					" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id ";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble

						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							PropietarioId = reader.GetInt32(4),
							TipoInmueble = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Disponible = reader.GetByte(7) == 1,
							Duenio = new Propietario
							{
								Id = reader.GetInt32(4),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9)
							},

						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, Ambientes, Superficie, PropietarioId, Tipo, " +
					$"i.Precio, Disponible, p.Nombre, p.Apellido " +
					$"FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id" +
					$" WHERE i.Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							PropietarioId = reader.GetInt32(4),
							TipoInmueble = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Disponible = reader.GetByte(7) == 1,

							Duenio = new Propietario
							{
								Id = reader.GetInt32(4),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9)
							},
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}

		public IList<Inmueble> ObtenerDisponibles()
		{
			IList<Inmueble> lista = new List<Inmueble>();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, Ambientes, Superficie, PropietarioId, Tipo, i.Precio, Disponible, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id" +					
					$" WHERE i.Disponible = 1;";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						lista.Add(new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							PropietarioId = reader.GetInt32(4),
							TipoInmueble = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Disponible = reader.GetByte(7) == 1,

							Duenio = new Propietario
							{
								Id = reader.GetInt32(4),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						}); 
					}
				}
			}

			return lista;
		}


		public List<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.Id, Direccion, Ambientes, Superficie, PropietarioId, Tipo, Precio, Disponible, p.Nombre, p.Apellido" +
					$" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.Id" +
					$" WHERE i.PropietarioId=@idPropietario";

				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							PropietarioId = reader.GetInt32(4),
							TipoInmueble = reader.GetString(5),
							Precio = reader.GetDecimal(6),
							Disponible = reader.GetByte(7) == 1,

							Duenio = new Propietario
							{
								Id = reader.GetInt32(4),
								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9)
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
