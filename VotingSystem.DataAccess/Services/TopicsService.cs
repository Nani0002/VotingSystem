using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VotingSystem.DataAccess.Exceptions;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public class TopicsService : ITopicsService
    {
        private readonly VotingSystemDbContext _context;

        public TopicsService(VotingSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Topic>> GetUsersAsync(string userId)
        {
            return await _context.Topics
                .Where(x => !x.DeletedAt.HasValue && x.OwnerId == userId)
                .Include(t => t.Choices)
                .Include(t => t.Votes).ThenInclude(v => v.SelectedChoices)
                .Include(t => t.VoterRecords).ThenInclude(r => r.User)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Topic>> GetAllAsync(string? nameFilter = null, DateTime? startDateFilter = null, DateTime? endDateFilter = null, bool onlyOpen = false, bool onlyClosed = false)
        {
            var now = DateTime.Now;

            var query = _context.Topics
                .Where(x => !x.DeletedAt.HasValue)
                .Include(t => t.Choices)
                .Include(t => t.Votes).ThenInclude(v => v.SelectedChoices)
                .Include(t => t.VoterRecords).ThenInclude(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(x => x.Name.Contains(nameFilter));
            }

            if (onlyOpen)
            {
                query = query.Where(x => x.StartDate <= now && x.CloseDate >= now);
            }
            else if (onlyClosed)
            {
                query = query.Where(x => x.CloseDate < now);
            }
            else if (startDateFilter.HasValue && endDateFilter.HasValue)
            {
                var start = startDateFilter.Value;
                var end = endDateFilter.Value;

                query = query.Where(x =>
                    x.StartDate <= end && x.CloseDate >= start &&
                    x.StartDate <= now
                );
            }
            else
            {
                query = query.Where(x => x.StartDate <= now);
            }

            return await query.OrderBy(x => x.CloseDate).ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Choices)
                .Include(t => t.Votes).ThenInclude(v => v.SelectedChoices)
                .Include(t => t.VoterRecords).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(x => x.Id == id && !x.DeletedAt.HasValue);

            if (topic is null)
                throw new ApiException("Nem található megfelelő szavazás!", 404);

            return topic;
        }

        public async Task AddAsync(Topic topic, string ownerId)
        {
            if (topic.StartDate.AddMinutes(15) > topic.CloseDate)
                throw new ApiException("Legalább 15 percnek el kell telnie a két időpont között.");
            topic.OwnerId = ownerId;
            topic.CreatedAt = DateTime.Now;

            try
            {
                await _context.Topics.AddAsync(topic);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ApiException("Sikertelen létrehozás.");
            }
        }

        public async Task UpdateAsync(Topic topic)
        {
            var existingTopic = await GetByIdAsync(topic.Id);
            _context.Choices.RemoveRange(existingTopic.Choices);

            foreach (var choice in topic.Choices)
            {
                choice.TopicId = topic.Id;
            }

            if (existingTopic is null)
                throw new ApiException("Nem található megfelelő szavazás!", 404);

            topic.CreatedAt = existingTopic.CreatedAt;

            try
            {
                _context.Entry(existingTopic).State = EntityState.Detached;
                _context.Topics.Update(topic);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ApiException("Sikertelen módosítás.");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var topic = await GetByIdAsync(id);

            topic.DeletedAt = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw new ApiException("Sikertelen törlés.");
            }
        }

        public async Task VoteOnTopicAsync(Vote vote, VoteRecord voteRecord)
        {
            if (vote.SelectedChoices.Count > vote.Topic.MaximalVotes)
                throw new ApiException("Túl sok megjelölt választás.");

            if (vote.SelectedChoices.Count < vote.Topic.MinimalVotes)
                throw new ApiException("Kevé megjelölt válaztás.");

            bool hasVoted = await _context.VoteRecords.AnyAsync(r => r.TopicId == voteRecord.TopicId && r.UserId == voteRecord.UserId);
            if (hasVoted)
                throw new ApiException("A felhasználó már egyzer szavazott.");

            _context.Votes.Add(vote);
            _context.VoteRecords.Add(voteRecord);
            await _context.SaveChangesAsync();
        }
    }
}
