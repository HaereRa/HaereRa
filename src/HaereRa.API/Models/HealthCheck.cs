using System.Collections.Generic;
namespace HaereRa.API
{
    public class HealthCheck
    {
        public bool Basic { get; set; }
        public Dictionary<string, bool> Detailed { get; set; }
    }
}
