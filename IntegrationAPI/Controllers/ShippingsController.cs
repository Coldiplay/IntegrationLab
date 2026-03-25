using IntegrationAPI.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaseLibrary.Model;

namespace IntegrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingsController : ControllerBase
    {
        private readonly IntegrationDbContext _context;

        public ShippingsController(IntegrationDbContext context)
        {
            _context = context;
        }

        // GET: api/Shippings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipping>>> GetShippings()
        {
            return await _context.Shippings.ToListAsync();
        }

        // GET: api/Shippings/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Shipping>> GetShipping(Guid id)
        {
            var shipping = await _context.Shippings.FindAsync(id);

            if (shipping == null)
            {
                return NotFound();
            }

            return shipping;
        }

        // PUT: api/Shippings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutShipping(Guid id, Shipping shipping)
        {
            if (id != shipping.Id)
            {
                return BadRequest();
            }

            _context.Entry(shipping).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingExists(id))
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

        // POST: api/Shippings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Shipping>> PostShipping(Shipping shipping)
        {
            _context.Shippings.Add(shipping);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShipping", new { id = shipping.Id }, shipping);
        }

        // DELETE: api/Shippings/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteShipping(Guid id)
        {
            var shipping = await _context.Shippings.FindAsync(id);
            if (shipping == null)
            {
                return NotFound();
            }

            _context.Shippings.Remove(shipping);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShippingExists(Guid id)
        {
            return _context.Shippings.Any(e => e.Id == id);
        }
    }
}
