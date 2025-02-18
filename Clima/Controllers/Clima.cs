using Clima.Data;
using Clima.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Clima.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Clima(AppDbContext appDbContext) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> InsertValues(int temperatura, int humidade, int risco_temperatura, int risco_humidade)
        {
            try
            {
                Tb_registos novo = new()
                {
                    temperatura = temperatura,
                    humidade = humidade,
                    risco_humidade = risco_humidade,
                    risco_temperatura = risco_temperatura,
                };
                await appDbContext.Tb_Registos.AddAsync(novo);
                await appDbContext.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}
