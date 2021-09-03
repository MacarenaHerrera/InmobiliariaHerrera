using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class RepositorioGarante : RepositorioBase
    {
        public RepositorioGarante(IConfiguration configuration) : base(configuration)
        {

        }

        public List<Garante> Obtener()
        {
            var res = new List<Garante>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Dni, Telefono" +
                    $" FROM Garantes";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Garante i = new Garante
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Dni = reader["Dni"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int Alta(Garante i)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO garantes(Nombre, Dni, Telefono) "
                    + $"VALUES(@nombre, @dni, @telefono);" +
                    "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = res;
                    connection.Close();
                }
            }
            return res;
        }


    }
}
