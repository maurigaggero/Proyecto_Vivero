﻿using Microsoft.AspNetCore.Authorization;
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
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> Get()
        {
            return await _context.Clientes.OrderBy(x => x.NombreyApellido).ToListAsync();
        }

        //GET: api/clientes/filtro/nombre
        [HttpGet("filtro")]
        public async Task<ActionResult<List<Cliente>>> Get([FromQuery] string nombre)
        {
            var queryable = _context.Clientes.OrderBy(x => x.NombreyApellido).AsQueryable();
            if (!string.IsNullOrEmpty(nombre))
            {
                queryable = queryable.Where(x => x.NombreyApellido.Contains(nombre));
            }
            return await queryable.ToListAsync();
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> Get(int id)
        {
            return await _context.Clientes.FirstAsync(x => x.Id == id);
        }

        // POST: api/clientes 
        [HttpPost]
        public async Task<ActionResult> Post(Cliente cliente)
        {
            if (!Exists(cliente.Dni))
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        // PUT: api/clientes
        [HttpPut]
        public async Task<ActionResult> Put(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/clientes/5  
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cliente>> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        private bool Exists(string dni)
        {
            return (_context.Clientes.Any(e => e.Dni == dni));
        }
    }
}