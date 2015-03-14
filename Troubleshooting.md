# Troubleshooting #

**I am not able to resolve free/busy data from an Exchange account on Exchange 2007?**

> Check the event logs on the CAS servers for issues related the Availability Service, Event IDs 4003,4004.

> The most common issues related to 4003 errors is related to IIS configurations. The Availability Service always uses HTTP to request free/busy data from the public folder store. If you have only a single Exchange server this always happens as the Exchange server is hosting both the CAS and Mailbox roles.

> There are two possible solutions:
> Change the require SSL option to not require SSL (assume the risk of traffic being sent over HTTP)

> NOTE: **As part of this change you should consider blocking all port 80 traffic from the Internet for security purposes.**

> Setup a second Exchange 2007 server to host the Public Folder store. Configure IIS on the secondary server so that the /Public IIS web directory allows traffic via HTTP

> For more details please refer to the Exchange Team Blog post:  http://blogs.technet.com/b/exchange/archive/2008/12/04/3406595.aspx

> Event 4003 Details: http://www.microsoft.com/technet/support/ee/transform.aspx?ProdName=Exchange&ProdVer=08.00.0632.000&EvtID=4003&EvtSrc=MSExchange+Availability&LCID=1033

> Event 4004 Details: http://www.microsoft.com/technet/support/ee/transform.aspx?ProdName=Exchange&ProdVer=08.00.0632.000&EvtID=4004&EvtSrc=MSExchange+Availability&LCID=1033



**I am not able to retrieve any free/busy data from Google Calendar for my Exchange users?**

  * Have you provided Google Enterprise support with the your URL of the server hosting the Web Service? You must provide the web service URL to the Google Enterprise Support Organization (ESO). ESO will ensure your Google Apps domain is configured for the Google Calendar Connector interoperability and that the URL is properly entered in your account settings on the Google Calendar server. (Example URL value: https://mywebserver.acme.com/GCalExchangeLookup/Exchangequerier.aspx)
  * Check the webservice.log to determine if the web service is receiving the request?
  * Enable DEBUG for the web service to see the details of the request being passed and why it might be failing to look up the Exchange user.

**I am not able to sync free/busy data for Google Calendar Resources into a contact object in Exchange? In the log I get the error: "This user may not have activated their account or may be inactive."**

  * Before the calendar can be read successfully a timezone must be defined in the Resource calendar. To set the timezone login as an Admin of the Gooogle Apps domain and load Google Calendar. Added the Resource calendar to your "My Calendars" by entering the e-mail address for the resource. Click on Settings | Calendars | click on the Resource calendar. Under "Calendar Time Zone:" select a timezone and click Save.

> If you don't know the e-mail address for the Resource open the Google Apps control panel select  Service Settings | Calendar | click Resources. Click on the resource name. It will display the e-mail address for the resource.

**Lookups from Google Calendar to Exchange time out after 30 seconds**
  * Make sure the URL address registered with Google Apps for your domain is correct.
  * Make sure you include "Exchangequerier.aspx" as part of the url.
  * The Google Calendar connector uses a 30 second timeout - make sure the Web Service is not taking a long time to respond.
> If Web Service lookups are taking too long:
    * Disable Appointment Lookup if it is not needed (set the Exchange.EnableAppointmentLookup option to false)
    * Enable the Optimized Free Busy lookup option (set the Exchange.EnableOptimizedFreeBusy option to true)
    * Ensure that the Web / Sync service have fast connections to the Exchange Server
    * Make sure that HTTP keep-alive connections are possible between the Web / Sync service and the Exchange Server

**I get the error Calendar does not exist when I try to look up an Exchange user in Google Calendar?**

  * This could be due to the fact the web service was unable to locate an Exchange user or contact based on the addressed you entered. Please enable DEBUG on the web service log. This will help you determine why the user look up is failing.
  * The web browser is not able to communicate to the Google Calendar web service
  * Make sure the URL address registered with Google Apps for your domain is correct
  * Make sure you are not entering the internal Exchange e-mail address and are using an e-mail address based on the Google Apps domain.
  * You might need to configure GoogleApps.GCal.DomainMapping if you have a different internal e-mail address domain from your Google Apps domain name.

**Free / Busy information for Exchange users are not available in Google Calendar**
  * Make sure the Exchange users do not exist in Google Apps or if they do the user is disabled
  * If the user needs to be enabled for other reason, configure the user's calendar to not be shared.

**Log files show that the Google Calendar feed for a user cannot be fetched (error 404)**
  * Make sure the user exists in Google Apps
  * Make sure the url request for the user is using the proper Google Apps e-mail address and not the internal Exchange e-mail address.
  * Make sure the user has agreed to the Google Apps terms of service

**Log files show Network connections are timing out**
  * Run netstat from the shell and make sure that connections are not being leaked. This may indicate that HTTP keep alive connections cannot be created between the Web / Sync service and Exchange.

**Internal exchange email addresses differ from external Google Apps email addresses**
  * Add a domain mapping with the GoogleApps.GCal.DomainMapping setting

**Log files show error 503 when reading / writing Schedule+ Free/Busy information**
  * Make sure the exchange server has a replica of the Free/Busy Public Folder - the Exchange.FreeBusyServerName option can be used to set a different Exchange Server for the Free/Busy replica

**Log files show error 401 when accessing a users mailbox**
  * Make sure the permissions for the on the mailbox are set correctly
  * Check the proper permissions for the service account Exchange.!GCalQueryUser and Exchange.!GCalQueryAdmin are set

**Cannot connect to Exchange Server / Google Calendar**
  * If you require the use of a proxy server for accesing the network, you may need to configure the proxy settings in the .config file
  * Authenticating proxy support will be added in the near future

**I am getting 403 errors when running the Google Calendar Sync service?**

  * If you are getting 403 errors when trying to sync free/busy from Google Calendar. Enable DEBUG logging on the sync service. You might need to configure SMTP domain mapping if your internal primary SMTP address on your Exchange user does not match the e-mail address in Google Apps.
  * The user account in Google Apps is configured not to share free busy data. To verify sharing is enabled in Google Calendar goto Settings | Calendars | Shared: Edit Settings. Make sure "Share this calendar with others should be enabled". To share appointment details "Share this calendar with everyone in this domain" should enabled and set to "See all event details".

**I need more options for controlling the logs**
  * The logging uses the log4net library - the settings can be modified in the .config file. See http://logging.apache.org/log4net/release/config-examples.html


**The Google Calendar Connector Sync service is trying to sync every user in my Active Directory.**

  * Configure the SyncService.LDAPUserFilter in the config file. This filter allows you to filter the users in scope of the sync service.


**I am getting 401 access denied error messages when trying to read or write free/busy or appointment data?**

  * Verify that you have configured the Windows service accounts with the proper permissions within Exchange to read and write free/busy or appointment data.
  * If using IP white-lists on your Exchange servers running IIS, verify the server you are communicating with includes the IP of the server that is running the web/sync service in its white-list.


**I get the error: !"Error writing appointment ---> System.Net.WebException: The remote server returned an error: (409) Conflict"?**

  * Verify the user has a properly configured legacyExchangeDN
  * Verify that you have configured the Windows service accounts with the proper permissions within Exchange to read and write free/busy or appointment data.


**The Google Calendar Sync service crashes or stops after starting?**

  * Review the config file to make sure all the xml syntax is correct. Improper syntax will cause the service to crash on start.


**The web service is not creating any log files?**

  * Make sure NETWORK SERVICE has "MODIFY" privileges to the file system paths where the log files are configured to be written.


**The Sync service is not creating any log files?**

  * Make sure SYSTEM has "MODIFY" privileges to the file system paths where the log files are configured to be written.


**The web or sync service is not encrypting the config file?**

> _Sync:_
    * Make sure you have enabled Configuration.EncryptOnNextRun to "True"
    * For the sync service, make sure SYSTEM has "MODIFY" privileges to the file system path where the config file is stored.


> _Web:_
    * For the web service, make sure NETWORK SERVICE has "MODIFY" privileges to the file system path where the config file is stored.


> _Plug-In:_
    * Make sure the plug-In is running as a Domain User account that has a "User" certificate installed in the User's Personal Certifate store.
    * Make sure the config.txt is configured for the value win.certname.


**I have a user that always shows empty free/busy when viewing it from Google Calendar, but there is free/busy data in Exchange?**

  * Most likely you have a Google Apps account setup for this user and it currently enabled and sharing free/busy data. The calendar free/busy look up tool is getting the data from Google Calendar and not from Exchange. You can disable the account so that it does not share free/busy data. This will force the Calendar free/busy look up function to take the data returned from the Web service.


**I get a security warning every time I try to look up free/busy data in Google Calendar?**

  * This is caused due to a mismatch between http vs https connections between Google Calendar and the web service. For example you have loaded Google Calendar via HTTP but you have SSL enabled on the web service so the request being sent to the web service is HTTPS. Or vice verse you have load Google Calendar as HTTPS but your web service is configured for HTTP.

> NOTE: If you have configured the web service for SSL but are loading Google Calendar as HTTP please work with you Enterprise Support Organization to configure Google Apps to always us SSL for Calendar.


**I am getting the error: "ERROR Google.GCalExchangeSync.Library.Util.ModifiedDateUtil - System.Xml.XmlException: 'None' is an invalid XmlNodeType." How do I fix this?**

  * You might see this error when setting up the connectors for the first time. Until a valid sync occurs, the xml file might be incomplete. To resolve this simply delete the UserModifiedTimes.xml from C:\google\data. This file will be rebuilt on the next sync service execution.


**How do I verify if the Google Calendar is using HTTPS (SSL) for Exchange free/busy requests being sent to the Google Calendar Connector Web Service?**

> Enable debug logging for the Google Calendar Connector Web Service. The debug logging will print the requester URL.

> To enable DEBUG logging for the Google Calendar Connector Web Service:
    * Edit web.config in C:\inetpub\wwwroot\GCalExchangeLookup
    * Find the default entry: 

&lt;level value="INFO" /&gt;


    * Replace this line with: 

&lt;level value="DEBUG" /&gt;


    * Load Google Calendar issue a lookup for an Exchange user
    * Review the web service logs C:\google\logs\webservice.log
    * Search for "Request URL" verify the URL is HTTPS


**Google Calendar Connector Plug-In is not getting free/busy requests from the Microsoft Calendar Connector? Event ID 6001 Unable to find Requestor in Directory.**

> To help debug the issue run the "C:\Program Files\Exchsrvr\bin\calcon.exe" interactively. If you see the request but see the following error, make sure the System Attendant Account has a valid GWISE: proxy address. For more details you can review the following article: http://support.microsoft.com/kb/312483

> To resolve the issue edit via ADSIEdit the Microsoft System Attendant object in Active Directory and set a proxyaddress entry such as GWISE:domain.domain to enable routing. The value is not really relevant so long as you don't have any other Groupwise infrastructure. Use the same value you defined when configuring the Groupwise Connector in Exchange System Manager.

  * Errors:
    * Unable to find Requestor in Directory
    * HrProcessNextTransaction Failed

**If your Google Apps domain is setup to use SSL but the Google Calendar Connector Web Service is configured for HTTP you will get the error: "Calendar doesn't exist or isn't shared" when requesting free/busy data for an Exchange user. When reviewing the webservice.log file there are no errors. However, you might notice the `GCalExchangeLookup.ExchangeQuerier - Referrer URL` is empty.**

> The web browser does not pass the referrer URL when it is passed from a secure enabled site to a none secure site. We use the referrer header to know if you are using Google Calendar via HTTP or HTTPS. By default in v 1.1 if no referrer header is found we use the HTTP as the default URL for Google Calendar.

> This behavior is fixed in the Google Calendar Connector Web Service 1.2 release.

> If you are running v 1.1 of the Google Calendar Connector Web Service and your Google Apps Domain is configured to always use HTTPS since the browser won't  set any referrer header the data will not be posted back to Google Calendar properly. To work around this problem you can force the Web Service to always post to HTTPS Google Calendar URL by adding the following property to the web.config file: `<add key="WebService.DefaultGoogleCalendarSSL" value="true"/>`. _NOTE: The case of the value is "true". There is a bug where if its not lower case the value is not applied._

**I get the message: "Although this page is encrypted, the information you have entered is
to be sent over an unencrypted connection and could easily be read by a third party. Are you sure you want to continue sending this information?" How can I fix this?**

> This is due to the fact your are accessing Google Calendar via HTTPS and the URL defined for your Google Calendar Connector Web service is HTTP. When you enter the e-mail address to look up free/busy data, Google Calendar will send an HTTP or HTTPS request to the URL you defined as your web service URL. Since your URL is configured for HTTP and not HTTPS the browser will notify you are submitting and unencrypted request from a secure page.

**Google Calendar Connector Plug-in is working with OWA so you can not see any free/busy data, all that is displayed is hash marks?**

> This can be caused by the System Attendant not being configured for a proxy address for the Groupwise domain. You will see an error if you run calcon.exe in a cmd window manually. You will see the message "Unable to find requestor in the directory"  The problem is caused by the System Attendant account not having any GroupWise addressing.

> The root cause of this problem is the System Attendant account does not contain a proxy address that corresponds to the GWISE proxy address configuration. This typically occurs if you removed the GWISE addressing from the default recipient policy configuration for Exchange 2003.

> You can either enable GWISE addressing in the default recipient policy or manually add a GWISE proxy address to the System Attendant account. An example would be: (GWISE:domain.com..systemattendant@domain.com)

> You can verify the proxy addresses of the System Attendant service using the ADSI Edit snap-in. Start ADSI Edit, connect to the configuration naming context of a domain controller, and then browse to the following object: CN=Microsoft System Attendant,CN=Your Server Name,CN=Servers,CN=Your Administrative Group,CN=Administrative Groups,CN=Your Organization Name,CN=Microsoft Exchange,CN=Services,CN=Configuration,DC=Your Domain,DC=com

**Web service logs show 404 error while getting free\busy for an Exchange user.**

> If you are using IIS 7 and IIS logs show 404.11 error, you need to allow double escaping. Please see http://support.microsoft.com/kb/942076 to know how to allow double escaping.

**I am getting "Invalid post back url" error while running Google Calendar Web Service.**

> To resolve this you need to specify a regular expression in config file which matches the url being reported as invalid. Add following parameter in the config file.
> 

&lt;add key="WebService.PostBackUrlRegEx" value="^https?://www.google.com/calendar/([a-zA-Z0-9/\_\\.\\-]+/)?mailslot$" /&gt;

