using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GallaryApp.Data;
using GallaryApp.Models;

namespace GallaryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoControll : ControllerBase
    {
        private readonly Context _context;

        public PhotoControll(Context context)
        {
            _context = context;
        }

        // GET: api/PhotoControll
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Photo>>> GetPhotos()
        {
            return await _context.Photos.ToListAsync();
        }

        // GET: api/PhotoControll/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Photo>> GetPhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);

            if (photo == null)
            {
                return NotFound();
            }

            return photo;
        }

        // PUT: api/PhotoControll/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhoto(int id, Photo photo)
        {
            if (id != photo.Id)
            {
                return BadRequest();
            }

            _context.Entry(photo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(id))
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

        // POST: api/PhotoControll
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Photo>> PostPhoto([FromForm] IFormFile photo)
        {
            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            
            var UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(UploadsFolder))
                Directory.CreateDirectory(UploadsFolder);
            
            var FilePath = Path.Combine(UploadsFolder, FileName);

            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var photos = new Photo
            {
                Title = photo.FileName,
                FileName = FileName,
                FileExtension = Path.GetExtension(FileName)
            };
            
            _context.Photos.Add(photos);
            await _context.SaveChangesAsync();
            
            return Ok(new { message = "Фото загружено!", photo = photo });
            //return CreatedAtAction("GetPhoto", new { id = photo.Id }, photo);
        }

        // DELETE: api/PhotoControll/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var photo = await _context.Photos.FindAsync(id);
            if (photo == null)
            {
                return NotFound();
            }
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", photo.FileName);
            System.IO.File.Delete(filePath);
            Console.WriteLine($"Файл удален: {filePath}");
            
            _context.Photos.Remove(photo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhotoExists(int id)
        {
            return _context.Photos.Any(e => e.Id == id);
        }
        
    }
}
