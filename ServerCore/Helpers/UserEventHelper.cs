using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServerCore.DataModel;

namespace ServerCore.Helpers
{
    /// <summary>
    /// Includes methods that connect a PuzzleUser with another part of the data model
    /// </summary>
    public static class UserEventHelper
    {
        /// <summary>
        /// Returns a list of the author's puzzles for the given event
        /// </summary>
        /// <param name="thisEvent">The event that's being checked</param>
        /// <param name="puzzleServerContext">Current PuzzleServerContext</param>
        public static IQueryable<Puzzle> GetPuzzlesForAuthorAndEvent(PuzzleServerContext dbContext, Event thisEvent, PuzzleUser user)
        {
            return dbContext.PuzzleAuthors.Where(pa => pa.Author.ID == user.ID && pa.Puzzle.Event.ID == thisEvent.ID).Select(pa => pa.Puzzle);
        }

        /// <summary>
        /// Returns whether the user is an author of this puzzle
        /// </summary>
        /// <param name="puzzle">The puzzle that's being checked</param>
        /// <param name="puzzleServerContext">Current PuzzleServerContext</param>
        public static Task<bool> IsAuthorOfPuzzle(PuzzleServerContext dbContext, Puzzle puzzle, PuzzleUser user)
        {
            return dbContext.PuzzleAuthors.Where(pa => pa.Author.ID == user.ID && pa.Puzzle.ID == puzzle.ID).AnyAsync();
        }

        /// <summary>
        /// Returns the the team for the given player
        /// </summary>
        /// <param name="dbContext">Current PuzzleServerContext</param>
        /// <param name="thisEvent">The event that's being checked</param>
        /// <param name="user">The user being checked</param>
        /// <returns>The user's team for this event</returns>
        public static async Task<Team> GetTeamForPlayer(PuzzleServerContext dbContext, Event thisEvent, PuzzleUser user)
        {
            if (user == null)
            {
                return null;
            }

            return await GetTeamForPlayer(dbContext, thisEvent, user.ID);
        }

        /// <summary>
        /// Returns the the team for the given player
        /// </summary>
        /// <param name="dbContext">Current PuzzleServerContext</param>
        /// <param name="thisEvent">The event that's being checked</param>
        /// <param name="userId">The user id being checked</param>
        /// <returns>The user's team for this event</returns>
        public static async Task<Team> GetTeamForPlayer(PuzzleServerContext dbContext, Event thisEvent, int userId)
        {
            if (thisEvent == null)
            {
                return null;
            }

            return await dbContext.TeamMembers.Where(t => t.Member.ID == userId && t.Team.Event.ID == thisEvent.ID).Select(t => t.Team).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns the team for the currently signed in player
        /// </summary>
        /// <param name="puzzlerServerContext">Current PuzzleServerContext</param>
        /// <param name="thisEvent">The event that's being checked</param>
        /// <param name="user">The claim for the user being checked</param>
        /// <param name="userManager">The UserManager for the current context</param>
        /// <returns>The user's team for this event</returns>
        public static async Task<Team> GetTeamForCurrentPlayer(PuzzleServerContext puzzleServerContext, Event thisEvent, ClaimsPrincipal user, UserManager<IdentityUser> userManager)
        {
            PuzzleUser pUser = await PuzzleUser.GetPuzzleUserForCurrentUser(puzzleServerContext, user, userManager);
            return await GetTeamForPlayer(puzzleServerContext, thisEvent, pUser.ID);
        }

        /// <summary>
        /// Returns the registration details for the given player in the given event
        /// </summary>
        /// <param name="dbContext">Current PuzzleServerContext</param>
        /// <param name="thisEvent">The event that's being checked</param>
        /// <param name="user">The user being checked</param>
        /// <returns>The user's registration details for this event</returns>
        public static async Task<PlayerInEvent> GetPlayerInEvent(PuzzleServerContext dbContext, Event thisEvent, PuzzleUser user)
        {
            if (user == null)
            {
                return null;
            }
            return await dbContext.PlayerInEvent.Where(p => p.PlayerId == user.ID && p.EventId == thisEvent.ID).FirstOrDefaultAsync();
        }
    }
}
