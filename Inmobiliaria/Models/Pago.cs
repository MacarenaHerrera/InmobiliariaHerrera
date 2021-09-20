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

        [Required, DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
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
        public int NumeroPago { get; set; }

        public int ObtenerPago() {

            if (Fecha.Month < Contrato.FechaInicio.Month)
            {
                int num = 1 + (Fecha.Year - Contrato.FechaInicio.Year) * 12 +
                   (-1 * (Fecha.Month - Contrato.FechaInicio.Month));
                return num;
            }
            else
                
            {
                int num = 1 + (Fecha.Year - Contrato.FechaInicio.Year) * 12 +
                   (Fecha.Month - Contrato.FechaInicio.Month);
                return num;

            }
        }
    }
}

