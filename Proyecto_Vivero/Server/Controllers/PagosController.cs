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
                .Include(x => x.Empleado)
                .Include(x => x.Cliente)
                .ToListAsync();
        }

        //GET: api/pagos/filtro/cliente&empleado&fecha
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Pago>>> Get([FromQuery] string fecha)
        {
            DateTime f = Convert.ToDateTime(fecha);

            var queryable = _context.Pagos.Include(x => x.Cliente)
                .Include(x => x.Empleado)
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
                .Include(x => x.Empleado)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/pagos 
        [HttpPost]
        public async Task<ActionResult<int>> Post(Pago pago)
        {
            _context.Pagos.Add(pago);
            try
            {
                pago.Fecha = DateTime.Now;

                await ActualizaCliente(pago, "create");

                await _context.SaveChangesAsync();
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

            CuentasCorrientesController controller = new CuentasCorrientesController(_context);
            CuentaCorriente cuenta = new CuentaCorriente()
            {
                Fecha = pago.Fecha,
                PagoId = pago.Id,
                ClienteId = Convert.ToInt32(pago.ClienteId),
                Concepto = CuentaCorriente.Conceptos.Haber,
                Importe = -pago.Importe,
                Saldo_Parcial = pago.Cliente.Saldo
            };
            await controller.Post(cuenta);

            return pago.Id;
        }

        public async Task ActualizaCliente(Pago pago, string accion)
        {
            ClientesController cliente = new ClientesController(_context);
            var c = await cliente.Get();

            switch (accion)
            {
                case "create":
                    for (int i = 0; i < c.Value.Count; i++)
                    {
                        if (c.Value[i].Id == pago.ClienteId)
                        {
                            var newsaldo = c.Value[i].Saldo - pago.Importe;

                            await cliente.PutSaldo(c.Value[i], newsaldo);
                        }
                    }
                    break;
                case "delete":
                    for (int i = 0; i < c.Value.Count; i++)
                    {
                        if (c.Value[i].Id == pago.ClienteId)
                        {
                            var newsaldo = c.Value[i].Saldo + pago.Importe;

                            await cliente.PutSaldo(c.Value[i], newsaldo);
                        }
                    }
                    break;
            }
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

            CuentasCorrientesController controller = new CuentasCorrientesController(_context);
            await controller.Delete("pago", pago.Id);

            if (pago == null)
            {
                return NotFound();
            }

            await ActualizaCliente(pago, "delete");

            _context.Pagos.Remove(pago);

            await _context.SaveChangesAsync();

            return pago;
        }

        private bool Exists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}