using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class VKfriends
    {
        public Response response;
        public Error error;
    }

    public class Error
    {
        public string error_msg;
    }

    public class Response
    {
        public int count;
        public Item[] items;
    }

    public class Item
    {
        public string first_name;
        public string last_name;
        public string bdate;
    }
}
