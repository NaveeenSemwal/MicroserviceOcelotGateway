using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetBook.Contracts.V1.Request
{
    public class PostRequest
    {
        public string PostId { get; set; }
        public string Name { get; set; }
    }
}
