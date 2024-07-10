using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Utilities.Slides;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.Application.Utilities.Slides
{
    public class SlideService : ISlideService
    {
        private readonly EShopDbContext _context;

        public SlideService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<SlideVm>> GetAll()
        {
            var slides = await _context.Slides
                .OrderBy(s => s.SortOrder)
                .Select(s => new SlideVm
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Url = s.Url,
                    Image = s.Image
                })
                .ToListAsync();

            return slides;
        }
    }
}
