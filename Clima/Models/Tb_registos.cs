using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clima.Models
{
    public class Tb_registos
    {
        [Key]
        public int ID_registo { get; set; }
        public double temperatura { get; set; }
        public double humidade { get; set; }
        public int risco_temperatura { get; set; }
        public int risco_humidade { get; set; }
        public double risco_incendio { get; set; }
        public double radiacao { get; set; }
        public double humidade_solo { get; set; }
        public int risco_humidade_solo { get; set; }
        public DateTime data_registo { get; set; } = DateTime.Now;
    }
}
