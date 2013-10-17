using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitter
{
    public class Tweet
    {

        private string userName;
        private string status;
        private Uri image;

        public string UserName
        {
            get { return userName; }
        }

        public string Status
        {
            get { return status; }
        }

        public Uri Image
        {
            get { return image; }
        }

        public Tweet(string userName, string status, Uri image)
        {
            this.userName = userName;
            this.status = status;
            this.image = image;
        }
    }
}
