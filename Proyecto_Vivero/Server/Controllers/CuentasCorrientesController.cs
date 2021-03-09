using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasCorrientesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public CuentasCorrientesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //GET: api/cuentascorrientes
        [HttpGet]
        public async Task<ActionResult<List<CuentaCorriente>>> Get()
        {
            return await context.CuentasCorrientes.OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // GET: api/cuentascorrientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaCorriente>> Get(int id)
        {
            return await context.CuentasCorrientes.FirstAsync(x => x.Id == id);
        }

        // GET: api/cuentascorrientes/porcliente/5
        [HttpGet("porcliente/{idcliente}")]
        public async Task<ActionResult<List<CuentaCorriente>>> GetPorCliente(int idcliente)
        {
            return await context.CuentasCorrientes.Where(x => x.ClienteId == idcliente)
                .OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // POST: api/cuentascorrientes 
        [HttpPost]
        public async Task<ActionResult> Post(CuentaCorriente cuenta)
        {
            context.CuentasCorrientes.Add(cuenta);
            try
            {
                await context.SaveChangesAsync();
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

        private bool Exists(int id)
        {
            return context.CuentasCorrientes.Any(e => e.Id == id);
        }
    }
}