using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inquilino
	{
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Campo obligatorio"),
             MaxLength(50, ErrorMessage = "Máximo 50 caracteres"),
            MinLength(4, ErrorMessage = "El nombre debe ser de 4 caracteres minimamente")]
        
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres"),
             MinLength(4, ErrorMessage = "El apellido debe ser de 4 caracteres minimamente")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            MinLength(8, ErrorMessage = "Un DNI debe tener mínimo 8 dígitos")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
            MinLength(6, ErrorMessage = "Un número de teléfono debe tener mínimo 6 dígitos")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            EmailAddress(ErrorMessage = "Debe ser una dirección de correo válida"),
            MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Email { get; set; }
    }
}
