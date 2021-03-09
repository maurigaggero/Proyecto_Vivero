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
using Proyecto_Vivero.Server.Common;

namespace Proyecto_Vivero.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public VentasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<List<Venta>>> Get()
        {
            return await context.Ventas.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo)
                .ToListAsync();
        }

        //GET: api/ventas/filtro/cliente&empleado&fecha
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Venta>>> Get([FromQuery] string empleado, [FromQuery] string fecha)
        {
            DateTime f = Convert.ToDateTime(fecha);

            var queryable = context.Ventas.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo).AsQueryable();

            if (!string.IsNullOrEmpty(empleado))
            {
                queryable = queryable.Where(x => x.ApplicationUser.NombreyApellido.Contains(empleado));
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
            return await context.Ventas.Include(x => x.Cliente)
                .Include(x => x.ApplicationUser)
                .Include(x => x.DetalleVentas)
                .ThenInclude(x => x.Articulo)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/ventas 
        [HttpPost]
        public async Task<ActionResult<int>> Post(Venta venta)
        {
            context.Ventas.Add(venta);
            try
            {
                var userid = User.GetUserId();
                venta.EmpleadoId = userid;
                venta.Fecha = DateTime.Now;
                await context.SaveChangesAsync();

                await DecrementaStock(venta);

                if (venta.ClienteId != null)
                {
                    await IncrementaSaldo(venta);
                }
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
            return venta.Id;
        }

        // DELETE: api/ventas/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Venta>> Delete(int id)
        {
            var venta = await context.Ventas.Include(x => x.DetalleVentas)
                .FirstAsync(x => x.Id == id);

            if (venta != null)
            {
                if (venta.ClienteId != null)
                {
                    await DecrementaSaldo(venta);
                }
                await IncrementaStock(venta);

                context.Ventas.Remove(venta);
                await context.SaveChangesAsync();

                return venta;
            }
            else
            {
                return NotFound();
            }
        }

        private bool Exists(int id)
        {
            return context.Ventas.Any(e => e.Id == id);

        }

        private async Task IncrementaSaldo(Venta venta)
        {
            var cliente = await context.Clientes.FirstAsync(x => x.Id == venta.ClienteId);
            cliente.Saldo = cliente.Saldo + venta.Total;

            ClientesController c = new ClientesController(context);
            await c.Put(cliente);

            CuentasCorrientesController cc = new CuentasCorrientesController(context);
            CuentaCorriente cuenta = new CuentaCorriente()
            {
                Fecha = venta.Fecha,
                ClienteId = Convert.ToInt32(venta.ClienteId),
                ComprobanteId = venta.Id,
                Concepto = CuentaCorriente.Conceptos.Debe,
                Importe = venta.Total,
                Saldo_Parcial = venta.Cliente.Saldo
            };
            await cc.Post(cuenta);
        }

        private async Task DecrementaSaldo(Venta venta)
        {
            var cliente = await context.Clientes.FirstAsync(x => x.Id == venta.ClienteId);
            cliente.Saldo = cliente.Saldo - venta.Total;

            ClientesController c = new ClientesController(context);
            await c.Put(cliente);

            CuentasCorrientesController cc = new CuentasCorrientesController(context);
            CuentaCorriente cuenta = new CuentaCorriente()
            {
                Fecha = DateTime.Now,
                ClienteId = Convert.ToInt32(venta.ClienteId),
                Concepto = CuentaCorriente.Conceptos.Reajuste,
                Importe = -venta.Total,
                Saldo_Parcial = venta.Cliente.Saldo
            };
            await cc.Post(cuenta);
        }

        private async Task DecrementaStock(Venta venta)
        {
            var lista_articulo = await context.Articulos.ToListAsync();
            ArticulosController articulos = new ArticulosController(context);

            foreach (var detalle in venta.DetalleVentas)
            {
                var articulo = lista_articulo.First(x => x.Id == detalle.ArticuloId);
                articulo.StockActual = articulo.StockActual - detalle.Cantidad;
                await articulos.Put(articulo);
            }
        }

        private async Task IncrementaStock(Venta venta)
        {
            var lista_articulo = await context.Articulos.ToListAsync();

            ArticulosController articulos = new ArticulosController(context);
            foreach (var detalle in venta.DetalleVentas)
            {
                var articulo = lista_articulo.First(x => x.Id == detalle.ArticuloId);
                articulo.StockActual = articulo.StockActual + detalle.Cantidad;
                await articulos.Put(articulo);
            }
        }
    }
}