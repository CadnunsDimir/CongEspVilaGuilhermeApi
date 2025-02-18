using Amazon.DynamoDBv2.DocumentModel;

namespace CongEspVilaGuilhermeApi.Domain.Entities
{
    public class LifeAndMinistryWeek
    {
        public required string Id { get; set; }
        public required string WeekLabel { get; set; }
        public required bool IsCancelled { get; set; }
        public required string BibleWeekReading { get; set; }
        public int OpeningSong { get; set; }
        public string President { get; set; }
        public string AuxiliarRoomConductor { get; set; }
        public string OpeningPrayerBrother { get; set; }
        public  LifeAndMinistryAsignment BibleTreasures { get; set; }
        public LifeAndMinistryStudentsAsignment BibleReading { get; set; }
        public string HiddenPearlsConductor { get; set; }
        public List<LifeAndMinistryStudentsAsignment> BecameBetterTeachers { get; set; }
        public int MiddleSong { get; set; }
        public List<LifeAndMinistryAsignment> OurChristianLife { get; set; }
        public LifeAndMinistryBibleStudy CongregationBibleStudy { get; set; }
        public int? EndingSong { get; set; }
        public string? EndingPrayerBrother { get; set; }
    }
}
