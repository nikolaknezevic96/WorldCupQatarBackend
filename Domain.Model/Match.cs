using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Match
    {
        public int Id { get; set; }
        public int Team1Id { get; set; }
        public int Team2Id { get; set; }
        public Team Team1 { get; set; } 
        public Team Team2 { get; set; } 
        public int? Team1GoalsScored { get; set; }  // Goals scored by Tim1
        public int? Team2GoalsScored { get; set; }  // Goals scored by Tim2
        public DateTime StartDateTime { get; set; }
        public bool Forfeited { get; set; }
        public int? StadiumId { get; set; }
        public Stadium? Stadium { get; set; }



    }
}
