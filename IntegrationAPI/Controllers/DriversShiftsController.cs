using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IntegrationAPI.Db;
using Models.Model;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversShiftsController : ControllerBase
    {
        private readonly IntegrationDbContext _context;

        public DriversShiftsController(IntegrationDbContext context)
        {
            _context = context;
        }

        // GET: api/DriversShifts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DriversShift>>> GetDriversShifts()
        {
            return await _context.DriversShifts.ToListAsync();
        }

        // GET: api/DriversShifts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DriversShift>> GetDriversShift(Guid id)
        {
            var driversShift = await _context.DriversShifts.FindAsync(id);

            if (driversShift == null)
            {
                return NotFound();
            }

            return driversShift;
        }

        // PUT: api/DriversShifts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriversShift(Guid id, DriversShift driversShift)
        {
            if (id != driversShift.Id)
            {
                return BadRequest();
            }

            _context.Entry(driversShift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriversShiftExists(id))
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

        // POST: api/DriversShifts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DriversShift>> PostDriversShift(DriversShift driversShift)
        {
            _context.DriversShifts.Add(driversShift);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDriversShift", new { id = driversShift.Id }, driversShift);
        }

        // DELETE: api/DriversShifts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriversShift(Guid id)
        {
            var driversShift = await _context.DriversShifts.FindAsync(id);
            if (driversShift == null)
            {
                return NotFound();
            }

            _context.DriversShifts.Remove(driversShift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DriversShiftExists(Guid id)
        {
            return _context.DriversShifts.Any(e => e.Id == id);
        }
    }
}
