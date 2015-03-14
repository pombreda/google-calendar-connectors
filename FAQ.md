# Frequently Asked Questions #

This FAQ contains common questions and answers about the Google Calendar Connectors.




---


**Do the Google Calendar Connectors support a native Exchange 2007 environment?**

> Yes, you might be required to create a public folder as by default Exchange 2007 will not create one unless you enable support for legacy clients. Also please read the FAQ if running IIS 7 on Windows 2008 for how to configure IIS for webdav support when writing free/busy data.

> To create a public folder database follow these instructions: http://technet.microsoft.com/en-us/library/bb123687(EXCHG.80).aspx


---




---


**How does Exchange resolve free/busy data for different clients (i.e. Legacy Outlook 2003 on Exchange 2007).**

> The Microsoft blog post: http://blogs.technet.com/b/exchange/archive/2006/10/23/3395177.aspx explains the different paths clients resolve free/busy data from the public folder or mailbox using different APIs based on the Exchange platform.


---


**What is the scope of the free/busy projection from Google Calendar that is written into the Exchange Free/Busy Store?**

> The free/busy projection is a total of 90 days of free/busy data. We store 30 days in the past and 60 days in the future.


---


**Do the Google Calendar Connectors support Exchange 2010?**

> A native Exchange 2010 environment is not supported due to the removal of WebDAV with Exchange 2010. If there is an active Exchange 2003 or 2007 server in the Exchange organization it is still possible to use the Google Calendar Connectors by writing free/busy data using the WebDAV services on the older Exchange server.


---


**I am getting an error "HTTP Error 404.11 – URL\_DOUBLE\_ESCAPED" when trying to write free/busy data via WebDAV when running IIS7.**

  * Microsoft has included in IIS 7 filter rules that by default prevent double escaping in the URL. However double escapes are used in the WebDAV calls for free/busy. For how to remove this filter option please review the following Microsoft KB Article: http://support.microsoft.com/kb/942076


---



**What are the technical requirements for running the Google Calendar Connectors in my environment?**

  * Google Apps Premier or Google Apps Education Edition
  * Exchange Server 2000, Exchange 2003 or Exchange 2003/2007 Mixed Mode or 2007 Native Mode
    * Exchange 2000 Server:
      * Windows Server 2000 SP4
      * Exchange 2000 SP3
        * A single Active Directory Forest.
        * .NET 2.0 runtime on the servers running the Web and Sync Services.
    * Exchange 2003 Server:
      * Windows Server 2003
        * Exchange 2003 SP2 or Exchange 2003/2007 Mixed Mode environment with a public free/busy store.
          * A single Active Directory Forest.
          * .NET 2.0 runtime on the servers running the Web and Sync Services.
    * Exchange 2007 Server:
      * Windows Server 2003 Server 64 Bit / Windows 2003 [R2](https://code.google.com/p/google-calendar-connectors/source/detail?r=2) 64 Bit or Windows 2008 Server 64 Bit
        * Exchange Server 2007 SP1 with a public folder database.
          * A single Active Directory Forest.
          * .NET 2.0 runtime on the servers running the Web and Sync Services.



---


**Are the Google Calendar Connectors secure?**
  * Role Accounts allow customers to define permission levels
  * Authenticated HTTPS GData feeds to talk to Google Apps
  * HTTPS support when interacting with HTTPS Google Calendar
  * TLS support for talking to LDAP
  * Configuration files support encryption

> The Google Calendar Connectors use role accounts to interact with your customer's Microsoft Exchange environment.  This allows the customer to decide what minimum level of privileges to grant the services in their environment.

> The Google Calendar Connectors also uses the authenticated GData feeds to retrieve free/busy information from Google Calendar.

> Google Calendar Connector Web Service uses HTTPS to communicate with Google Calendar when it detects Google Calendar is using HTTPS.  This means the Google Calendar Connector Web Service can operate in HTTP or HTTPS mode, depending on whether Google Calendar is in HTTP or HTTPS.  In general, Google recommends running Google Calendar and the Google Calendar Connectors in a mutual level of security; either both in HTTPS or both in HTTP.

> The Google Calendar Connector Web Service does **NOT** provide an authentication layer for choosing which requests for free/busy information and appointment details to allow or dis-allow.  Anyone who knows the URL address of your Web Service and has network access to its URL would theoretically be able to perform Microsoft Exchange free/busy look ups and, if enabled, retrieve appointment details to the same degree as any user of your Google Apps domain.


---


**What is the Google Calendar Connector Web Service URL?**
  * This is the url that Google Calendar posts the free/busy lookup requests to get Exchange user free/busy data. (i.e. https://mywebserver.acme.com/GCalExchangeLookup/Exchangequerier.aspx). This URL must be defined in your Google Apps domain before Google Calendar can retrieve Exchange free/busy data.


---


**How do I register a Google Calendar Connector Web Service URL?**

> Login to your Google Apps Domain control pannel

> [https://www.google.com/a/yourdomain.com](https://www.google.com/a/*yourdomain.com*)

  1. In the "Service settings" drop-down, select "Calendar"
  1. Scroll down to Free/Busy service
  1. Click on "Set up Calendar to use your free/busy web service"
  1. Select I am using a free/busy service at this URI:
  1. Enter the URI of the installed Calendar Connector Web Service
> For example: https://mywebserver.acme.com/GCalExchangeLookup/Exchangequerier.aspx (Please review the project [http://code.google.com/p/google-calendar-connectors/wiki/FAQ](FAQ.md) for more details and common questions)


---


**How does Google Calendar Connectors determine which Exchange users map to Google Apps user?**

  * There is a setting SyncService.LDAPUserFilter that controls what Active Directory users/contacts are in scope of the sync service.
  * The connector reads the Primary SMTP address of the Exchange user account or contact   object. We use this e-mail address as the key between Exchange and Google Apps and vice verse.


---


**My Exchange primary SMTP address is different than my Google Apps SMTP address. How do I setup the connector to understand this?**

> _Google Calendar Connector Web Service:_
    * Use the GoogleApps.GCal.DomainMapping setting in the configuration file to specify your internal SMTP domain name and your Google Apps domain name.

> _Google Calendar Connector Sync Service:_
    * Use the GoogleApps.GCal.DomainMapping setting in the configuration file to specify your internal SMTP domain name and your Google Apps domain name.


> _Google Calendar Connector Pug-in:_
    * Use the ldap.domainMap setting in the configuration file to specify your internal SMTP domain name and your Google Apps domain name. Using this setting in conjunction with ldap.whitelist and ldap.blacklist lets you control what users are in scope of the Google Calendar Connector Plug-in.


---


**What servers does the Google Calendar Connector need to connect to within my company?**

> _Web Service:_
    * If enabled to return appointment detail every server that hosts a mailbox. Make sure if you have IP whitelisting configured that the sever running the Google Calendar connectors is also include in that whitelist.
    * The web service will need to communicate via LDAP/GC to the domain controller configured in the web.config.
    * If not returning appointment detail the Web service only needs to talk to the Exchange servers configured in the web.config file.
    * If you have configured a proxy server the sync service will need to be able to talk to the configured server.


> _Sync Service:_
    * If you are running the sync service in Appointment mode, the sync service will need to communicate to every server that hosts a mailbox you are syncing.
    * If you are running the sync service in SchedulePlus mode,  the Sync service only needs to talk to the Exchange servers configured in the GoogleGCalExhangeSync.Service.exe.config file.
    * The web service will need to communicate via LDAP/GC to the domain controller configured in the web.config.
    * If you have configured a proxy server the sync service will need to be able to talk to the configured server.


---


**Do I need to setup the Google Calendar Connector Web Service in the DMZ and open ports so that Google can communicate with the Google Calendar Connector Web Service service?**

> No. The communication to the web service is between the client web browser and the Google Calendar Connector Web service, not a Google cloud server and the web service. The URL to this server can be an intranet only based DNS address.

> If you want Google Calendar Exchange free/busy to work outside your intranet, via the Internet, then you can setup the server in a DMZ and open the firewall to access this server. The DNS address for this server should be accessible and the same for the intranet and Internet clients. For security reasons you should ensure the web service is running over SSL and only port 443 is open on your firewall to this server.


---


**Does the Google Calendar Connector Sync service synchronize existing Exchange Calendar data into users Google Calendar?**

> No, the Google Calendar Sync service only is designed to synchronize Google Calendar free/busy data into your Exchange environment. It does not sync appointment details or other meeting data, just free/busy status for the meeting time.

> There is functionally with the Google Calender Connector Web service to retrieve in real time Exchange free/busy data within Google Calendar.

> A separate Google product: Google Calendar Sync is a client desktop application that syncs Outlook Calendar with a Google Calendar. This tool runs on an end user client desktop. For more information go here: http://www.google.com/support/calendar/bin/answer.py?answer=89955.


---


**I host more than a single SMTP domain in my Exchange environment. How do I enable these other SMTP domains hosted in Exchange so that free/busy can be seen in Google Calendar?**

  * We match all domain names so all SMTP addresses will be passed to the connector.

---


**I need to sync both Exchange mailboxes and contacts/mail enabled users. How do I setup the sync service to run in both modes?**

  * The sync service does not support running in both a SchedulePlus and Appointment mode at the same time. We recommend you install the sync service on two separate servers. Configure each server for one of the respective sync modes.


---


**I have users configured in both Google Calendar and Exchange. What free/busy data will be displayed?**

  * The first service to respond to the free/busy request is displayed. If the user is a Google Apps user then Google apps data will be displayed. If there is a no Google Apps user or the account is disabled, Exchange data will be displayed. If the user is both a Google Apps user and Exchange user, only Google Apps free/busy data will be displayed.


---


**How do I force a complete re-sync of the Google Calendar Sync Service?**

  * Delete the file UserModifiedTimes.xml from C:\google\data and restart the Google Calendar Sync Service. This will force a full re-sync for each user.

**Can the Google Calendar Connectors sync a Google Calendar Resource's free/busy data from Google Apps into Exchange?**

> Yes if using the Google Calendar Sync Service!

  1. Create an Exchange contact with the e-mail address of the Google Apps Calendar Resource.
  1. Define a timezone for the Resource calendar. To set the timezone login as an Admin of the Gooogle Apps domain and load Google Calendar. Added the Resource calendar to your "My Calendars" by entering the e-mail address for the resource. Click on Settings | Calendars | click on the Resource calendar. Under "Calendar Time Zone:" select a timezone and click Save.
  1. Configure the Google Calendar Connector Sync service using SchedulePlus mode. Ensure the contact object is in scope of the sync service.


---


**How is the free busy status mapped from the Google Calendar to Exchange?**

The GCal meetings can have 5 possible statuses:
  1. Cancelled.
  1. Confirmed.
  1. Tentative.
  1. Not set.
  1. Any unknown value.

The attendees of a meeting in GCal can have 6 possible statuses:
  1. Accepted.
  1. Declined.
  1. Invited.
  1. Tentative.
  1. Not set.
  1. Any unknown value.

The combination of those (event status X attendee status) are mapped to the following Exchange free busy statuses:
  1. Free.
  1. Busy.
  1. Tentative.
  1. OOF.

First several special cases:
  1. Nothing is mapped to the Exchange OOF, since no such information is available from GCal.
  1. The meeting status is not set when the projection is just free busy. In such case, it is considered same as Confirmed.
  1. The attendee status is not set when the projection is just free busy. In such case, it is considered same as Accepted.
  1. Unknown value for meeting status is treated as Confirmed. There is no perfect match here, but this should not happen unless calendar introduces new status.
  1. Unknown value for attendee status is treaded as Accepted. Same logic as above.

So here are the rules
  1. If the meeting is Cancelled, regardless of the attendee status, the time is considered Free.
  1. If the attendee status is declined, regardless of the meeting status, the time is considered Free.
  1. If the meeting is not Cancelled, and the attendee status is Invited, the time is considered Tentative (to match Exchange/Outlook behavior).
  1. If the meeting is Tentative, and the attendee status is not declined, the time is considered Tentative.
  1. If the meeting is Confirmed and the attendee status is accepted, the time is considered Busy.
  1. If the meeting is Confirmed and the attendee status is tentative, the time is considered Tentative.

Here is the mapping in table format:

| |Confirmed |Not set |Unknown |Tentative |Cancelled |
|:|:---------|:-------|:-------|:---------|:---------|
|Accepted |Busy |Busy |Busy |Tentative |Free |
|Not set |Busy |Busy |Busy |Tentative |Free |
|Unknown |Busy |Busy |Busy |Tentative |Free |
|Invited |Tentative |Tentative |Tentative |Tentative |Free |
|Tentative |Tentative |Tentative |Tentative |Tentative |Free |
|Declined |Free |Free |Free |Free |Free |



---


**Can I run multiple instances of the Google Calendar Connectors on the same server?**

  * [Running Multiple Sync Service Instances](http://code.google.com/p/google-calendar-connectors/wiki/SyncServiceGuide#Multiple_Sync_Service_Instances)

  * [Running Multiple Web Service Instances](http://code.google.com/p/google-calendar-connectors/wiki/WebServiceGuide#Multiple_Web_Service_Instances)