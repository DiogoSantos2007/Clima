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
        public async Task<IActionResult> InsertValues([FromBody] Tb_registos climaData)
        {
            try
            {
                Console.WriteLine("Novo Registo");
                Console.WriteLine($"Temperatura: {climaData.temperatura}");
                Console.WriteLine($"Humidade: {climaData.humidade}");
                Console.WriteLine($"Risco Temperatura: {climaData.risco_temperatura}");
                Console.WriteLine($"Risco Humidade: {climaData.risco_humidade}");


                await appDbContext.Tb_Registos.AddAsync(climaData);
                await appDbContext.SaveChangesAsync();
                return Ok("Sucesso");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }


    }
}
