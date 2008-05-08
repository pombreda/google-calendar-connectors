/* Copyright (c) 2008 Google Inc. All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Google.GCalExchangeSync.Library.Util
{
    /// <summary>
    /// Utilities for getting URLs from Exchange
    /// </summary>
    public class ExchangeUtil
    {
        /// <summary>
        /// Returns the mailbox of the current user on the exchange server
        /// </summary>
        /// <param name="exchangeServer">The exchange server to use</param>
        /// <param name="user">The user to get the mailbox URL for</param>
        /// <returns>The Mailbox URL for the user</returns>
        public static string GetMailboxUrl(string exchangeServer, string user)
        {
            if ( user.Contains( "@" ) )
            {
                string[] tokens = user.Split( '@' );
                user = tokens[ 0 ];
            }

            return string.Format("{0}/exchange/{1}", exchangeServer, user);
        }

        /// <summary>
        /// Get the default calendar URL for the user
        /// </summary>
        /// <param name="exchangeServer">Exchnage Server to use</param>
        /// <param name="user">The Exchange user to use</param>
        /// <returns>The default calendar URL for the user</returns>
        public static string GetDefaultCalendarUrl( string exchangeServer, ExchangeUser user )
        {
            string path = user.ProxyAddresses;

            if ( path.StartsWith( "SMTP:" ) )
                path = path.Substring( "SMTP:".Length );

            return string.Format( "{0}/exchange/{1}/calendar", exchangeServer, path );
        }

        /// <summary>
        /// Get the default calendar URL for the user
        /// </summary>
        /// <param name="exchangeServer">Exchnage Server to use</param>
        /// <param name="email">The email address to use</param>
        /// <returns>The default calendar URL for the user</returns>
        public static string GetDefaultCalendarUrl(string exchangeServer, string email)
        {
            return string.Format( "{0}/exchange/{1}/calendar", exchangeServer, email );
        }
    }
}