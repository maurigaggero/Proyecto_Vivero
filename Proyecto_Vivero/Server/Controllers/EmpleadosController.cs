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
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmpleadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/empleados
        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> Get()
        {
            return await _context.Empleados.ToListAsync();
        }

        // GET: api/empleados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> Get(int id)
        {
            return await _context.Empleados.FirstAsync(x => x.Id == id);
        }


        // POST: api/empleados 
        [HttpPost]
        public async Task<ActionResult> Post(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(empleado.Id))
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


        // PUT: api/empleados
        [HttpPut]
        public async Task<ActionResult> Put(Empleado empleado)
        {
            _context.Entry(empleado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/empleados/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Empleado>> Delete(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            return empleado;
        }

        private bool Exists(int id)
        {
            return _context.Empleados.Any(e => e.Id == id);
        }
    }
}
