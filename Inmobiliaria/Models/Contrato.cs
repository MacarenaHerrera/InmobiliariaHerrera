using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public enum estadoContrato
    {
        Vigente = 1,
        Cancelado = 2
    }

    public class Contrato
    {

        [Display(Name = "Código")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Fecha Inicio"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Fecha Cierre"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        public DateTime FechaCierre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        public int Estado { get; set; }
        public string EstadoInmueble => ((estadoContrato)Estado).ToString();

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

        [Required(ErrorMessage = "Campo obligatorio"),
        ForeignKey(nameof(GaranteId))]
        [Display(Name = "Dato Garante")]
        public int GaranteId { get; set; }
              
        [Display(Name = "Inmueble")]
        public Inmueble Inmueble { get; set; }

        [Display(Name = "Inquilino")]
        public Inquilino Inquilino { get; set; }


        [Display(Name = "Garante")]
        public Garante Garante { get; set; }

        public string EstadoNombre => Estado > 0 ? ((estadoContrato)Estado).ToString() : "";

        public static IDictionary<int, string> ObtenerEstados()
        {
            SortedDictionary<int, string> estados = new SortedDictionary<int, string>();
            Type tipoEnumEstado = typeof(estadoContrato);
            foreach (var valor in Enum.GetValues(tipoEnumEstado))
            {
                estados.Add((int)valor, Enum.GetName(tipoEnumEstado, valor));
            }
            return estados;
        }

        [Display(Name = "Multa Cancelación")]
        [DataType(DataType.Currency)]
        public decimal CalcularMulta() => FechaCierre.Subtract(DateTime.Now).TotalDays < (FechaCierre.Subtract(FechaInicio).TotalDays / 2) ? Precio : (Precio * 2);

        public int CalcularMeses() => (int)DateTime.Now.Subtract(FechaInicio).TotalDays / 30;

        [DisplayName("Vigente")]
        public bool EsVigente => (DateTime.Now <= FechaCierre && FechaInicio <= DateTime.Now);
        
    }
}
