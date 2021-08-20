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
        MaxLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        StringLength(2, MinimumLength = 1, ErrorMessage = "Ingrese un numero válido")]
        public int Ambientes { get; set; }
        
        
        [Required(ErrorMessage = "Campo obligatorio"),
        StringLength(8, MinimumLength = 1, ErrorMessage = "Ingrese una superficie válida")]
        public int Superficie { get; set; }
       
        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(PropietarioId))]
        public int PropietarioId { get; set; }

        [Display(Name = "Tipo")]
        public string TipoInmueble { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        StringLength(8, MinimumLength = 1, ErrorMessage = "Ingrese un precio válido")]
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }

        [Display(Name = "Dueño")]
        public Propietario Duenio { get; set; }
    }
}
