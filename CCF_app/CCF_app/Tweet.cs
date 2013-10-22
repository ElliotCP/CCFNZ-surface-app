using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCF_app
{
    //Create a data item class for the tweet items for TweetList
    public class Tweet
    {

        private string userName;
        private string status;
        private string timeString;

        public string UserName
        {
            get { return userName; }
        }

        public string Status
        {
            get { return status; }
        }

        public string TimeString
        {
            get { return timeString; }
        }

        public Tweet(string userName, string status, string timeString)
        {
            this.userName = userName;
            this.status = status;
            this.timeString = timeString;
        }

    }
}
