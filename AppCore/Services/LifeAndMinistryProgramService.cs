
using CongEspVilaGuilhermeApi.AppCore.Repositories;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class LifeAndMinistryProgramService
    {
        private ILifeAndMinistryRepository repository;
        private WebContentService browser;
        private DateTime[] memorialWeeksMondays = new[] { 
            new DateTime(2025, 04, 7),
            new DateTime(2026, 03, 30),
            new DateTime(2027, 03, 22),
        };

        public LifeAndMinistryProgramService(ILifeAndMinistryRepository repository)
        {
            this.repository = repository;
            this.browser = new WebContentService();
        }
        public async Task<LifeAndMinistryWeek> GetProgramByDate(DateTime date)
        {
            var weekId = getWeekId(date);
            var entity = await repository.GetById(weekId);
            if(entity == null)
            {
                entity  = await BuildNew(weekId);
                await this.repository.CreateOrUpdate(entity);
            }
            return entity;
        }

        private async Task<LifeAndMinistryWeek> BuildNew(string weekId)
        {
            var url = $"https://wol.jw.org/es/wol/d/r4/lp-s/20{weekId}";
            var html = await browser.GetAsync(url);

            var asignments = html.SelectNodes("//h3").Select(x => x.InnerText).ToList();
            var times = GetTimeList(html);
            var bbt = BuildBecameBetterTeachers(asignments, times);

            return new LifeAndMinistryWeek
            {
                WeekLabel = html.SelectSingleNode("//h1").InnerText,
                Id = weekId,
                IsCancelled = false,
                BibleWeekReading = html.SelectSingleNode("//h2").InnerText,
                OpeningSong = Convert.ToInt32(asignments[0].Split(" ")[1]),
                BibleTreasures = new LifeAndMinistryAsignment
                {
                    Title = asignments[1],
                    BrotherName = "",
                    Minutes = 10
                },
                BibleReading = new LifeAndMinistryStudentsAsignment
                {
                    Title = "3. Lectura de biblia",
                    BrotherName = ""
                },
                BecameBetterTeachers = bbt,
                MiddleSong = Convert.ToInt32(getMiddleSongText(asignments).Replace("Canción", string.Empty).TrimStart()),
                OurChristianLife = BuildOurChristianLife(asignments, times, bbt.Count),
                CongregationBibleStudy = new LifeAndMinistryBibleStudy
                {
                    Title = getBiblyStudy(asignments) ?? string.Empty,
                    BrotherName = string.Empty,
                    Reader = string.Empty,
                    Minutes = 30
                },
                EndingSong = GetEndingSong(asignments)
            };
        }

        private int GetEndingSong(List<string> asignments)
        {
            int.TryParse((asignments.FirstOrDefault(x => x.Contains("Palabras de conclusión")) ?? string.Empty).Split(" | ")[1].Split(" ")[1], out int endingSong);
            return endingSong;
        }

        private List<int> GetTimeList(HtmlNode html)
        {
            return html.SelectNodes("//div[contains(@class, 'bodyTxt')] //div //p")
                .Where(x => x.InnerText.Contains("mins"))
                .Select(x => Convert.ToInt32(x.InnerText.Split(" mins.)").FirstOrDefault() ?? "0".Replace("(", "")))
                .ToList();
        }

        private List<LifeAndMinistryAsignment> BuildOurChristianLife(List<string> asignments, List<int> minutesList, int becameBetterTeachersCount)
        {
            var initialIndex = asignments.IndexOf(getMiddleSongText(asignments)) + 1;
            var finalIndex = asignments.IndexOf(getBiblyStudy(asignments)) - 1;
            var initialIndexMinutes = becameBetterTeachersCount + 3;
            var minutes = minutesList.Where((_, i) => i >= initialIndexMinutes).ToList();

            return FilterAndBuildAsignmeny(asignments, minutes, initialIndex, finalIndex);
        }

        private string getMiddleSongText(List<string> asignments) => asignments.Where(x => x.StartsWith("Canción")).ToList()[1];
        private string getBiblyStudy(List<string> asignments) => asignments.FirstOrDefault(x => x.Contains("Estudio bíblico de la congregación")) ?? string.Empty;

        private List<LifeAndMinistryStudentsAsignment> BuildBecameBetterTeachers(List<string> asignments, List<int> minutesList)
        {
            var initialIndex = asignments.IndexOf(asignments.FirstOrDefault(x => x.StartsWith("4.")) ?? string.Empty);
            var finalIndex = asignments.IndexOf(asignments.Where(x => x.StartsWith("Canción")).ToList()[1]) -1;

            var finalIndexMinutes = (finalIndex - initialIndex + 1) + 3;
            var minutes = minutesList.Where((_, i) => i >= 3 && i <= finalIndexMinutes).ToList();

            return FilterAndBuildAsignmeny(asignments, minutes, initialIndex, finalIndex).Select(x => new LifeAndMinistryStudentsAsignment 
            {  
               BrotherName = x.BrotherName,
               Title = x.Title,
               Minutes = x.Minutes
            }).ToList();
        }

        private List<LifeAndMinistryAsignment> FilterAndBuildAsignmeny(List<string> asignments, List<int> minutesList, int initialIndex, int finalIndex)
        {
            return asignments.Where((x, i) => i >= initialIndex && i <= finalIndex).Select((x, i) => new LifeAndMinistryAsignment
            {
                Title = x,
                BrotherName = "",
                Minutes = minutesList[i]
            }).ToList();
        }

        private string getWeekId(DateTime date)
        {
            var week = 1;
            var edition = 0;
            var month = date.Month % 2 == 1 ? date.Month : date.Month - 1;
            
            var firstMondayOfEdition = new DateTime(date.Year, month, 1);
            while (firstMondayOfEdition.DayOfWeek != DayOfWeek.Monday)
            {
                firstMondayOfEdition = firstMondayOfEdition.AddDays(1);
            }

            switch (month)
            {
                case 3:
                case 4:
                    edition = 8;
                    break;
                case 5:
                case 6:
                    edition = 16;
                    break;
                case 7:
                case 8:
                    edition = 24;
                    break;
                case 9:
                case 10:
                    edition = 32;
                    break;
                case 11:
                case 12:
                    edition = 40;
                    break;
            }

            while (firstMondayOfEdition <= date)
            {
                week++;
                //ignorando a semana da comemoração
                if (memorialWeeksMondays.Any(x => x == firstMondayOfEdition))
                {
                    week++;
                }
                if(week == 10)
                {
                    week = 0;
                    edition++;
                }
                firstMondayOfEdition = firstMondayOfEdition.AddDays(7);
            }

            return $"{firstMondayOfEdition.Year}{edition.ToString("00")}{week}";
        }

        public Task UpdateWeek(LifeAndMinistryWeek week)
        {
            return repository.CreateOrUpdate(week);
        }
    }
}
