using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PasswdAPI
{
    public class PasswdErrors
    {
        public static JObject errorHandler(PasswdError error)
        {
            JObject errorResponse = new JObject();
            errorResponse.Add("StatusCode", 500);

            switch (error)
            {
                case PasswdError.FileNotFound:
                    errorResponse.Add("Error Message", @"One of the files could not be found.
Please check the appsettings.json and restart the service.");
                    break;
                case PasswdError.FileDeleted:
                    errorResponse.Add("Error Message", @"One of the files was deleted.
Please check your directories, update the appsettings.json and restart the service.");
                    break;
                case PasswdError.FileRenamed:
                    errorResponse.Add("Error Message", @"One of the files was renamed.
Please update the appsettings.json and restart the service.");
                    break;
                case PasswdError.BadData:
                    errorResponse.Add("Error Message", @"One of the files contains bad or corrupted data. Tables could not be built.
Please make sure data is clean and formatted correctly before restarting service.");
                    break;
            }
            return errorResponse;
        }

        public enum PasswdError
        {
            NoError = 0,
            FileNotFound = 1,
            FileDeleted = 2,
            FileRenamed = 3,
            BadData = 4
        }

    }
}
