using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
	public enum rol
	{
		Empleado = 1,
		Admin = 2
	}

	public class Usuario
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Nombre { get; set; }
		[Required]
		public string Apellido { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, DataType(DataType.Password)]
		public string Clave { get; set; }
	
		public int Rol { get; set; }

		public string RolNombre => Rol > 0 ? ((rol)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(rol);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
			}
			return roles;
		}
	}
}
