# Google Calendar Connector Web Service Installation and Configuration Guide 1.2.0.289 #

In the Google Calendar Connector framework, data flow from Exchange is provided by the Google Calendar Connector Web Service. Google Calendar connects to the Web Service to query Exchange servers for free/busy data and appointment data, which includes the name, place, and attendees of events.  This document provides instructions for installing and configuring the Google Calendar Connector Web Service.
Contents

  1. [About This Document](#1_About_This_Document.md)
    1. [Audience](#Audience.md)
    1. [For More Information](#For_More_Information.md)
  1. [Before You Install](#2_Before_You_Install.md)
    1. [Prerequisites](#Prerequisites.md)
    1. [Google Calendar Account Setup](#Google_Calendar_Account_Setup.md)
    1. [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md)
    1. [Installation Checklist](#Installation_Checklist.md)
  1. [Installing the Google Calendar Connector Web Service](#3_Installing_the_Google_Calendar_Connector_Web_Service.md)
    1. [MSI Installation](#MSI_Installation.md)
    1. [Multiple Web Service Instances](#Multiple_Web_Service_Instances.md)
    1. [File Permissions](#File_Permissions.md)
  1. [Configuring the Google Calendar Connector Web Service](#4_Configuring_the_Google_Calendar_Connector_Web_Service.md)
    1. [Editing the Configuration File](#Editing_the_Configuration_File.md)
    1. [Encrypting the Configuration File](#Encrypting_the_Configuration_File.md)
    1. [HTTP Proxy Configuration Optional](#HTTP_Proxy_Configuration_Optional.md)
  1. [Installation Success](#5_Installation_Success.md)
    1. [Verifying Communication](#Verifying_Communication.md)
  1. [Testing and Troubleshooting](#6_Testing_and_Troubleshooting.md)
    1. [Running Diagnostics Tests](#Running_Diagnostics_Tests.md)
    1. [Logging Configuration](#Logging_Configuration.md)
    1. [HTTP Checking](#HTTP_Checking.md)
  1. [Known Issues](#7_Known_Issues.md)
  1. [Copyright Notices](#8_Copyright_Notices.md)

## 1 About This Document ##

This section describes the audience for this document, the organization of the material, and some additional sources of information.


The Google Calendar Connector Kit is a set of server-installed tools that require knowledgeable  installation and configuration, tailored to the particulars of a customer's environment.  These tools are only the **technological part** of the integration experience.

Before proceeding to install the Google Calendar Connector Web Service, please read and understand the important **Security Considerations** outlined in the [Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview).

### Audience ###

This documentation is for systems administrators tasked with setting up and managing the Google Calendar Connector Web Service. Users of this documentation need to be familiar with the administration of Google Calendar, Microsoft Exchange Server, Active Directory, and Internet Information Services (IIS).

It is important to note that this tool only represents part of a solution for free/busy interoperability.

### For More Information ###

This document is part of the Google Calendar Connector document set, which includes the following related documents:

  * **[Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview)**. Describes the components, process flow, and security considerations for the Google Calendar Connector Kit.

  * **[Sync Service Installation and Configuration Guide](http://code.google.com/p/google-calendar-connectors/wiki/SyncServiceGuide)**. Provides instructions for installing and configuring the Sync Service to provide free/busy data flow from Google Calendar to Exchange.

For background or general information on Google Apps, see the [Google Apps for Administrators Help Center](http://www.google.com/support/a/).

For background or general information on Windows administration, see [Microsoft TechNet](http://technet.microsoft.com/).


### 2 Before You Install ###

Before you install and configure the Google Calendar Connector Web Service, make sure your Windows environment meets the following prerequisites, and that Google Calendar and Exchange are prepared for sharing data.

### Prerequisites ###

  * Google Apps Premier or Education Edition domain

To support the Web Service, your Windows environment must meet these prerequisites:
  * Microsoft Windows Server 2003
  * Internet Information Services (IIS) 6.0
  * Microsoft Exchange Server 2003 SP2 or Microsoft Exchange Server 2007 SP1 or SP2
  * ASP.NET 2.

### Google Calendar Account Setup ###

To prepare Google Calendar for sharing free/busy data, you must perform these tasks:

#### Specify the Google Calendar Connector Web Service URL. ####

  1. Login to your Google Apps Domain control panel. [https://www.google.com/a/yourdomain.com](https://www.google.com/a/*yourdomain.com*)
  1. Select Advanced Tools
  1. Scroll down to Free/Busy service
  1. Click on Set up Calendar to use your free/busy web service
  1. Select I am using a free/busy service at this URI:
  1. Enter the URI for the Calendar Connector Web Service
> For example: https://mywebserver.acme.com/GCalExchangeLookup/Exchangequerier.aspx (Please review the project [http://code.google.com/p/google-calendar-connectors/wiki/FAQ](FAQ.md) for more details and common questions)

#### Create dedicated Google Apps service account and perform an initial login. ####

> The user name and password are required when you populate the GoogleApps.AdminUser.Login and Password parameters in the configuration process. NOTE: Before this account can be used with the connector you must login to Google Apps account and accept the standard terms of service agreement for the user account.

### Dedicated Windows Service Accounts ###

The Web Service requires dedicated Active Directory user accounts for performing LDAP queries and diagnostic tests with Exchange. Create these users as described in this section, and note the user names and passwords to provide when configuring the web service.


> #### **Active Directory LDAP Query User** ####

> Create a dedicated user for performing Active Directory LDAP queries. The user name and password are required later when you populate the _ActiveDirectory.DomainUser.Login_ and _Password_ parameters in the configuration process. This account is also used by the Google Calendar Connector Sync Service if it is installed in the same environment.

> #### **Exchange Query User** ####
> Create a dedicated user for the connector web service to use for retrieving free/busy data. The user name and password are required later when you populate the _Exchange.GCalQueryUser.Login_ and _Password_ parameters in the configuration process.

> _NOTE: If you want to include appointment detail along with free/busy data, the Exchange Query User will need to be granted "Receive As" permission for all mailboxes. Refer to the Exchange Query Admin User for details on how to delegate this permission._

> #### **Exchange Query Admin User** ####
> Create a dedicated user for the connector web service to use for running diagnostic tests. The user name and password are required later when you populate _Exchange.GCalQueryAdmin.Login_ and _Password_ parameters in the configuration process. This account is also used by the Google Calendar Connector Sync Service if it is installed in the same environment.

> The **Exchange Query Admin** user needs the extended "**Receive As**" permissions to be able to read and write both free/busy or appointments in Exchange.

> To configure the permissions for the query admin user:
    1. Start Registry Editor (regedit).
    1. Navigate to the following key: HKEY\_CURRENT\_USER\Software\Microsoft\Exchange\ExAdmin
    1. On the **Edit** menu, click **Add Value**, and then add the following registry value:
    1. Value Name : **ShowSecurityPage**
    1. Data Type : **REG\_DWORD**
    1. Value : **1**
    1. Close Registry Editor.
    1. Start the **Exchange System Manager**.
    1. Right Click on your **Exchange Organization Name** and select **Properties**
    1. Select the **Security Tab**
    1. Click the **Advanced** button
    1. Click **Add**
    1. In the **Select Users, Computers, or Groups** dialog enter the user name of the query admin user and click OK
    1. Check the **Allow** box for the permission "**Receive As**" and click **OK**


## Remounting the Exchange Information Stores ##

The Exchange Information store caches permissions data. To flush the cache immediately after setting new permissions, dismount and remount the appropriate public folder store or restart the Information Store service. Otherwise the changes will not take effect until Exchange rebuilds the permissions cache.


To dismount and remount the public folder store:

  1. Open Exchange System Manager
  1. In tree view, expand **Administrative Groups**, _{Your Administrative Group}_, **Servers**,_{Your local Exchange server}_, and _{Your Storage Group}_.
  1. Right click on the **Public Folder Store**, select **Dismount Store** and click **Yes** to continue.
  1. Right click on the **Public Folder Store**, select **Mount Store** and click **Yes** at the success message dialog.

To restart the service Microsoft Exchange Information Store:
  1. Open the services.msc
  1. In right pane view, locate **Microsoft Exchange Information Store**, right click and select **Restart**

### Installation Checklist ###

| **Task** | **Completed?** | **Notes** |
|:---------|:---------------|:----------|
| Verify that Windows environment prerequisites are met |  |  |
|Provide Web Service URL to Google Apps Support |  | URL: |
|Create dedicated Google Apps Web Service user|  |User name and password: |
|Create dedicated Active Directory user|  |User name and password: |
|Create dedicated Exchange Query user|  |User name and password: |
|Create dedicated Exchange Admin user|  |User name and password: |
|Set Exchange permissions for Query and Admin users |  |  |


## 3 Installing the Google Calendar Connector Web Service ##

### MSI Installation ###

The installer for the Web Service is provided as the file _GoogleCalendarConnectorWebService.msi_.

To install the Web Service:
  1. Locate and open the file _GoogleCalendarConnectorWebService.msi_.
  1. In the welcome dialog, click **Next**.
  1. Specify the desired virtual directory location, or accept the default value _GCalExchangeLookup_ on the Default Web Site, and click **Next**.
  1. Click **Next** to begin the installation, and then **Close** when the dialog displays the message "Installation Complete".
  1. Open Internet Information Services (IIS) Manager and verify that the ASP.NET 2.0 Web Service Extension is "Allowed".
    * In the IIS Manager tree view, expand the local server.
    * Select **Web Service Extensions**.
    * From the list of web service extensions, select ASP.NET v2.0 and click **Allowed**.
  1. Using the IIS Management Console, update the **Web Site** to use v2.0 of the ASP.NET runtime.
  1. In the IIS Manager tree view, select the Web Site where the GCALExchangeLookup virtual web directory was created. .
    * Right click on the Web Site and select **Properties**.
    * Select the **ASP.NET** tab and set the ASP.NET version to 2.0.
    * Click **OK** to close the **Properties** dialog.


### Multiple Web Service Instances ###

If you need to setup more than one domain with the Google Calendar Web Service you can use the following instructions for setting up multiple instances of the Google Calendar Web Service on a single server.

  1. Install the Google Calendar Web Service
  1. Copy the default install of the Web Service to a new folder, for example: C:\Inetpub\wwwroot\GCalExchangeLookup-domain.com
  1. Create new IIS Virtual Web Directory for the copy for domain.com on the default web site
```
cscript %WINDIR%\SYSTEM32\iisvdir.vbs /create w3svc/1/ROOT "GcalExchangeLookup-domain.com" "C:\Inetpub\wwwroot\GCalExchangeLookup-domain.com"
```
  1. Configure new logging paths: Edit the web.config and update the following log path values
```
 <file value="C:\Google\logs\domain.com\WebService.log" />
 initializeData="c:\google\logs\domain.com\WebNetTrace.log"
```
  1. Update the specific Google Apps Domain Values
```
 <add key="GoogleApps.DomainName" value="domain.com" />
 <add key="GoogleApps.AdminUser.Login" value="user" />
 <add key="GoogleApps.AdminUser.Password" value="password" />
```

### File Permissions ###

Special file system permissions must be set to allow the encryption of the web.config and the ability for the Web service to write files to the file system.

#### **NETWORK SERVICE "Modify" Access for the Web Service virtual directory and logging directories.** ####

The local NETWORK SERVICE account requires modify access to the Web Service's virtual directory folder to perform encryption of the configuration file as well as modify access to the logging and data folders configured in the web.config file.


In Web.config\appSettings\:
  * **NETWORK SERVICE** must have "**MODIFY**" privileges to the _GoogleApps.GCal.LogDirectory_ directory.
  * **NETWORK SERVICE** must have "**MODIFY**" privileges to _Web.config_ file.

In _Web.config\configuration\log4net\appender\_:
  * **NETWORK SERVICE** must have "**MODIFY**" privileges to _configuration\log4net\appender\file_.

To grant "Modify" access to a directory do the following:
  1. Open Windows Explorer.
  1. Navigate to the directory root folder (_i.e. C:\Inetpub\wwwroot\GCalExchangeLookup_).
  1. Right click on the folder and select **Properties**.
  1. Select the **Security** tab and click **Add**, type **NETWORK SERVICE** and click **OK**.
  1. With the **NETWORK SERVICE** user highlighted, check **Allow** for **Modify** privileges.


## 4 Configuring the Google Calendar Connector Web Service ##

Configuration settings for the Web Service are stored in _Web.config_. You must edit this file to enter appropriate values for your setup. Additional configuration options for advanced users are available with in-line comments in _Web.config_. It is recommended you enable _Configuration.EncryptOnNextRun_ when you have finished editing and tested the _Web.config_.

### Editing the Configuration File ###

The "appSettings" section of _web.config_, located in the Web Service virtual directory, contains the following important configuration keys:
| **Configuration Key** | **Description** | **Default Value** |
|:----------------------|:----------------|:------------------|
| ActiveDirectory.DomainController | Specify the fully qualified domain name of a domain controller for your Windows Active Directory domain. This server is used for LDAP queries to retrieve user login and Exchange user attributes. For queries directly against a domain controller, prefix the machine name with LDAP://. For queries against the Global Catalog, prefix the machine name with GC://. For example: `<add key="ActiveDirectory.DomainController" value="LDAP://HQAD1.corp.acme.com"/> ` |  |
| ActiveDirectory.DomainUser.Login | The userPrincipalName for the user account used by the web service to perform Active Directory lookups. [See Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on this user account. For example: `<add key="ActiveDirectory.DomainUser.Login" value="ADDomainUser@corp.acme.com"/>` |  |
| ActiveDirectory.DomainUser.Password | The password for the user account used by the web service to perform Active Directory lookups. |  |
| Exchange.ServerName | The full qualified domain name of the target Exchange for the web service to communicate with. If this server does not host a copy of the Free/Busy Public Information store, you must specify the setting: Exchange.FreeBusyServerName. For example: `<add key="Exchange.ServerName" value="HQEXCH1.corp.acme.com"/>`|  |
| Exchange.GCalQueryUser.Login | The userPrincipalName of the user account used to query Exchange for free/busy data. [See Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on this user account. For example: `<add key="Exchange.GCalQueryUser.Login" value="GCalQueryUser@corp.acme.com"/>` |  |
| Exchange.GCalQueryUser.Password | The password for the Active Directory user account used to query Exchange for free/busy data. Exchange. |  |
| Exchange.GCalQueryAdmin.Login |The userPrincipalName of the user account used in running diagnostic tests for the web service. This must be an Active Directory user with rights to create appointments on other users' calendars. See [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on this user account. For example: `<add key="Exchange.GCalQueryAdmin.Login" value="GCalQueryAdmin@corp.acme.com"/>` This account is also used to write appointments to calendars in Exchange when using the appointment writer. |  |
| Exchange.GCalQueryAdmin.Password | The password for the user account used in running diagnostic tests for the web service. |  |
| Exchange.DefaultDomain | This setting prevents Exchange WebDav redirects to servers outside of the DNS domain defined. Enable this feature if the connector should not redirect Exchange server outside of this DNS domain. For example: `<add key="Exchange.DefaultDomain" value=".corp.acme.com"/>` | DISABLED |
| Exchange.FreeBusyServerName | This setting allows the definition of a specific Exchange server to use when reading and writing free/busy data. By default the value Exchange.ServerName is used. Use this value if the default Exchange server does not host a replica of the SchedulePlus  Free/Busy Information Store. To enable this setting remove comments and define the fully qualified domain name of the server to use. For Example: `<add key="Exchange.FreeBusyServerName" value="http://HQEXCH2.corp.acme.com"/>` | DISABLED |
| Exchange.EnableAppointmentLookup | This setting controls if the free/busy lookup should also retrieve and return appointment details. To enable appointment details to be returned set the value to True. For Example: `<add key="Exchange.EnableAppointmentLookup" value="True"/>` _NOTE: See [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on how to grant access to read appointment data for the service accounts_. | false |
| Exchange.LDAPQuery | This setting specifies the LDAP query that returns unique ActiveDirectory user object based on the identity passed in. In most cases, the passed in identity is a user's primary SMTP email address and therefore, the LDAP query string in most cases is "(mail={0})" (the _{0}_ is replaced with the identity while querying LDAP). However, in the cases when users are identified by other primary fields, the value of this key should be set to appropriate LDAP query string. For example, to uniquely identify users by userPrincipalName, set the value to (userPrincipalName={0}) | (mail={0}) |
| Exchange.MaxConnections | This setting defines the maximum number of  concurrent connections that can be made to configured Exchange server. To enable this setting remove comments and and define the max number of connections. For Example: `<add key="Exchange.MaxConnections" value="50"/>` |  |
| Exchange.ExchangeAPI | This setting determines which API is used by Web Service to query for free/busy data. There are two supported options "WebDAV" and "EWS". For Exchange 2003 environments, only "WebDAV" is supported. For Exchange 2007 environments "EWS" is the preferred method as this API provides real-time free/busy data from Google Apps along with appointment details. The "WebDAV" options is still fully supported for Exchange 2007 however, this API method only provides free/busy data and does not support appointment details. The free/busy data for the "WebDAV" API option is updated only periodically by the Sync Service. | WebDAV |
| Exchange.EWSURLPath | The URL path to Exchange Web Services on an Exchange Client Access Server. For example: `<add key="Exchange.EWSURLPath" value="/EWS/Exchange.asmx"/>` | /EWS/Exchange.asmx |
| GoogleApps.DomainName | The domain name used by Google Apps. For example: `<add key="GoogleApps.DomainName" value="acme.com" />` |  |
| GoogleApps.AdminUser.Login | The Google Apps user account name used for querying Google Calendar. This account must be a dedicated account in Google Apps setup for use with the Google Calendar Connector. For example: `<add key="GoogleApps.AdminUser.Login" value="gcc_websvc" />` |  |
| GoogleApps.AdminUser.Password |The password for the Google Apps user account used for querying Google Calendar. |  |
| GoogleApps.GCal.LogDirectory | This setting defines the directory to write debug output for the GDATA API feed for each user account queried from Google Apps. An individual file is written out per user request. The output contains the calendar feed data for the specific user. If no value is defined no output is written. For Example: `<add key="GoogleApps.GCal.LogDirectory" value="C:\Google\logs"/>` | DISABLED |
| GoogleApps.GCal.DomainMapping | This setting allows the definition of SMTP domain name mappings. Use this setting if your Exchange Primary SMTP address is different from the Google Apps SMTP domain name. When enabled the connector maps the external Google Apps SMTP domain name to the internal Exchange SMTP domain name. This setting by default is commented out in the config file. To enable this setting remove comments and define the External and Internal SMTP domain names. For example: `<add key="GoogleApps.GCal.DomainMapping" value="acme.com,exchange.acme.com"/>` | DISABLED |
| WebService.RequireLocalAccessforDiagnostics | This settings allows controlled access to the diagnostics.aspx page. By default the value is set to true which means you can only access the diagnostics.aspx from the local machine where its installed. _**NOTE: There is not authentication required to access this page and it can not be deleted. This page can query Active Directory for user accounts and Exchange user free/busy data. Opening access outside of localhost could leak sensitive user information. We recommend you always ensure this restriction is enforced.**_ For example: `<add key="WebService.RequireLocalAccessforDiagnostics" value="true"/>`  | true |
| WebService.DefaultGoogleCalendarSSL | This setting allows you to override the default Google Calendar URL detection used by the web service. Some browsers might not properly set the referrer header. The referrer header is used to determine if you are using Google Calendar via HTTP or HTTPS. By default if no referrer header is found we use HTTPS as the default URL for Google Calendar. If your Google Apps Domain is configured to always use HTTP and the browser is not setting the referrer header data might not be posted back to Google Calendar properly. To work around this problem you can force the Web Service to always post to a HTTP Google Calendar URL. Add the following property to the web.config file: `<add key="WebService.DefaultGoogleCalendarSSL" value="false"/>`. **_NOTE: We recommend you only set this value if you are experiencing this specific problem, and only if your Google Apps Calendar settings always force HTTP._| NOT CONFIGURED**|

### Encrypting the Configuration File ###

Because some configuration keys contain user names and passwords in plain text, it is recommended that you encrypt these configuration parameters. To enable encryption, Load the Diagnostics.aspx page. Click the "Encrypt Config Settings" button. This will rewrite the web.config with the configuration values encrypted. _NOTE: "Network Service" must have modify privileges to the directory to rewrite the encrypted file._


This encryption method will encrypt the entire appSettings node of the configuration file, rendering it unreadable. Once encrypted, the settings are no longer in plain text on the file system. However, settings can still be changed through the IIS Manager.

### HTTP Proxy Configuration Optional ###

The Web Service can optionally be configured to work with an HTTP proxy. This configuration may be required if all out-bound communication needs to be routed through a proxy. Configuring the HTTP proxy parameters in Web.config will instruct the Web Service to forward its communication through such a proxy.

Microsoft Support includes an overview of the same process and configuration in the following Knowledge Base article: http://support.microsoft.com/kb/307220.

The table below outlines the configuration parameters located under the <system.net.defaultProxy> node of Web.config:

| **Configuration Key** | **Description** | **Default Value** |
|:----------------------|:----------------|:------------------|
|bypasslist.address | Allows the Web Service to bypass the proxy for additional, non-local addresses. As per Microsoft's instructions, this field can contain a host name, or a regular expression. |  |
| proxy.usesystemdefault | Instructs the .NET client to either use the default system profile for access, or to use a custom proxy defined in **proxy.proxyaddress**. To override the system default and configure a custom web proxy for the Web Service, this variable should be **FALSE** | True |
| proxy.proxyaddress | The URL:PORT pair for your proxy server. For example: `http://proxyserver.corp.acme.com:3128` |  |
| proxy.bypassonlocal | Allows the Web Service to bypass the proxy for local addresses containing a ".". | True|

**_Important Note: Please carefully consider whether your Microsoft Exchange server communication should be sent through a custom proxy. Including Microsoft Exchange servers in a custom proxy scheme could result in decreased performance and certain types of Windows authentication may fail._**

## 5 Installation Success ##

### Verifying Communication ###

After completing the installation and configuration tasks, perform the following to verify that the Google Calendar can query free/busy data from Exchange.

To look up an Exchange user's free/busy information from Google Calendar:
  1. Choose an Exchange user with known free/busy times.
  1. Log on to Google Calendar.
  1. Click **Create Event** to open an event page.
  1. Click **Check guest and resource availability** to open the resource scheduler.
  1. Enter the e-mail address of the chosen Exchange user and click "**Add**".
  1. **Verify** that the free/busy information displayed in the resource scheduler matches the known information.

If the Web Service does not work as expected, diagnostics should be run to identify any issues.
If no data is return to Google Calendar, enable DEBUG logging to verify the web service is receiving the Google Calendar free/busy request. Refer to the "Logging" section for details. If unexpected issues persist, please refer to the sections entitled "Logging" and "HTTP Checking".

## 6 Testing and Troubleshooting ##

This section describes simple verification tests for the Web Service and provides some resources for troubleshooting common test failures. If the Web Service does not return free/busy data, use the following diagnostics tests, logging information, and troubleshooting checklist to identify and correct any issues.


**Important Note:** Please review [Logging Configuration](#Logging_Configuration.md) before beginning to test any of the functionality of the service.

### Running Diagnostics Tests ###

The Web Service contains a diagnostics page that can help diagnose issues. Diagnostics.aspx, located in the root of the Web Service virtual directory, contains the following diagnostic tests for the web service:
  * **Verify users can be found in Active Directory**

> This test queries the Active Directory domain controller specified in ActiveDirectory.DomainController and returns all objects found in the result set. Specific LDAP queries can be tested using the text-area (cn=`*` is the default). This test verifies that the Web Service can properly connect to an Active Directory server and retrieve user/contact information.

  * **Verify free / busy information can be found in Exchange (+/- 7 days)**

> This test verifies that the Web Service is correctly configured to talk to the Exchange server, and retrieve  free/busy information for a user or contact. Please be aware that this test only returns free/busy information within 7 days of the current date.

  * Diagnostics.aspx also contains other tests which are intended for test and troubleshooting the Sync Service.


### Logging Configuration ###

The Google Calendar Connector includes a configuration node named 

&lt;log4net&gt;

 in Web.config which controls the logging behavior of the Web Service.

To use logging to troubleshoot Web Service issues:

  * Verify Log Path: 

&lt;file value="C:\Google\logs\WebService.log" /&gt;


  * Increase the logging level: 

&lt;level value="DEBUG" /&gt;


  * Verify log file system permissions if the file is not created.

#### Verify Log Path ####

To change where the log file is stored, edit the node path configuration/log4net/appender/file and change the value attribute of the file node to the new location. Include the file name in the value attribute. NOTE: the NETWORK SERVICE account needs "Modify" access to the file system path.
Increase the logging level



There are four logging levels, with DEBUG providing the most output and ERROR only logging severe events:

  * DEBUG
  * INFO
  * WARN
  * ERROR

The Web Service has many potential logging points for the DEBUG and ERROR levels. When logging is set to these levels, the size of the log file may grow very rapidly.

To set the Web Service logging to a certain level, edit the node path configuration/log4net/root/level and change the value attribute to one of the four levels listed. Lower severity levels are inclusive of higher levels. For example if the Web Service is set to INFO, it also logs WARN and ERROR but not DEBUG messages.

#### Verify log file permissions ####

If log files are not being written verify that the following file permissions have been set up properly:

In Web.config\appSettings\:

  * **NETWORK SERVICE** must have "**MODIFY**" privileges to the GoogleApps.GCal.LogDirectory directory.
  * **NETWORK SERVICE** must have "**MODIFY**" privileges to Web.config.

In Web.config\configuration\log4net\appender\:
  * **NETWORK SERVICE** must have "**MODIFY**" privileges to configuration\log4net\appender\file.

To grant "**Modify**" access to a directory do the following:
  1. Open Windows Explorer.
  1. Navigate to the directory root folder _(i.e. C:\Inetpub\wwwroot\GCalExchangeLookup)_.
  1. Right click on the folder and select **Properties**.
  1. Select the **Security** tab and click **Add**, type **NETWORK SERVICE** and click **OK**.
  1. With the **NETWORK SERVICE** user highlighted, check **Allow** for **Modify** privileges.

### HTTP Checking ###

In addition to diagnostic tests, HTTP analysis tools can be used to view the actual data that is being passed between Google Calendar and the Web Service. _HTTP Analyzer_ (for Microsoft Internet Explorer) or _Live HTTP Headers_ (for Firefox) are examples of HTTP analysis tools.

To troubleshoot the Web Service with an HTTP utility:
  1. Check whether a lookup request is sent from Google Calendar to the Web Service. The link for the request should match the Web Service URL provided to Google Apps.
  1. If Google Calendar is not making any requests, please contact Google Apps support to verify that the Google Calendar Connector interoperability is enabled for your domain and properly configured.
  1. Check the IIS log files for requests for the Web Service URL which was provided to Google Apps (ending in ExchangeQuerier.aspx) and verify that the result code is 200.
  1. Check the IIS log files on your Exchange server to verify the WebDAV requests the Web Service is making into Exchange.
  1. If a Google Calendar request is being made and the Web Service is responding, verify that the Web Service response is an HTML snippet with a POST action to the Google Calendar servers. If it is not, please send a copy of the response to Google Apps Support.
  1. If Google Calendar fails to add free/busy information from a valid Web Service response, there may be browser issues. Please contact Google Apps Support with your browser version and error information.

## 7 Known Issues ##

  * Some versions of Firefox 3 Beta do not pass the referrer header to the Google Calendar Connector Web Service. When using SSL, this can result in the Google Calendar free-busy lookup requests causing a redirect to the login page. The referrer header is used to determine if the mailslot post url should be HTTP or HTTPS. To force SSL if the header is not being passed you can set the config setting: WebService.DefaultGoogleCalendarSSL=True

## 8 Copyright Notices ##

Library and license attributions are provided to conform with the Apache License, Version 2.0. A copy of the Apache License, Version 2.0 can be found [here](http://www.apache.org/licenses/LICENSE-2.0.html). The following licenses and libraries are used in the Google Calendar Connector Web Service:

  * Google Data (GData) API .NET Client Library and its dependencies licensed under the **Apache License, Version 2.0**. ([project](http://code.google.com/p/google-gdata/), [license](http://www.apache.org/licenses/LICENSE-2.0))
  * Apache log4net licensed under the **Apache License, Version 2.0**. ([project](http://logging.apache.org/log4net/index.html), [license](http://logging.apache.org/log4net/license.html))
  * tz4net v3.0.2.0 licensed under the **GNU LGPL V2**. ([project](http://www.babiej.demon.nl/Tz4Net/main.htm), [license](http://www.gnu.org/copyleft/lgpl.html))

Portions Copyright (c) 2002 James W. Newkirk, Michael C. Two, Alexei
A. Vorontsov or Copyright (c) 2000-2002 Philip A. Craig


---


_Google, Google Calendar, Google Calendar Connector, Google Calendar Connector Web Service, Google Calendar Connector Sync Service_ are trademarks of Google, Inc.

All other company and product names may be trademarks of the respective companies with which they are associated.