using System.Collections.Generic;

namespace JustineCore.Entities
{
    public class UserProblemAccount
    {
        public ulong Id { get; set; }
        public List<UserProblem> Problems { get; set; } = new List<UserProblem>();
    }
}