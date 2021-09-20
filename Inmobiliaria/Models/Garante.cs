using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Garante
    {
        [Key]
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "Campo obligatorio"),
        MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            MinLength(8, ErrorMessage = "Un DNI debe tener dígitos")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
            MinLength(6, ErrorMessage = "Un número de teléfono debe tener mínimo 6 dígitos")]
        public string Telefono { get; set; }
    }
}
