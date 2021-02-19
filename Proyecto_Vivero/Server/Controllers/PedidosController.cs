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
    public class PedidosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PedidosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/pedidos
        [HttpGet]
        public async Task<ActionResult<List<Pedido>>> Get()
        {
            return await _context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.DetallePedidos)
                .ThenInclude(x => x.Articulo)
                .ToListAsync();
        }

        //GET: api/pedidos/filtro/nombre&categoria
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Pedido>>> Get([FromQuery] string cliente, [FromQuery] string articulo)
        {
            var queryable = _context.Pedidos
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
            return await _context.Pedidos
                .Include(x => x.Cliente)
                .Include(x => x.DetallePedidos)
                .ThenInclude(x => x.Articulo)
                .FirstAsync(x => x.Id == id);
        }


        // POST: api/pedidos 
        [HttpPost]
        public async Task<ActionResult> Post(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            try
            {
                pedido.Fecha = DateTime.Now;
                await _context.SaveChangesAsync();
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

            _context.Entry(pedido).State = EntityState.Modified;

            pedido.Fecha = DateTime.Now;
            foreach (var detalle in pedido.DetallePedidos)
            {
                if (detalle.Id != 0)
                {
                    _context.Entry(detalle).State = EntityState.Modified;
                }
                else
                {
                    _context.Entry(detalle).State = EntityState.Added;
                }
            }
            var listadodetalle_ids = pedido.DetallePedidos.Select(x => x.Id).ToList();
            var detallesborrar = await _context
                .DetallePedidos
                .Where(x => !listadodetalle_ids.Contains(x.Id) && x.PedidoId == pedido.Id)
                .ToListAsync();

            _context.RemoveRange(detallesborrar);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/pedidos/finalizado
        [HttpPut("finalizado/{finalizado}")]
        public async Task<ActionResult> PutFinalizado(Pedido pedido, bool finalizado)
        {

            _context.Entry(pedido).State = EntityState.Modified;
            pedido.Finalizado = finalizado;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/pedidos/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> Delete(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }

            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return pedido;
        }

        private bool Exists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}