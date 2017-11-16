using System;
using System.Collections.Generic;

namespace QuickstartIdentityServer
{
    public static class UserEmergencyLog
    {
        public static List<UserEmergency> UserEmergencies = new List<UserEmergency>();
    }


    public class UserEmergency
    {
        public string Sub { get; set; }

        public DateTime DateTime { get; set; }
    }
}