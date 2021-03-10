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
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ComprasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //GET: api/compras
        [HttpGet]
        public async Task<ActionResult<List<Compra>>> Get()
        {
            return await context.Compras.Include(x => x.ApplicationUser)
                .Include(x => x.DetalleCompras)
                .ThenInclude(x => x.Articulo)
                .ToListAsync();
        }

        //GET: api/compras/filtro/cliente&empleado&fecha
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Compra>>> Get([FromQuery] string empleado, [FromQuery] string fecha)
        {
            DateTime f = Convert.ToDateTime(fecha);

            var queryable = context.Compras.Include(x => x.ApplicationUser)
                .Include(x => x.DetalleCompras)
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

        // GET: api/compras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> Get(int id)
        {
            return await context.Compras.Include(x => x.ApplicationUser)
                .Include(x => x.DetalleCompras)
                .ThenInclude(x => x.Articulo)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/compras 
        [HttpPost]
        public async Task<ActionResult<int>> Post(Compra compra)
        {
            context.Compras.Add(compra);
            try
            {
                var userid = User.GetUserId();
                compra.EmpleadoId = userid;
                compra.Fecha = DateTime.Now;
                await context.SaveChangesAsync();

                await IncrementaStock(compra);
            }
            catch (DbUpdateException)
            {
                if (Exists(compra.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return compra.Id;
        }

        // DELETE: api/compras/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Compra>> Delete(int id)
        {
            var compra = await context.Compras.Include(x => x.DetalleCompras)
                .FirstAsync(x => x.Id == id);

            if (compra != null)
            {
                await DecrementaStock(compra);

                context.Compras.Remove(compra);
                await context.SaveChangesAsync();

                return compra;
            }
            else
            {
                return NotFound();
            }
        }

        private bool Exists(int id)
        {
            return context.Compras.Any(e => e.Id == id);

        }

        private async Task DecrementaStock(Compra compra)
        {
            var lista_articulo = await context.Articulos.ToListAsync();
            ArticulosController articulos = new ArticulosController(context);

            foreach (var detalle in compra.DetalleCompras)
            {
                var articulo = lista_articulo.First(x => x.Id == detalle.ArticuloId);
                articulo.StockActual = articulo.StockActual - detalle.Cantidad;
                articulo.PrecioMayorista = detalle.PrecioMayorista;
                articulo.PrecioUnitario = detalle.PrecioUnitario;
                await articulos.Put(articulo);
            }
        }

        private async Task IncrementaStock(Compra compra)
        {
            var lista_articulo = await context.Articulos.ToListAsync();

            ArticulosController articulos = new ArticulosController(context);
            foreach (var detalle in compra.DetalleCompras)
            {
                var articulo = lista_articulo.First(x => x.Id == detalle.ArticuloId);
                articulo.StockActual = articulo.StockActual + detalle.Cantidad;
                articulo.PrecioMayorista = detalle.PrecioMayorista;
                articulo.PrecioUnitario = detalle.PrecioUnitario;
                await articulos.Put(articulo);
            }
        }
    }
}