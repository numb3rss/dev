using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAppService.BP
{
    public class MessageService : IMessageService
    {
        public string Get()
        {
            return "test container docker in app service !!!";
        }
    }

    public interface IMessageService
    {
        string Get();
    }
}
