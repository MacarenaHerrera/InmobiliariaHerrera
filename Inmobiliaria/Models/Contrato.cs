using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"), 
        Display(Name = "Fecha de inicio"),
        DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        Display(Name = "Fecha de cierre"),
        DataType(DataType.Date)]
        public DateTime FechaCierre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public int Estado { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(InmuebleId))]
        [Display(Name = "Dato Inmueble")]
        public int InmuebleId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(InmuebleId))]
        [Display(Name = "Dato Inquilino")]
        public int InquilinoId { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public decimal Precio { get; set; }

        [Display(Name = "Inquilino")]
        public Inquilino Inquilino { get; set; }

        [Display(Name = "Inmueble")]
        public Inmueble Inmueble { get; set; }

       
    }
}
