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
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public PedidosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //GET: api/pedidos
        [HttpGet]
        public async Task<ActionResult<List<Pedido>>> Get()
        {
            return await context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.DetallePedidos)
                .ThenInclude(x => x.Articulo)
                .ToListAsync();
        }

        //GET: api/pedidos/filtro/nombre&categoria
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Pedido>>> Get([FromQuery] string cliente, [FromQuery] string articulo)
        {
            var queryable = context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.DetallePedidos)
                .ThenInclude(x => x.Articulo).AsQueryable();

            if (!string.IsNullOrEmpty(cliente))
            {
                queryable = queryable.Where(x => x.Cliente.NombreyApellido.Contains(cliente));
            }

            if (!string.IsNullOrEmpty(articulo))
            {
                queryable = queryable.Where(x => x.DetallePedidos
                .Any(x => x.Articulo.Nombre.Contains(articulo)));
            }
            return await queryable.OrderByDescending(x => x.Fecha).ToListAsync();
        }

        // GET: api/pedidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> Get(int id)
        {
            return await context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.DetallePedidos)
                .ThenInclude(x => x.Articulo)
                .FirstAsync(x => x.Id == id);
        }

        // POST: api/pedidos 
        [HttpPost]
        public async Task<ActionResult> Post(Pedido pedido)
        {
            context.Pedidos.Add(pedido);
            try
            {
                pedido.Fecha = DateTime.Now;
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(pedido.Id))
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

        // PUT: api/pedidos
        [HttpPut]
        public async Task<ActionResult> Put(Pedido pedido)
        {
            context.Entry(pedido).State = EntityState.Modified;
            pedido.Fecha = DateTime.Now;

            foreach (var detalle in pedido.DetallePedidos)
            {
                if (detalle.Id != 0)
                {
                    context.Entry(detalle).State = EntityState.Modified;
                }
                else
                {
                    context.Entry(detalle).State = EntityState.Added;
                }
            }
            var listadodetalle_ids = pedido.DetallePedidos.Select(x => x.Id).ToList();
            var detallesborrar = await context
                .DetallePedidos
                .Where(x => !listadodetalle_ids.Contains(x.Id) && x.PedidoId == pedido.Id)
                .ToListAsync();

            context.RemoveRange(detallesborrar);

            await context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/pedidos/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> Delete(int id)
        {
            var pedido = await context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            context.Pedidos.Remove(pedido);
            await context.SaveChangesAsync();

            return pedido;
        }

        private bool Exists(int id)
        {
            return context.Pedidos.Any(e => e.Id == id);
        }
    }
}