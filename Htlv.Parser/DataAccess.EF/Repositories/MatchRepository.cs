using Htlv.Parser.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.DataAccess.EF.Repositories
{
    public class MatchRepository
    {
        private readonly BotDbContext _context;

        public MatchRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<CSGOMatch> GetMatchById(int matchId)
        {
            return await _context.CSGOMatches.FindAsync(matchId);
        }

        public async Task AddMatches(IEnumerable<CSGOMatch> matches)
        {
            await _context.CSGOMatches.AddRangeAsync(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CSGOMatch>> GetActualMatches()
        {
            return await _context.CSGOMatches.Where(m => m.MatchTime.Date.CompareTo(DateTime.Now.Date) != -1).ToListAsync();
        }

        public IQueryable<CSGOMatch> GetActualMatchesQuery()
        {
            return _context.CSGOMatches.Where(m => m.MatchTime.Date.CompareTo(DateTime.Now.Date) != -1);
        }
    }
}
