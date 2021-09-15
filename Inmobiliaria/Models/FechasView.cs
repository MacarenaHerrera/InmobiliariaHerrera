using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class FechasView
    {
        [Required]
        [DisplayName("Fecha Inicio"), DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Required]
        [DisplayName("Fecha Cierre"), DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaCierre { get; set; }

        public String Hoy(){
            DateTime fecha = DateTime.Now;
            int anio = fecha.Year;
            int mes = fecha.Month;
            int dia = fecha.Day;
            String fechaHoy = (anio / mes / dia).ToString();
            return fechaHoy;
        }
        }
}
