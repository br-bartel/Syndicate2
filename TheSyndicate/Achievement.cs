using System;

namespace TheSyndicate
{
    public class Achievement
    {
		public string Id;
		public bool State;
		public string Completed;
		public string Hint;
        public Achievement(string id, bool state, string completed, string hint)
        {
           this.Id = id;
		   this.State = state;
		   this.Completed = completed;
		   this.Hint = hint;
        }
    }
}
