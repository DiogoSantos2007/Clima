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
        public DateTime data_registo { get; set; } = DateTime.Now;
    }
}
