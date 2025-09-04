using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface ITopicsService
    {
        public Task<IReadOnlyCollection<Topic>> GetUsersAsync(string userId);
        public Task<IReadOnlyCollection<Topic>> GetAllAsync(string? nameFilter = null, DateTime? startDateFilter = null, DateTime? endDateFilter = null, bool onlyOpen = false, bool onlyClosed = false);
        public Task<Topic> GetByIdAsync(int id);
        public Task AddAsync(Topic topic, string ownerId);
        public Task UpdateAsync(Topic topic);
        public Task DeleteAsync(int id);
        public Task VoteOnTopicAsync(Vote vote, VoteRecord voteRecord);
    }
}
