using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Vivero.Server.Data;
using Proyecto_Vivero.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Vivero.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasCorrientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CuentasCorrientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/cuentascorrientes
        [HttpGet]
        public async Task<ActionResult<List<CuentaCorriente>>> Get()
        {
            return await _context.CuentasCorrientes.OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // GET: api/cuentascorrientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaCorriente>> Get(int id)
        {
            return await _context.CuentasCorrientes.FirstAsync(x => x.Id == id);
        }

        // GET: api/cuentascorrientes/porcliente/5
        [HttpGet("porcliente/{idcliente}")]
        public async Task<ActionResult<List<CuentaCorriente>>> GetPorCliente(int idcliente)
        {
            return await _context.CuentasCorrientes.Where(x => x.ClienteId == idcliente)
                .OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // POST: api/cuentascorrientes 
        [HttpPost]
        public async Task<ActionResult> Post(CuentaCorriente cuenta)
        {
            _context.CuentasCorrientes.Add(cuenta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(cuenta.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }


        // PUT: api/cuentascorrientes
        [HttpPut]
        public async Task<ActionResult> Put(CuentaCorriente cuenta)
        {
            _context.Entry(cuenta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/cuentascorrientes/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<CuentaCorriente>> Delete(string borrar, int id)
        {
            CuentaCorriente cuenta = new CuentaCorriente();

            if (borrar == "pago")
            {
                cuenta = await _context.CuentasCorrientes.FirstAsync(x => x.PagoId == id);

            }
            if (borrar == "venta")
            {
                cuenta = await _context.CuentasCorrientes.FirstAsync(x => x.VentaId == id);
            }

            if (cuenta == null)
            {
                return NotFound();
            }

            _context.CuentasCorrientes.Remove(cuenta);
            await _context.SaveChangesAsync();

            return cuenta;
        }

        private bool Exists(int id)
        {
            return _context.CuentasCorrientes.Any(e => e.Id == id);
        }
    }
}