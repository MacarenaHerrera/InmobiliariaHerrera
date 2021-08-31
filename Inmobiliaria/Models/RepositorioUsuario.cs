using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioUsuario : RepositorioBase
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario u)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Usuarios " +
                    $"({nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, " +
                    $"{nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, " +
                    $"{nameof(Usuario.Rol)}) " +
                    $"VALUES (@nombre, @apellido, @email, @clave, @rol);" +
                    "SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@apellido", u.Apellido);
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    u.Id = res;
                    connection.Close();
                }

            }
            return u.Id;
        }

        public int Baja(int id)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Usuarios WHERE {nameof(Usuario.Id)} = @id;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }

            }
            return res;
        }

        public int Modificacion(Usuario u)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Usuarios SET " +
                    $"{nameof(Usuario.Nombre)}=@nombre, " +
                    $"{nameof(Usuario.Apellido)}=@apellido, " +
                    $"{nameof(Usuario.Email)}=@email, " +
                    $"{nameof(Usuario.Clave)}=@clave, " +
                    $"{nameof(Usuario.Rol)}=@rol " +
                    $"WHERE {nameof(Usuario.Id)}=@id;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", u.Nombre);
                    command.Parameters.AddWithValue("@apellido", u.Apellido);
                    command.Parameters.AddWithValue("@email", u.Email);
                    command.Parameters.AddWithValue("@clave", u.Clave);
                    command.Parameters.AddWithValue("@rol", u.Rol);
                    command.Parameters.AddWithValue("@id", u.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public Usuario Obtener(int id)
        {
            Usuario res = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT {nameof(Usuario.Id)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, " +
                    $"{nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Rol)} " +
                    $"FROM Usuarios WHERE {nameof(Usuario.Id)}=@id;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Email = reader.GetString(3),
                            Clave = reader.GetString(4),
                            Rol = reader.GetInt32(5)
                        };
                    }
                    connection.Close();
                }
            }

            return res;
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario res = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT {nameof(Usuario.Id)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, " +
                    $"{nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Rol)} " +
                    $"FROM Usuarios WHERE {nameof(Usuario.Email)}=@email;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Email = reader.GetString(3),
                            Clave = reader.GetString(4),
                            Rol = reader.GetInt32(5)
                        };
                    }
                    connection.Close();
                }
            }

            return res;
        }

        public IList<Usuario> ObtenerTodos()
        {
            IList<Usuario> lista = new List<Usuario>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT {nameof(Usuario.Id)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, " +
                    $"{nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.Rol)} " +
                    $"FROM Usuarios;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Email = reader.GetString(3),
                            Clave = reader.GetString(4),
                            Rol = reader.GetInt32(5)
                        });
                    }
                }
            }

            return lista;
        }
    }
}
