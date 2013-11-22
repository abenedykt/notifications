using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatConfig
{
    public class ChatConfigurationClass : ConfigurationSection
    {
        public static ChatConfigurationClass GetConfig()
        {
            return ConfigurationManager.GetSection("chatConfiguration") as ChatConfigurationClass;
        }

        [ConfigurationProperty("service", IsRequired = true)]
        public ChatConfigurationClassElement Service
        {
            get
            {
                return this["service"] as ChatConfigurationClassElement;
            }
        }
    }

    public class ChatConfigurationClassElement : ConfigurationElement
    {
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get
            {
                return this["url"] as string;
            }
        }
    }
}
