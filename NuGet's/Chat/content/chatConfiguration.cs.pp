using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace $rootnamespace$
{
    public class chatConfigurationClass : ConfigurationSection
    {
        public static chatConfigurationClass GetConfig()
        {
            return ConfigurationManager.GetSection("chatConfiguration") as chatConfigurationClass;
        }

        [ConfigurationProperty("service", IsRequired = true)]
        public chatConfigurationClassElement service
        {
            get
            {
                return this["service"] as chatConfigurationClassElement;
            }
        }

        [ConfigurationProperty("chatHub", IsRequired = true)]
        public chatConfigurationClassElement chatHub
        {
            get
            {
                return this["chatHub"] as chatConfigurationClassElement;
            }
        }
    }

    public class chatConfigurationClassElement : ConfigurationElement
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