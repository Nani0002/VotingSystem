using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class VoteRecord
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public required string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; } = null!;
    }
}
