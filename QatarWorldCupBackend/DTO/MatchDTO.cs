namespace QatarWorldCupBackend.DTO
{
    public class MatchDTO
    {
        public int Id { get; set; }
        public int Team1Id { get; set; }

        public int Team2Id { get; set; }

        public DateTime StartDateTime { get; set; } = DateTime.Now;
        public int? Team1GoalsScored { get; set; }
        public int? Team2GoalsScored { get; set; }
        public int? StadiumId { get; set; }
        public bool Forfeited { get; set; } = false;
        public bool Team1Forfeited { get; set; } = false;
        public bool Team2Forfeited { get; set; } = false;
    }
}
