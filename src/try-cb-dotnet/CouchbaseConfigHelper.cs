using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace try_cb_dotnet
{
    public class CouchbaseConfigHelper
    {
        public CouchbaseConfigHelper()
        {
        }

        private static CouchbaseConfigHelper instance = null;
        public static CouchbaseConfigHelper Instance
        {
            get { if (instance == null) { instance = new CouchbaseConfigHelper(); } return instance; }
        }

        public string Bucket
        {
            get
            {
                return ConfigurationManager.AppSettings["couchbaseBucketName"];
            }
        }

        public string Server
        {
            get
            {
                return ConfigurationManager.AppSettings["couchbaseServer"];
            }
        }

        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["couchbasePassword"];
            }
        }

        public string User
        {
            get
            {
                return ConfigurationManager.AppSettings["couchbaseUser"];
            }
        }
    }
}