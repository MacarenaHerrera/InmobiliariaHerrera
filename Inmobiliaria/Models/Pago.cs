using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        Display(Name = "Fecha"),
        DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public decimal Importe { get; set; }

        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(ContratoId))]
        [Display(Name = "Datos Contrato")]
        public int ContratoId { get; set; }

        [Display(Name = "Contrato")]
        public Contrato Contrato { get; set; }

        [Display(Name = "Número de Pago")]
        public int NumeroPago => 1 + Fecha.Month - (Contrato != null ? Contrato.FechaInicio.Month : Fecha.Month) +
            (Fecha.Year - (Contrato != null ? Contrato.FechaInicio.Year : Fecha.Year)) * 12;
    }
}
