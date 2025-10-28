using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Stadium
    {
        public int Id { get; set; }
        public string StadiumName { get; set; } = null!;
        public ICollection<Match> Matches { get; set; }   // Matches played in this stadium
    }
}
