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
    public class ShippingOrdersController : ControllerBase
    {
        private readonly IntegrationDbContext _context;

        public ShippingOrdersController(IntegrationDbContext context)
        {
            _context = context;
        }

        // GET: api/ShippingOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShippingOrder>>> GetShippingOrders()
        {
            return await _context.ShippingOrders.ToListAsync();
        }

        // GET: api/ShippingOrders/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShippingOrder>> GetShippingOrder(int id)
        {
            var shippingOrder = await _context.ShippingOrders.FindAsync(id);

            if (shippingOrder == null)
            {
                return NotFound();
            }

            return shippingOrder;
        }

        // PUT: api/ShippingOrders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        public async Task<IActionResult> PutShippingOrder(int id, ShippingOrder shippingOrder)
        {
            if (id != shippingOrder.Id)
            {
                return BadRequest();
            }

            _context.Entry(shippingOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingOrderExists(id))
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

        // POST: api/ShippingOrders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShippingOrder>> PostShippingOrder(ShippingOrder shippingOrder)
        {
            _context.ShippingOrders.Add(shippingOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingOrder", new { id = shippingOrder.Id }, shippingOrder);
        }

        // DELETE: api/ShippingOrders/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShippingOrder(int id)
        {
            var shippingOrder = await _context.ShippingOrders.FindAsync(id);
            if (shippingOrder == null)
            {
                return NotFound();
            }

            _context.ShippingOrders.Remove(shippingOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShippingOrderExists(int id)
        {
            return _context.ShippingOrders.Any(e => e.Id == id);
        }
    }
}
