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
    public class EstadisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        ////GET: api/estadisticas/ingresos/tiempo
        [HttpGet("ingresos/{tiempo}")]
        public async Task<ActionResult<Tuple<decimal, decimal>>> GetIngresos(string tiempo)
        {
            var queryableA = _context.Ventas.AsQueryable();
            var queryableB = _context.Ventas.AsQueryable();
            var queryableC = _context.Pagos.AsQueryable();
            var queryableD = _context.Pagos.AsQueryable();

            switch (tiempo)
            {
                case "dia":
                    queryableA = queryableA.Where(x => x.Fecha.Date == DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date == DateTime.Today.Date.AddDays(-1));
                    queryableC = queryableC.Where(x => x.Fecha.Date == DateTime.Today.Date);
                    queryableD = queryableD.Where(x => x.Fecha.Date == DateTime.Today.Date.AddDays(-1));
                    break;

                case "mes":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddDays(-30)
                                   && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date < DateTime.Today.Date.AddMonths(-2)
                                   && x.Fecha.Date <= DateTime.Today.Date.AddMonths(-1));
                    queryableC = queryableC.Where(x => x.Fecha.Date > DateTime.Today.Date.AddDays(-30)
                                   && x.Fecha.Date <= DateTime.Today.Date);
                    queryableD = queryableD.Where(x => x.Fecha.Date < DateTime.Today.Date.AddMonths(-2)
                                   && x.Fecha.Date <= DateTime.Today.Date.AddMonths(-1));
                    break;

                case "año":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-1)
                                  && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-2)
                                 && x.Fecha.Date <= DateTime.Today.Date.AddYears(-1));
                    queryableC = queryableC.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-1)
                                  && x.Fecha.Date <= DateTime.Today.Date);
                    queryableD = queryableD.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-2)
                                 && x.Fecha.Date <= DateTime.Today.Date.AddYears(-1));
                    break;
            }

            decimal actual = await queryableA.Where(x => x.FormaPago != FormasPago.CuentaCorriente)
                .SumAsync(x => x.Total) + await queryableC.SumAsync(x => x.Importe);

            decimal anterior = await queryableB.Where(x => x.FormaPago != FormasPago.CuentaCorriente)
                .SumAsync(x => x.Total) + await queryableD.SumAsync(x => x.Importe);

            return Tuple.Create(actual, anterior);
        }

        //GET: api/estadisticas/total/tiempo
        [HttpGet("total/{tiempo}")]
        public async Task<ActionResult<Tuple<decimal,decimal>>> GetTotal(string tiempo)
        {
            var queryableA = _context.Ventas.AsQueryable();
            var queryableB = _context.Ventas.AsQueryable();

            switch (tiempo)
            {
                case "dia":
                    queryableA = queryableA.Where(x => x.Fecha.Date == DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date == DateTime.Today.Date.AddDays(-1));
                    break;

                case "mes":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddDays(-30)
                                   && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date < DateTime.Today.Date.AddMonths(-2)
                                   && x.Fecha.Date <= DateTime.Today.Date.AddMonths(-1));
                    break;

                case "año":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-1)
                                  && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-2)
                                 && x.Fecha.Date <= DateTime.Today.Date.AddYears(-1));
                    break;
            }

            decimal actual = await queryableA.SumAsync(x => x.Total);
            decimal anterior = await queryableB.SumAsync(x => x.Total);

            return Tuple.Create(actual, anterior);
        }

        //GET: api/estadisticas/cantidad/tiempo
        [HttpGet("cantidad/{tiempo}")]
        public async Task<ActionResult<Tuple<int,int>>> GetCantidad(string tiempo)
        {
            var queryableA = _context.Ventas.AsQueryable();
            var queryableB = _context.Ventas.AsQueryable();

            switch (tiempo)
            {
                case "dia":
                    queryableA = queryableA.Where(x => x.Fecha.Date == DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date == DateTime.Today.Date.AddDays(-1));
                    break;

                case "mes":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddDays(-30)
                                   && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date < DateTime.Today.Date.AddMonths(-2)
                                   && x.Fecha.Date <= DateTime.Today.Date.AddMonths(-1));
                    break;

                case "año":
                    queryableA = queryableA.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-1)
                                  && x.Fecha.Date <= DateTime.Today.Date);
                    queryableB = queryableB.Where(x => x.Fecha.Date > DateTime.Today.Date.AddYears(-2)
                                 && x.Fecha.Date <= DateTime.Today.Date.AddYears(-1));
                    break;
            }

            int actual = await queryableA.CountAsync();
            int anterior = await queryableB.CountAsync();

            return Tuple.Create(actual, anterior);
        }

        // GET: api/estadisticas/cantpendientes
        [HttpGet("cantpendientes")]
        public async Task<ActionResult<int>> GetCantPendientes()
        {
            return await _context.Pedidos.Where(x => x.Finalizado == false).CountAsync();
        }
    }
}