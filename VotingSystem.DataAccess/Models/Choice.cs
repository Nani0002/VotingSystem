using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }
        public required string Value { get; set; }
        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public virtual Topic Topic { get; set; } = null!;
        public virtual ICollection<Vote> Votes { get; set; } = null!;
    }
}
