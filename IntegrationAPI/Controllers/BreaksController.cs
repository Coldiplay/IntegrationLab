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
    public class BreaksController : ControllerBase
    {
        private readonly IntegrationDbContext _context;

        public BreaksController(IntegrationDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        // GET: api/Breaks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Break>>> GetBreaks()
        {
            return await _context.Breaks.ToListAsync();
        }

        // GET: api/Breaks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Break>> GetBreak(Guid id)
        {
            var @break = await _context.Breaks.FindAsync(id);

            if (@break == null)
            {
                return NotFound();
            }

            return @break;
        }

        // PUT: api/Breaks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBreak(Guid id, Break @break)
        {
            if (id != @break.Id)
            {
                return BadRequest();
            }

            _context.Entry(@break).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreakExists(id))
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

        // POST: api/Breaks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Break>> PostBreak(Break @break)
        {
            _context.Breaks.Add(@break);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBreak", new { id = @break.Id }, @break);
        }

        // DELETE: api/Breaks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBreak(Guid id)
        {
            var @break = await _context.Breaks.FindAsync(id);
            if (@break == null)
            {
                return NotFound();
            }

            _context.Breaks.Remove(@break);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BreakExists(Guid id)
        {
            return _context.Breaks.Any(e => e.Id == id);
        }
    }
}
