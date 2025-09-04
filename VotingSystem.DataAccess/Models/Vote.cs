using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Topic")]
        public int TopicId { get; set; }
        public required virtual Topic Topic { get; set; }
        public virtual ICollection<Choice> SelectedChoices { get; set; } = [];
        public DateTime CreatedAt { get; set; }
    }
}
