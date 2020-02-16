using System;

namespace TheSyndicate
{
    public class Achievement
    {
		public string Id;
		public bool State;
		public string Hint;
        public Achievement(string id, bool state, string hint)
        {
           this.Id = id;
		   this.State = state;
		   this.Hint = hint;
        }
    }
}
