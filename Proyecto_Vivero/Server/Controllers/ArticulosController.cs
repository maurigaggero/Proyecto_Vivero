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
    public class ArticulosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticulosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/articulos
        [HttpGet]
        public async Task<ActionResult<List<Articulo>>> Get()
        {
            return await _context.Articulos.OrderBy(x => x.Nombre).ToListAsync();
        }

        //GET: api/articulos/filtro/nombre
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Articulo>>> Get([FromQuery] string nombre)
        {
            var queryable = _context.Articulos.OrderBy(x => x.Nombre).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.Nombre.Contains(nombre));
            }
            return await queryable.ToListAsync();
        }

        // GET: api/articulos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Articulo>> Get(int id)
        {
            return await _context.Articulos.FirstAsync(x => x.Id == id);
        }


        // POST: api/articulos 
        [HttpPost]
        public async Task<ActionResult> Post(Articulo articulo)
        {
            _context.Articulos.Add(articulo);
            try
            {
                articulo.Ultima_Modificación = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(articulo.Id))
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


        // PUT: api/articulos
        [HttpPut]
        public async Task<ActionResult> Put(Articulo articulo)
        {

            _context.Entry(articulo).State = EntityState.Modified;
            articulo.Ultima_Modificación = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/articulos
        [HttpPut("stock/{newstock}")]
        public async Task<ActionResult> PutStock(Articulo articulo, int newstock)
        {
            _context.Entry(articulo).State = EntityState.Modified;
            articulo.Ultima_Modificación = DateTime.Now;
            articulo.StockActual = newstock;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/articulos/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Articulo>> Delete(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            _context.Articulos.Remove(articulo);
            await _context.SaveChangesAsync();

            return articulo;
        }

        private bool Exists(int id)
        {
            return _context.Articulos.Any(e => e.Id == id);
        }
    }
}