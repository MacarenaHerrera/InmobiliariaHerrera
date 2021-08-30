using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Display(Name = "Código")]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        MinLength(5, ErrorMessage = "Mínimo 5 caracteres para dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public int Ambientes { get; set; }


        [Required(ErrorMessage = "Campo obligatorio")]
        public int Superficie { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(PropietarioId))]
        [Display(Name = "Propietario")]
        public int PropietarioId { get; set; }


        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "Campo obligatorio")]
        public string TipoInmueble { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }

        [Display(Name = "Dueño")]
        public Propietario Duenio { get; set; }
    }
}
