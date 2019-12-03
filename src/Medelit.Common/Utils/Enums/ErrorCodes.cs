using System;
using System.Collections.Generic;
using System.Text;

namespace Medelit.Common
{
    public static class MessageCodes
    {
        public const string ERROR_OCCURED = "An error occured while processing your request.";
        public const string EMAIL_ALREADY_TAKEN = "Email already taken.";
        public const string NO_CHANGES_SAVED = "The record has been changed by another process. No changes were saved.";
        public const string API_DATA_MISSING = "No data passed to API.";
        public const string API_DATA_INVALID = "Invalid data passed to API.";
        public const string API_KEY_MISSING = "The API KEY was missing.";
        public const string API_KEY_INVALID = "The API KEY was invalid.";
        public const string RECORD_NOT_FOUND = "Record not found.";


        public const string SUCCESS = "success";
    }

}
