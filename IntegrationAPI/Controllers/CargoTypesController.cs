using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntegrationAPI.Db;
using Models.Model;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoTypesController : ControllerBase
    {
        private readonly IntegrationDbContext _context;

        public CargoTypesController(IntegrationDbContext context)
        {
            _context = context;
        }

        // GET: api/CargoTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoType>>> GetTypes()
        {
            return await _context.CargoTypes.ToListAsync();
        }

        // GET: api/CargoTypes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CargoType>> GetCargoType(int id)
        {
            var cargoType = await _context.CargoTypes.FindAsync(id);

            if (cargoType == null)
            {
                return NotFound();
            }

            return cargoType;
        }

        // PUT: api/CargoTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutCargoType(int id, CargoType cargoType)
        {
            if (id != cargoType.Id)
            {
                return BadRequest();
            }

            _context.Entry(cargoType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CargoTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CargoTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CargoType>> PostCargoType(CargoType cargoType)
        {
            _context.CargoTypes.Add(cargoType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCargoType", new { id = cargoType.Id }, cargoType);
        }

        // DELETE: api/CargoTypes/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCargoType(int id)
        {
            var cargoType = await _context.CargoTypes.FindAsync(id);
            if (cargoType == null)
            {
                return NotFound();
            }

            _context.CargoTypes.Remove(cargoType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CargoTypeExists(int id)
        {
            return _context.CargoTypes.Any(e => e.Id == id);
        }
    }
}
