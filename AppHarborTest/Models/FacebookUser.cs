using System;

namespace AppHarborTest.Models
{
    public class FacebookUser
    {

        public long FacebookId { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }


        public string Name { get; set; }
    }
}