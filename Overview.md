# Google Calendar Connector Kit Overview #

## Contents ##

  * [Introduction](#Introduction.md)
  * [Connectors Overview](#Overview.md)
    * [Google Calendar Connector Web Service](#Google_Calendar_Connector_Web_Service.md)
    * [Google Calendar Connector Sync Service](#Google_Calendar_Connector_Sync_Service.md)
  * [Security Considerations](#Security_Considerations.md)
  * [Additional Resources](#Additional_Resources.md)
  * [Copyright Notices](#Copyright_Notices.md)

# Introduction #

The Google Calendar Connector Kit is a set of tools that enable users to look up free/busy information for their domain across Microsoft Exchange and Google Apps. Google Apps users can see free/busy information for Microsoft Exchange users from within the Google Calendar resource scheduling interface, and Microsoft Exchange users can see free/busy information for Google Apps users from within the Microsoft Outlook calendar scheduler.

The Google Calendar Connector Kit is a set of server-installed tools that require knowledgeable installation and configuration, tailored to the particulars of a customer's environment.  These tools are only the **technological part** of the integration experience.

Before proceeding to install the Google Calendar Connector Kit, please read and understand the important [Security Considerations](#Security_Considerations.md) outlined below.

# Overview #

The Google Calendar Connector Kit contains two separate services, which need to be installed and configured in order to provide two-way free/busy scheduling.  These services are:

  * The Google Calendar Connector Web Service
  * The Google Calendar Connector Sync Service

The Web Service provides free/busy scheduling for Google Apps users, while the Sync Service provides free/busy scheduling for Microsoft Exchange users.

**Why are there two different services for Microsoft Exchange users?**

The easy answer is that there are scenarios where each service might provide a better user experience. The Google Calendar Connector Sync Service and the Google Calendar Connector Plug-In both provide free/busy scheduling for Microsoft Exchange users. The Sync Service may make more sense for an environment where synchronizing free/busy information into Exchange is efficient for the large majority of users. The Plug-In may make more sense in other cases.

The diagrams below illustrates the similarities and differences between the components of the Google Calendar Connector Kit:

![http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/comparison.png](http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/comparison.png)

![http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/features.png](http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/features.png)


**NOTE:** A native Exchange 2010 environment is not supported due to the removal of WebDAV with Exchange 2010. If there is an active Exchange 2003 or 2007 server in the Exchange organization it is still possible to use the Google Calendar Connectors by writing free/busy data using the WebDAV services on the older Exchange server.

# Google Calendar Connector Web Service #

The Google Calendar Connector Web Service allows Google Apps users to retrieve free/busy information for Microsoft Exchange users. This tool is an ASP.NET web application that queries the Microsoft Exchange free/busy store in real-time, on behalf of Google Calendar. The ASP.NET web application should be installed on a system that has access to your Exchange server and Active Directory.

Below is a diagram illustrating the operation of the Web Service:

![http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/web_service.png](http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/web_service.png)

The following is a step-by-step process overview of the Web Service:

  1. A user opens the Google Calendar Scheduler dialog, and enters the e-mail address of a Microsoft Exchange user, user@acme.com. **(_See Security Note_)**
  1. Google Calendar checks to see if external free/busy look up is enabled for the current Google Apps domain, and if so, it finds the Intranet or Internet accessible URL of the Google Calendar Connector Web Service that has been installed on your server. **(_See Security Note_)**
  1. Google Calendar uses HTTP POST to send a free/busy look up request for user@acme.com to the Google Calendar Connector Web Service. **(_See Security Note_)**
  1. The Google Calendar Connector Web Service parses the free/busy request and issues an Active Directory LDAP query for the user account that matches user@acme.com. **(_See Security Note_)**
  1. Microsoft Active Directory returns a user object to the Google Calendar Connector Web Service which contains the Exchange mailbox name and Display Name for user@acme.com.
  1. The Google Calendar Connector Web Service parses the returned LDAP results and issues a single WebDAV query to the Exchange free/busy store. **(_See Security Note_)**
  1. The WebDAV query returns with free/busy information for the matching user, assuming proper access privileges.
  1. The Google Calendar Connector Web Service retrieves appointment data for events occurring within the free/busy query's date range, assuming access privileges have been enabled. **(_See Security Note_)**
  1. The Google Calendar Connector Web Service merges the separate appointment details with the free/busy information and Display Name of the user.
  1. The Google Calendar Connector Web Service uses HTTP POST to send the combined free/busy and appointment details back to Google Calendar's Scheduler. **(_See Security Note_)**
  1. Google Calendar's Scheduler receives the results and displays the data appropriately in Scheduler view.

Notes about data security and access permissions:

  * 1. Any Google Apps user in your domain.
  * 2. The Google Apps domain administrator or Google defines the URL of the Google Calendar Connector Web Service.
  * 3. The Google Calendar Connector Web Service must be accessible by the user's browser.
  * 4. The LDAP query is executed with standard LDAP logon credentials.
  * 6. The WebDAV request is issued with Exchange user permissions sufficient for read-only access to the free/busy store.
  * 8. The WebDAV request is issued with Exchange user permissions sufficient to read event details from another user's mailbox.
  * 9. If the event has been marked as "private", the event details will not be returned.
  * 11. The merged free/busy and appointment data is not permanently stored on Google servers, but due to javascript limitations, it is routed through a Google server on its return to the user's browser.

# Google Calendar Connector Sync Service #

The Google Calendar Connector Sync Service allows Microsoft Exchange users to see free/busy information for Google Apps users. The sync service is a Windows service written in C# that periodically queries Google Apps for updated free/busy information and writes the changes directly into Exchange. The changes are written directly into the Exchange free/busy store.

When a Microsoft Exchange user looks up free/busy information for a Google Apps user, the Exchange server accesses the local free/busy information written by the sync service. The Google Calendar Connector Sync Service should be installed on a server that has network access to your Active Directory server and your Microsoft Exchange server.

![http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/sync_service.png](http://google-calendar-connectors.googlecode.com/svn/trunk/connectors/docs/wiki/sync_service.png)

Google Calendar Connector Sync Service process overview:

  1. On a configurable and periodic basis (default 15 minutes), the Google Calendar Connector Sync Service queries Active Directory using LDAP for the list of users who are defined as Google Calendar users. This list of users is defined by an custom LDAP filter in the Google Calendar Connector Sync Service configuration file. **(_See Security Note_)**
  1. Active Directory returns to the Google Calendar Connector Sync Service each user and their e-mail address that matches the LDAP query.
  1. The Google Calendar Connector Sync Service uses the Google Calendar GData API (HTTP/XML) to request free/busy information from Google Calendar for the users match by the LDAP query. **(_See Security Note_)**
  1. The Google Calendar Connector Sync Service writes new free/busy messages to the Exchange free/busy store via WebDAV.
  1. The Google Calendar Connector Sync Service generates an XML file that contains the last time each user was synchronized. On the next scheduled synchronization a check is performed to determine if the user's calendar data has changed or not. If no changes are detected no data is downloaded for that user.

Typical use case:

  1. A Microsoft Exchange user looks up free/busy information for a Google Apps user within Microsoft Outlook.
  1. Microsoft Outlook issues a free/busy look up request to the Exchange free/busy store for the Google Apps user.
  1. Because the Google Calendar Connector Sync Service has populated the Exchange free/busy store with free/busy messages for the Google Apps users, relevant free/busy data is returned to Microsoft Outlook.

Notes about data security and access permissions:

  * 1. The Google Calendar Connector Sync Service needs access to an LDAP login and password with standard "Domain User" privileges.
  * 3. The Google Calendar Connector Sync Service authenticates using a Google Apps Administrator account to retrieve the GData free/busy feeds.
  * 4. The Google Calendar Connector Sync Service writes free/busy messages to the free/busy store using an Active Directory account with write permission to the free/busy store for other users. This requires an Exchange account with Admin privileges.

# Security Considerations #

The following are important security considerations to review when deploying the Google Calendar Connector Kit.

## Limiting access to the Web Service ##

The Google Calendar Connector Web Service does NOT provide an authentication layer for choosing which requests for free/busy information and appointment details to allow or dis-allow.  Anyone who knows the URL address of your Web Service and has network access to its URL would theoretically be able to perform Microsoft Exchange free/busy look ups and, if enabled, retrieve appointment details to the same degree as any user of your Google Apps domain.

**Therefore, to restrict access to the Web Service to members of your Google Apps domain, we recommend:**

  * Using a private Web Service URL in your company's internal network that is not accessible outside of your company's VPN / Firewall.

The Google Calendar Connector Web Service contains a diagnostics.aspx which can query Active Directory for user accounts and Exchange user free/busy data. By default starting with version 1.2.0.289 access to this page is restricted to the localhost of the server running the Google Calendar Connector Web Service. If you disable this localhost access you could leak sensitive user information. We recommend you always ensure this restriction is enforced.

## Transport security ##

  * **Google Calendar Connector Web Service:** The Web Service supports passing free/busy information using HTTP or HTTPS. To enable HTTPS mode, perform the following steps:
  1. Use Google Calendar in HTTPS mode or force HTTPS mode for your entire Google Apps domain.
  1. Provide an **`https://`** Web Service URL to Google support. Please refer to the [FAQ](http://code.google.com/p/google-calendar-connectors/wiki/FAQ) for how to contact Google Apps support.

> If Google Calendar is configured with an HTTPS url that fails, it will not fallback to HTTP. Free/busy information is not publicly broadcast, and when Google Calendar and the Web Service are using HTTPS, POST communications use SSL encryption in transit.

  * **Google Calendar Connector Sync Service:** Free/busy information is downloaded from Google Calendar using a Google Apps administrator account and an authenticated GData feed over HTTPS.

## Account Privileges ##

The Google Calendar Connectors generally require a Google Apps account to access Google Calendar information, a Windows system account to access the local file system, and an Active Directory account to interact with Active Directory and Microsoft Exchange.  The following is a summary of the various accounts you will configure during the setup of the various Google Calendar Connectors.

### Google Calendar Connector Web Service: ###

  1. Google Apps user account.
  1. Active Directory user with standard DOMAIN USER privileges.
  1. Active Directory user with Read-Access to the free/busy store (retrieving free/busy).
  1. The Windows System account NETWORK SERVICE is used by ASP.NET and needs MODIFY privileges to:
    * C:\Inetpub\wwwroot\{Virtual Web Application Directory}
    * C:\Inetpub\wwwroot\{Virtual Web Application Directory}\Web.config
    * GoogleApps.GCal.LogDirectory

### Google Calendar Connector Sync Service: ###

  1. Google Apps user account.
  1. Active Directory user with standard DOMAIN USER privileges.
  1. Active Directory user with Exchange extended "owner" privileges for writing free/busy to the Public Folder store.
  1. The Windows System account SYSTEM requires:
    * MODIFY privileges for SyncServiceXmlStorageDirectory
    * MODIFY privileges for the install directory

## Free/busy and appointment details privacy ##

Free/busy information is generally available to all users of the domain, without restriction. An exception is appointment detail information, which is only shared if it is not private. Explicit sharing of appointment detail information is not required.

  * **Google Calendar Connector Web Service:**
> Free/busy information is accessible by any domain user, for any domain user who has free/busy information in the Exchange free/busy public store. Appointment details are available for any domain user, if the appointment detail look up is enabled and if the appointment is not marked as private. Private event details are not returned to Google Calendar.  See the Security Consideration entitled "Limiting access to the Web Service" for more information.

  * **Google Calendar Connector Sync Service:**
> Free/busy information is accessible by any Google Apps domain user, for all domain users on Google Apps. Appointment details for Google Apps users are not available to Microsoft Exchange users because the free/busy store only allows storage of free/busy data.

## Google Calendar's access to your corporate data ##

The Google Calendar Connectors will only return free/busy information and non-private appointment details from Microsoft Exchange. Only free/busy information is shared from Google Apps. No Microsoft Exchange free/busy information or appointment details are explicitly stored on Google's servers.

### Google Calendar Connector Web Service: ###

The Google Calendar Scheduler is returned the free/busy and appointment information from your Exchange environment, but this information is not stored on Google's servers, and it only transits Google's servers to be parsed and rendered via javascript within the user's browser.

**_The Web Service responds to any valid requests it receives, and so access to the Web Service URL should be limited appropriately.  See the Security Consideration "Limiting Access to the Web Service" for more details._**

**_Concurrent, malicious javascript execution in the browser could expose your free/busy and appointment information to an outside party.  This is mentioned only as a precaution._**

### Google Calendar Connector Sync Service: ###

No additional corporate data is exposed to Google Calendar when operating the Sync Service. The Sync Service only reads free/busy information from an authenticated Google Calendar GData feed.

## Configuration file protection and encryption ##

  * The Google Calendar Connectors use configuration files that are installed with the connector software. To protect the sensitive account information in the files, Google recommends you restrict access to these files. The configuration files are stored in plain text which means they can be viewed with a standard text editor. The Google Calendar Connector Web Service and the Google Calendar Connector Sync Service can optionally have their appSettings node encrypted, rendering their sensitive information permanently unreadable.

# Additional Resources #

[Google Calendar Connector Web Service Install and Configure Guide](http://code.google.com/p/google-calendar-connectors/wiki/WebServiceGuide)

[Google Calendar Connector Sync Service Install and Configure Guide](http://code.google.com/p/google-calendar-connectors/wiki/SyncServiceGuide)

[Google Calendar Connector Technical FAQ Guide](http://code.google.com/p/google-calendar-connectors/wiki/FAQ)

# Copyright Notices #

Library and license attributions are provided to conform with the Apache License, Version 2.0. A copy of the Apache License, Version 2.0 can be found [here](http://www.apache.org/licenses/LICENSE-2.0.html).

The following licenses and libraries are used in the Google Calendar Connector Web Service:

  * Google Data (GData) API .NET Client Library and its dependencies licensed under the **Apache License, Version 2.0.** ([project](http://code.google.com/p/google-gdata/), [license](http://www.apache.org/licenses/LICENSE-2.0))
  * Apache log4net licensed under the **Apache License, Version 2.0.** ([project](http://logging.apache.org/log4net/index.html), [license](http://logging.apache.org/log4net/license.html))
  * tz4net v3.0.2.0 licensed under the **GNU LGPL V2**. ([project](http://www.babiej.demon.nl/Tz4Net/main.htm), [license](http://www.gnu.org/copyleft/lgpl.html))

The following licenses and libraries are used in the Google Calendar Connector Sync Service:

  * Google Data (GData) API .NET Client Library and its dependencies licensed under the **Apache License, Version 2.0.** ([project](http://code.google.com/p/google-gdata/), [license](http://www.apache.org/licenses/LICENSE-2.0))
  * Apache log4net licensed under the **Apache License, Version 2.0.** ([project](http://logging.apache.org/log4net/index.html), [license](http://logging.apache.org/log4net/license.html))
  * tz4net v3.0.2.0 licensed under the **GNU LGPL V2**. ([project](http://www.babiej.demon.nl/Tz4Net/main.htm), [license](http://www.gnu.org/copyleft/lgpl.html))


---


_Google, Google Calendar, Google Calendar Connector, Google Calendar Connector Web Service, Google Calendar Connector Sync Service, Google Calendar Connector Kit are trademarks of Google, Inc._

_All other company and product names may be trademarks of the respective companies with which they are associated._