using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Vivero.Server.Data;
using Proyecto_Vivero.Server.Common;
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
    public class PagosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/pagos
        [HttpGet]
        public async Task<ActionResult<List<Pago>>> Get()
        {
            return await _context.Pagos.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .Include(x => x.Cliente)
                .ToListAsync();
        }

        //GET: api/pagos/filtro/empleado&fecha
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Pago>>> Get([FromQuery] string fecha)
        {
            DateTime f = Convert.ToDateTime(fecha);

            var queryable = _context.Pagos.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .AsQueryable();

            if (f != DateTime.Today.AddDays(+1))
            {
                queryable = queryable.Where(x => x.Fecha.Day == f.Day &&
                                            x.Fecha.Month == f.Month &&
                                            x.Fecha.Year == f.Year);
            }
            return await queryable.OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // GET: api/pagos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pago>> Get(int id)
        {
            return await _context.Pagos.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/pagos 
        [HttpPost]
        public async Task<ActionResult<int>> Post(Pago pago)
        {
            _context.Pagos.Add(pago);
            try
            {
                var userid = User.GetUserId();
                pago.EmpleadoId = userid;
                pago.Fecha = DateTime.Now;

                await _context.SaveChangesAsync();

                await DecrementaSaldo(pago);
            }
            catch (DbUpdateException)
            {
                if (Exists(pago.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return pago.Id;
        }

        // PUT: api/pagos
        [HttpPut]
        public async Task<ActionResult> Put(Pago pago)
        {
            _context.Entry(pago).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/pagos/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pago>> Delete(int id)
        {
            var pago = await _context.Pagos.FirstAsync(x => x.Id == id);

            if (pago != null)
            {
                await IncrementaSaldo(pago);
            }
            else
            {
                return NotFound();
            }
            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();

            return pago;
        }

        private bool Exists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }

        private async Task DecrementaSaldo(Pago pago)
        {
            var cliente = await _context.Clientes.FirstAsync(x => x.Id == pago.ClienteId);
            cliente.Saldo = cliente.Saldo - pago.Importe;

            ClientesController c = new ClientesController(_context);
            await c.Put(cliente);

            CuentasCorrientesController cc = new CuentasCorrientesController(_context);
            CuentaCorriente cuenta = new CuentaCorriente()
            {
                Fecha = pago.Fecha,
                PagoId = pago.Id,
                ClienteId = Convert.ToInt32(pago.ClienteId),
                Concepto = CuentaCorriente.Conceptos.Haber,
                Importe = pago.Importe,
                Saldo_Parcial = pago.Cliente.Saldo
            };
            await cc.Post(cuenta);
        }

        private async Task IncrementaSaldo(Pago pago)
        {
            var cliente = await _context.Clientes.FirstAsync(x => x.Id == pago.ClienteId);
            cliente.Saldo = cliente.Saldo + pago.Importe;

            ClientesController c = new ClientesController(_context);
            await c.Put(cliente);

            CuentasCorrientesController controller = new CuentasCorrientesController(_context);
            await controller.Delete("pago", pago.Id);
        }
    }
}