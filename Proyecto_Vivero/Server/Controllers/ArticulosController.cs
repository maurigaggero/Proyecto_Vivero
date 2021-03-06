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
        private readonly ApplicationDbContext context;

        public ArticulosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //GET: api/articulos
        [HttpGet]
        public async Task<ActionResult<List<Articulo>>> Get()
        {
            return await context.Articulos.OrderBy(x => x.Nombre).ToListAsync();
        }

        //GET: api/articulos/filtro/nombre
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Articulo>>> Get([FromQuery] string nombre)
        {
            var queryable = context.Articulos.OrderBy(x => x.Nombre).AsQueryable();
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
            return await context.Articulos.FirstAsync(x => x.Id == id);
        }

        // POST: api/articulos 
        [HttpPost]
        public async Task<ActionResult> Post(Articulo articulo)
        {
            context.Articulos.Add(articulo);
            try
            {
                articulo.Ultima_Modificación = DateTime.Now;
                await context.SaveChangesAsync();
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
            context.Entry(articulo).State = EntityState.Modified;
            articulo.Ultima_Modificación = DateTime.Now;
            await context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/articulos/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Articulo>> Delete(int id)
        {
            var articulo = await context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }

            context.Articulos.Remove(articulo);
            await context.SaveChangesAsync();

            return articulo;
        }

        private bool Exists(int id)
        {
            return context.Articulos.Any(e => e.Id == id);
        }
    }
}