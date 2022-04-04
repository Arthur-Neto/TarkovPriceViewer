using System.Net;
using System.Text;

namespace TarkovPriceViewer
{
    internal class TPVWebClient : WebClient
    {
        private readonly int timeout = 5000;

        public TPVWebClient()
        {
            Encoding = Encoding.UTF8;
            Proxy = null;
        }
        public TPVWebClient(int _timeout)
        {
            Encoding = Encoding.UTF8;
            Proxy = null;
            timeout = _timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            request.Timeout = timeout;

            return request;
        }
    }
}
