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
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<List<Venta>>> Get()
        {
            return await _context.Ventas.Include(x => x.Cliente)
                .Include(x => x.Empleado)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo)
                .ToListAsync();
        }

        //GET: api/ventas/filtro/cliente&empleado&fecha
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Venta>>> Get([FromQuery] string cliente, [FromQuery] string empleado, [FromQuery] string fecha)
        {
            DateTime f = Convert.ToDateTime(fecha);

            var queryable = _context.Ventas.Include(x => x.Cliente)
                .Include(x => x.Empleado)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo).AsQueryable();

            if (!string.IsNullOrEmpty(cliente))
            {
                queryable = queryable.Where(x => x.Cliente.NombreyApellido.Contains(cliente));
            }
            if (!string.IsNullOrEmpty(empleado))
            {
                queryable = queryable.Where(x => x.Empleado.NombreyApellido.Contains(empleado));
            }
            if (f != DateTime.Today.AddDays(+1))
            {
                queryable = queryable.Where(x => x.Fecha.Day == f.Day &&
                                            x.Fecha.Month == f.Month &&
                                            x.Fecha.Year == f.Year);
            }

            return await queryable.OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // GET: api/ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> Get(int id)
        {
            return await _context.Ventas.Include(x => x.Cliente)
                .Include(x => x.Empleado)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/ventas 
        [HttpPost]
        public async Task<ActionResult<int>> Post(Venta venta)
        {
            _context.Ventas.Add(venta);
            try
            {
                venta.Fecha = DateTime.Now;

                await ActualizaStock(venta, "create");

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(venta.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            if (venta.ClienteId != null)
            {
                await ActualizaCliente(venta, "create");

                CuentasCorrientesController controller = new CuentasCorrientesController(_context);
                CuentaCorriente cuenta = new CuentaCorriente()
                {
                    Fecha = venta.Fecha,
                    VentaId = venta.Id,
                    ClienteId = Convert.ToInt32(venta.ClienteId),
                    Concepto = CuentaCorriente.Conceptos.Debe,
                    Importe = venta.Total,
                    Saldo_Parcial = venta.Cliente.Saldo
                };
                await controller.Post(cuenta);
            }
            return venta.Id;
        }

        public async Task ActualizaCliente(Venta venta, string accion)
        {
            ClientesController cliente = new ClientesController(_context);
            var c = await cliente.Get();

            switch (accion)
            {
                case "create":
                    for (int i = 0; i < c.Value.Count; i++)
                    {
                        if (c.Value[i].Id == venta.ClienteId)
                        {
                            var newsaldo = c.Value[i].Saldo + venta.Total;

                            await cliente.PutSaldo(c.Value[i], newsaldo);
                        }
                    }
                    break;
                case "delete":
                    for (int i = 0; i < c.Value.Count; i++)
                    {
                        if (c.Value[i].Id == venta.ClienteId)
                        {
                            var newsaldo = c.Value[i].Saldo - venta.Total;

                            await cliente.PutSaldo(c.Value[i], newsaldo);
                        }
                    }
                    break;
            }
        }

        public async Task ActualizaStock(Venta venta, string accion)
        {
            ArticulosController articulos = new ArticulosController(_context);

            var a = await articulos.Get();

            switch (accion)
            {
                case "create":
                    for (int i = 0; i < venta.DetalleVentas.Count; i++)
                    {
                        for (int j = 0; j < a.Value.Count; j++)
                        {
                            if (a.Value[j].Id == venta.DetalleVentas[i].ArticuloId)
                            {
                                var newstock = a.Value[j].StockActual - venta.DetalleVentas[i].Cantidad;

                                await articulos.PutStock(a.Value[j], newstock);
                            }
                        }
                    }
                    break;

                case "delete":
                    for (int i = 0; i < venta.DetalleVentas.Count; i++)
                    {
                        for (int j = 0; j < a.Value.Count; j++)
                        {
                            if (a.Value[j].Id == venta.DetalleVentas[i].ArticuloId)
                            {
                                var newstock = a.Value[j].StockActual + venta.DetalleVentas[i].Cantidad;

                                await articulos.PutStock(a.Value[j], newstock);
                            }
                        }
                    }
                    break;
            }
        }

        // PUT: api/ventas
        [HttpPut]
        public async Task<ActionResult> Put(Venta venta)
        {
            _context.Entry(venta).State = EntityState.Modified;

            foreach (var detalle in venta.DetalleVentas)
            {
                if (detalle.Id != 0)
                {
                    //_context.Entry(detalle).State = EntityState.Modified; //en desarrollo
                }
                else
                {
                    _context.Entry(detalle).State = EntityState.Added;

                    await ActualizaStock(venta, "create");
                }
            }
            var listadodetalle_ids = venta.DetalleVentas.Select(x => x.Id).ToList();
            var detallesborrar = await _context
                .DetalleVentas
                .Where(x => !listadodetalle_ids.Contains(x.Id) && x.VentaId == venta.Id)
                .ToListAsync();


            _context.RemoveRange(detallesborrar);
            //actualiza stock de borrado
            ArticulosController articulos = new ArticulosController(_context);
            var a = await articulos.Get();
            for (int i = 0; i < detallesborrar.Count; i++)
            {
                for (int j = 0; j < a.Value.Count; j++)
                {
                    if (a.Value[j].Id == detallesborrar[i].ArticuloId)
                    {
                        var newstock = a.Value[j].StockActual + detallesborrar[i].Cantidad;

                        await articulos.PutStock(a.Value[j], newstock);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/ventas/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Venta>> Delete(int id)
        {
            var venta = await _context.Ventas.Include(x => x.DetalleVentas)
                .FirstAsync(x => x.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            if (venta.ClienteId != null)
            {
                await ActualizaCliente(venta, "delete");

                CuentasCorrientesController controller = new CuentasCorrientesController(_context);
                await controller.Delete("venta", venta.Id);
            }

            await ActualizaStock(venta, "delete");

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return venta;
        }

        private bool Exists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}