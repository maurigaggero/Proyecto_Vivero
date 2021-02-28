using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EstadisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        ////GET: api/estadisticas/ejercicio/
        [HttpGet("ejercicio")]
        public decimal[] GetEjercicio()
        {
            int año = DateTime.Today.Year;

            decimal[] meses = new decimal[12];

            for (int i = 1; i < meses.Length; i++)
            {
                meses[i-1] = _context.Ventas
                .Where(x => x.Fecha.Date.Month == i && x.Fecha.Date.Year == año
                       && x.FormaPago != FormasPago.CuentaCorriente)
                .Sum(x => x.Total) + _context.Pagos
                .Where(x => x.Fecha.Date.Month == i && x.Fecha.Date.Year == año)
                .Sum(x => x.Importe);
            }
            return meses;
        }

        ////GET: api/estadisticas/ventas/
        [HttpGet("ventas")]
        public decimal[] GetVentas()
        {
            int año = DateTime.Today.Year;

            decimal[] meses = new decimal[12];

            for (int i = 1; i < meses.Length; i++)
            {
                meses[i-1] = _context.Ventas
                .Where(x => x.Fecha.Date.Month == i && x.Fecha.Date.Year == año)
                .Count();
            }
            return meses;
        }

        // GET: api/estadisticas/cantpendientes
        [HttpGet("cantpendientes")]
        public async Task<ActionResult<int>> GetCantPendientes()
        {
            return await _context.Pedidos.Where(x => x.Finalizado == false).CountAsync();
        }
    }
}