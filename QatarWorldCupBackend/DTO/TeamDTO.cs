namespace QatarWorldCupBackend.DTO
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Flag { get; set; }
        public int? GroupId { get; set; }
        public int NumPoints { get; set; }
        public int NumWins { get; set; }
        public int NumLosses { get; set; }
        public int NumDraws { get; set; }
        public int NumGoalsScored { get; set; }
        public int NumGoalsConceded { get; set; }
    }
}
