
using CongEspVilaGuilhermeApi.AppCore.Enums;
using CongEspVilaGuilhermeApi.AppCore.Repositories;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Repositories;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    public class LifeAndMinistryProgramService
    {
        private readonly ILifeAndMinistryRepository repository;
        private readonly WebContentService browser;
        private static readonly Dictionary<int, int> memorialWeeks = new Dictionary<int, int>()
        {
            { 2024,  82 },
            { 2025,  87 },
            { 2026,  85 },
        };

        public LifeAndMinistryProgramService(ILifeAndMinistryRepository repository)
        {
            this.repository = repository;
            this.browser = new WebContentService();
        }
        public async Task<LifeAndMinistryWeek> GetProgramByDate(DateTime date)
        {
            var weekId = GetWeekId(date);
            var entity = await repository.GetById(weekId);
            if (entity == null)
            {
                entity = await BuildNew(weekId);
                await this.repository.CreateOrUpdate(entity);
            }
            return entity;
        }

        private async Task<LifeAndMinistryWeek> BuildNew(string weekId)
        {
            var url = $"https://wol.jw.org/es/wol/d/r4/lp-s/20{weekId}";
            var html = await browser.GetAsync(url);
            if (html == null)
            {
                throw new InvalidOperationException("Meeting not found on wol.jw.org");
            }

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
                    BrotherName = "",
                    Minutes = 4
                },
                BecameBetterTeachers = bbt,
                MiddleSong = Convert.ToInt32(GetMiddleSongText(asignments).Replace("Canción", string.Empty).TrimStart()),
                OurChristianLife = BuildOurChristianLife(asignments, times, bbt.Count),
                CongregationBibleStudy = new LifeAndMinistryBibleStudy
                {
                    Title = GetBiblyStudy(asignments) ?? string.Empty,
                    BrotherName = string.Empty,
                    Reader = string.Empty,
                    Minutes = 30
                },
                EndingSong = GetEndingSong(asignments)
            };
        }

        private static int GetEndingSong(List<string> asignments)
        {
            int.TryParse((asignments.FirstOrDefault(x => x.Contains("Palabras de conclusión")) ?? string.Empty).Split(" | ")[1].Split(" ")[1], out int endingSong);
            return endingSong;
        }

        private static List<int> GetTimeList(HtmlNode html)
        {
            return html.SelectNodes("//div[contains(@class, 'bodyTxt')] //div //p")
                .Where(x => x.InnerText.Contains("mins"))
                .Select(x =>
                {
                    var min = Regex.Replace(
                        x.InnerText.Split(".)").FirstOrDefault() ?? string.Empty, "[^0-9]", "");
                    var minOut = 0;
                    int.TryParse(min, out minOut);
                    return minOut;
                }).ToList();
        }

        private List<LifeAndMinistryAsignment> BuildOurChristianLife(List<string> asignments, List<int> minutesList, int becameBetterTeachersCount)
        {
            var initialIndex = asignments.IndexOf(GetMiddleSongText(asignments)) + 1;
            var finalIndex = asignments.IndexOf(GetBiblyStudy(asignments)) - 1;
            var initialIndexMinutes = becameBetterTeachersCount + 3;
            var minutes = minutesList.Where((_, i) => i >= initialIndexMinutes).ToList();

            return FilterAndBuildAsignmeny(asignments, minutes, initialIndex, finalIndex);
        }

        private static string GetMiddleSongText(List<string> asignments) => asignments.Where(x => x.StartsWith("Canción")).ToList()[1];
        private static string GetBiblyStudy(List<string> asignments) => asignments.FirstOrDefault(x => x.Contains("Estudio bíblico de la congregación")) ?? string.Empty;

        private List<LifeAndMinistryStudentsAsignment> BuildBecameBetterTeachers(List<string> asignments, List<int> minutesList)
        {
            var initialIndex = asignments.IndexOf(asignments.FirstOrDefault(x => x.StartsWith("4.")) ?? string.Empty);
            var finalIndex = asignments.IndexOf(asignments.Where(x => x.StartsWith("Canción")).ToList()[1]) - 1;

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

        private static string GetWeekId(DateTime date)
        {
            var thisWeekMonday = date;
            while (thisWeekMonday.DayOfWeek != DayOfWeek.Monday)
            {
                thisWeekMonday = thisWeekMonday.AddDays(-1);
            }
            int editionWeek = (int)LifeAndMinistryProgramEdition.JanuaryEdition;
            var month = thisWeekMonday.Month % 2 == 1 ? thisWeekMonday.Month : thisWeekMonday.Month - 1;

            var firstMondayOfEdition = new DateTime(thisWeekMonday.Year, month, 1, 0, 0, 0, DateTimeKind.Local);
            while (firstMondayOfEdition.DayOfWeek != DayOfWeek.Monday)
            {
                firstMondayOfEdition = firstMondayOfEdition.AddDays(1);
            }

            switch (month)
            {
                case 3:
                case 4:
                    editionWeek = (int)LifeAndMinistryProgramEdition.MarchEdition;
                    break;
                case 5:
                case 6:
                    editionWeek = (int)LifeAndMinistryProgramEdition.MayEdition;
                    break;
                case 7:
                case 8:
                    editionWeek = (int)LifeAndMinistryProgramEdition.JulyEdition;
                    break;
                case 9:
                case 10:
                    editionWeek = (int)LifeAndMinistryProgramEdition.SeptemberEdition;
                    break;
                case 11:
                case 12:
                    editionWeek = (int)LifeAndMinistryProgramEdition.NovemberEdition;
                    break;
            }

            while (firstMondayOfEdition < thisWeekMonday)
            {
                editionWeek++;
                //ignorando a semana da comemoração
                if (memorialWeeks[thisWeekMonday.Year] == editionWeek)
                {
                    editionWeek++;
                }
                firstMondayOfEdition = firstMondayOfEdition.AddDays(7);
            }

            return $"{firstMondayOfEdition.Year}{editionWeek:000}";
        }

        public Task UpdateWeek(LifeAndMinistryWeek week)
        {
            return repository.CreateOrUpdate(week);
        }
    }
}
