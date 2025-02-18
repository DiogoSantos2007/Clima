using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Clima.Models
{
    public class Tb_registos
    {
        [Key]
        public int ID_registo { get; set; }
        public int temperatura { get; set; }
        public int humidade { get; set; }
        public int risco_temperatura { get; set; }
        public int risco_humidade { get; set; }
        public DateTime data_registo { get; set; } = DateTime.Now;
    }
}
