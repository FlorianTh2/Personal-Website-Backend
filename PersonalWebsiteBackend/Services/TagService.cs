using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;

namespace PersonalWebsiteBackend.Services
{
    public class TagService : ITagService
    {

        private readonly DataContext _dataContext;

        public TagService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _dataContext.Tags.AsNoTracking().ToListAsync();
        }

        public async Task<Tag> GetTagByNameAsync(string tagName)
        {
            return await _dataContext.Tags.SingleOrDefaultAsync(a => a.Name == tagName);
        }

        public async Task<bool> CreateTagAsync(Tag tag)
        {
            tag.Name = tag.Name.ToLower();
            var existingTag = await _dataContext.Tags.AsNoTracking().SingleOrDefaultAsync(x => x.Name == tag.Name);
            if (existingTag != null)
                return true;
            await _dataContext.AddAsync(tag);
            var created = await _dataContext.SaveChangesAsync();
            // The task result contains the number of state entries written to the underlying database
            return created > 0;
        }

        public async Task<bool> DeleteTagAsync(string tagName)
        {
            var tag = await _dataContext.Tags.AsNoTracking()
                .Include(a => a.Projects)
                .Include(b => b.Documents)
                .SingleOrDefaultAsync(c => c.Name == tagName);

            if (tag == null)
            {
                return true;
            }

            _dataContext.Remove(tag);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
    }
}