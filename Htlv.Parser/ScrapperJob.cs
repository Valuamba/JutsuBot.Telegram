using Htlv.Parser.DataAccess.EF;
using Htlv.Parser.DataAccess.EF.Repositories;
using Htlv.Parser.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser
{
    public class ScrapperJob : IJob
    {
        private readonly BotDbContext _context;
        private readonly ILogger<ScrapperJob> _logger;
        private readonly CSGOMatchEqualityComparer _csgoMatchEquality;
        private readonly MatchRepository _matchRepository;

        public ScrapperJob(
            CSGOMatchEqualityComparer csgoMatchEquality, 
            BotDbContext context, 
            ILogger<ScrapperJob> logger,
            MatchRepository matchRepository)
        {
            _logger = logger;
            _context = context;
            _csgoMatchEquality = csgoMatchEquality;
            _matchRepository = matchRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var actualMatches = GetCSGOMatches();
            var currentMatches = await _matchRepository.GetActualMatches();

            _logger.LogInformation("Execute job");
            if(actualMatches is not null && currentMatches is not null)
            {
                var matchesToBeAdded = actualMatches.Except(currentMatches, _csgoMatchEquality);

                if (matchesToBeAdded is not null && matchesToBeAdded.Count() != 0)
                {
                    _logger.LogInformation("Add new matches to schedule");

                    await _matchRepository.AddMatches(matchesToBeAdded);
                }
            }
        }

        private List<CSGOMatch> GetCSGOMatches()
        {
            HtmlDocument docc = new HtmlDocument();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.hltv.org/matches");
                request.Method = "GET";

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        docc.Load(stream, Encoding.UTF8);
                    }
                }

                var upcomingMatchSections = docc.DocumentNode.SelectNodes("//*[@class='upcomingMatchesSection']");

                List<CSGOMatch> csgoMatches = new();

                foreach (var upcomingMatchSection in upcomingMatchSections)
                {
                    var matchDayText = upcomingMatchSection.SelectSingleNode(".//*[@class='matchDayHeadline']").InnerText;
                    CultureInfo cultureInfo = new("en-EN");
                    var matchDate = DateTime.ParseExact(matchDayText, "dddd - yyyy-MM-dd", cultureInfo);

                    var matches = upcomingMatchSection.SelectNodes(".//*[contains(@class,'upcomingMatch') and @team1 and @team2]");
                    if (matches != null)
                    {
                        foreach (var match in matches)
                        {
                            CSGOMatch matchModel = new();

                            matchModel.FirstTeam = match.SelectSingleNode(".//*[@class='matchTeam team1']/*[contains(@class, 'matchTeamName')]").InnerText;
                            matchModel.SecondTeam = match.SelectSingleNode(".//*[@class='matchTeam team2']/*[contains(@class, 'matchTeamName')]").InnerText;
                            matchModel.MatchMeta = match.SelectSingleNode(".//*[@class='matchMeta']").InnerText;
                            matchModel.MatchEvent = match.SelectSingleNode(".//*[contains(@class,'matchEventName')]").InnerText;
                            var matchTime = DateTime.ParseExact(match.SelectSingleNode(".//*[@class='matchTime']").InnerText, "HH:mm", cultureInfo);
                            matchModel.MatchTime = new DateTime(matchDate.Year, matchDate.Month, matchDate.Day, matchTime.Hour, matchTime.Minute, 0);

                            csgoMatches.Add(matchModel);
                        }
                    }
                }

                return csgoMatches;

            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
