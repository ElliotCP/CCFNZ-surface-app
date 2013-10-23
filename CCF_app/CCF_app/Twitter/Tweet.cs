namespace CCF_app.Twitter
{
    //Create a data item class for the tweet items for TweetList
    public class Tweet
    {
        private readonly string _status;
        private readonly string _timeString;
        private readonly string _userName;

        public Tweet(string userName, string status, string timeString)
        {
            _userName = userName;
            _status = status;
            _timeString = timeString;
        }

        public string UserName
        {
            get { return _userName; }
        }

        public string Status
        {
            get { return _status; }
        }

        public string TimeString
        {
            get { return _timeString; }
        }
    }
}