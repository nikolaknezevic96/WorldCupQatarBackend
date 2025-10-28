using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }  
        public string Flag { get; set; } 
        public int? GroupId { get; set; }
        public Group? Group { get; set; } 
        public int NumPoints { get; set; }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }
        public int NumDraws { get; set; }
        public int NumGoalsScored { get; set; }
        public int NumGoalsConceded { get; set; }
    }
}
