# Google Calendar Connector Sync Service Installation and Configuration Guide 1.3.0.0 #

The Google Calendar Connector Sync Service provides data flow from Google Calendar. The Sync Service periodically polls Google Calendar for free/busy data and appointment data, which includes the name, place, and attendees of events.

This document provides detailed instructions for installing and configuring the Google Calendar Connector Sync Service.
Contents

  1. [About This Document](#1_About_This_Document.md)
    1. [Audience](#Audience.md)
    1. [For More Information](#For_More_Information.md)
  1. [Before You Install](#2_Before_You_Install.md)
    1. [Prerequisites](#Prerequisites.md)
    1. [Google Calendar Account Setup](#Google_Calendar_Account_Setup.md)
    1. [Exchange User Mapping](#Exchange_User_Mapping.md)
    1. [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md)
    1. [Installation Checklist](#Installation_Checklist.md)
  1. [Installing the Google Calendar Connector Sync Service](#3_Installing_the_Google_Calendar_Connector_Sync_Service.md)
    1. [MSI Installation](#MSI_Installation.md)
    1. [Multiple Sync Service Instances](#Multiple_Sync_Service_Instances.md)
  1. [Configuring the Google Calendar Connector Sync Service](#4_Configuring_the_Google_Calendar_Connector_Sync_Service.md)
    1. [Editing the Configuration File](#Editing_the_Configuration_File.md)
    1. [Encrypting the Configuration File](#Encrypting_the_Configuration_File.md)
    1. [HTTP Proxy Configuration Optional](#HTTP_Proxy_Configuration_Optional.md)
  1. [Installation Success](#5_Installation_Success.md)
    1. [Start the Google Calendar Connector Sync Service](#Start_the_Google_Calendar_Connector_Sync_Service.md)
  1. [Testing and Troubleshooting](#6_Testing_and_Troubleshooting.md)
    1. [Running Diagnostics Tests](#Running_Diagnostics_Tests.md)
    1. [Logging Configuration](#Logging_Configuration.md)
  1. [Known Issues](#7_Known_Issues.md)
  1. [Copyright Notices](#8_Copyright_Notices.md)

## 1 About This Document ##

This section describes the audience for this document, the organization of the material, and some additional sources of information.

The Google Calendar Connector Kit is a set of server-installed tools that require knowledgeable  installation and configuration, tailored to the particulars of a customer's environment.  These tools are only the **technological part** of the integration experience.

Before proceeding to install the Google Calendar Connector Sync Service, please read and understand the important **Security Considerations** outlined in the [Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview).

### Audience ###

This document is for systems administrators tasked with setting up and managing the Google Calendar Connector Sync Service. Users of this documentation need to be familiar with Google Calendar administration, Microsoft Exchange Server and Windows network administration.

It is important to note that this tool only represents part of a solution for free/busy interoperability.

### For More Information ###

This document is part of the Google Calendar Connector document set, which includes the following related documents:

  * [Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview). Describes the components, process flow, and security considerations for the connector.

  * [Web Service Installation and Configuration Guide](http://code.google.com/p/google-calendar-connectors/wiki/WebServiceGuide). Provides instructions for installing and configuring the Web Service, which provides Exchange free/busy data flow to Google Calendar.

For background or general information on Google Apps, see the [Google Apps for Administrators Help Center](http://www.google.com/support/a/).

For background or general information on Windows administration, see [Microsoft TechNet](http://technet.microsoft.com/).

## 2 Before You Install ##

Before you install and configure the connector, make sure your Windows environment meets the prerequisites, and that your Google Calendar and Exchange accounts are prepared for sharing data.

### Prerequisites ###

  * A Google Apps Premier or Education Edition domain

> To support the Sync Service, your Windows environment must meet these prerequisites:
  * Microsoft Windows Server 2003
  * Internet Information Services (IIS) 6.0
  * Microsoft Exchange Server 2003 SP2 or Microsoft Exchange Server 2007 SP1 or SP2
  * Exchange 2003 Native Mode or 2007/2003 Mix Mode Environment or 2007 Native mode with addition of public folders
  * ASP.NET 2.0 runtime

### Google Calendar Account Setup ###

Create a dedicated Google Apps account and perform an initial login. The user name and password are required later when you populate the GoogleApps.AdminUser.Login and Password parameters in the configuration process. NOTE: This account does NOT need admin access to the Google Apps domain. If you use a non-admin account make sure you configure the SyncService.FreeBusy.DetailLevel to "BASIC" as the sync will fail for users who only share free/busy data if you don't use the "BASIC" feed option.  Before this account can be used with the Sync service you must login to Google Apps account and accept the standard terms of service agreement for the user account.


### Instructions applicable when running IIS 7 ###

If you are using a server OS using IIS 7 you will need to configure the /public website to accept double escaped content.

This needs to be performed on the server which is configured as the Free/Busy server. This server is by default defined by the setting Exchange.ServerName or Exchange.FreeBusyServerName value.

For details on how to allow double escaped content review this Microsoft Support KB article: http://support.microsoft.com/kb/942076/

### Instructions applicable only for Exchange 2007 ###

Sync Service uses public folders to store free/busy information of Google Calendar users in Exchange. If Exchange 2007 doesn't already have public folders added and attached to mailbox databases, they should be added.

Run the following cmdlet in Exchange PowerShell to configure free/busy for Google Calendar users
  * Indicate to Exchange 2007 that the free/busy information for all Google Calendar users should be queried from public folders
```
 Add-AvailabilityAddressSpace -ForestName domain.com -AccessMethod PublicFolder
 Example: Add-AvailabilityAddressSpace -ForestName pilot.domain.com -AccessMethod PublicFolder
```
> If the free/busy information of other common calendar resources (like room resources) on Google Calendar need be provisioned on Exchange, the above cmdlet should be run with the _ForestName_ value set to the SMTP domain of the resource's email. Typically, this value would be _resource.calendar.google.com_. An additional Sync service instance also should be configured to publish these resources' free/busy on Exchange (see **Multiple Sync Service Instances** section below for details on how to configure multiple instances of Sync service).

NOTE: If there is no default public folder database in Exchange 2007 follow these instructions for how to create the public folder store: http://technet.microsoft.com/en-us/library/bb123687(EXCHG.80).aspx

### Exchange User Mapping ###

Each Google Calendar user must have a corresponding mail-enabled Active Directory user, contact object or mailbox-enabled  (Exchange 2003 only) user object.

See the [Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview) for more information.

For instructions on how to create a mail-enabled users, see the Microsoft TechNet article, [How to Create a New Mail-Enabled User](http://technet.microsoft.com/en-us/library/bb125258.aspx).

For instructions on how to create an Active Directory contact, see the Microsoft TechNet article, [How to Create a Contact in Active Directory](http://technet.microsoft.com/en-us/library/aa995841.aspx).

### Dedicated Windows Service Accounts ###

The Sync Service requires dedicated Active Directory role accounts for performing LDAP queries against Active Directory and performing free/busy reads and writes against Exchange.

Create these service accounts as described in this section, and reference the user names and passwords when configuring the Sync Service.

The required service accounts are itemized below:

### Active Directory LDAP Query User ###

Create a dedicated user for performing Active Directory LDAP queries. The user name and password are required later when you populate the `ActiveDirectory.DomainUser.Login` and `Password` parameters in the Sync Service configuration file.

### Exchange Query Admin User ###

Create a dedicated user for the Sync Service to read and write free/busy against Exchange. The user name and password are required later when you populate `Exchange.GCalQueryAdmin.Login` and `Password` parameters in the Sync Service configuration file.

The **Exchange Query Admin** user needs permissions to be able to read and write to the free/busy Public folder store. To configure the permissions for the query admin user:

NOTE: In the past "Receive As" permission was required for a GCC feature that is now depreciated, and thus no longer necessary.

**Exchange 2003**

  1. Open Exchange System Manager
  1. Browse to Folders | Public Folders
  1. Right Click and select show View System Folders
  1. Expand SCHEDULE+ FREE BUSY
  1. Right Click on the EX:/o=First Organization/ou=First Administrative Group/ (This name may vary based on your setup)
  1. Select the Permissions tab
  1. Select Client Permissions
  1. Click Add
  1. Select the  query admin user account you created
  1. Select Roles: Owner
  1. Click Ok
  1. Click OK

**Exchange 2007**
  1. Allow access to **Exchange Query Admin** role account to write free/busy into public folders (NOTE: the value of _Identity_ flag may be different for different organizations)
```
 Add-PublicFolderClientPermission -Identity _Public Folder Identity_ -User Exchange.GCalQueryAdmin.Login -AccessRights Owner
 Example: Add-PublicFolderClientPermission -Identity "\NON_IPM_SUBTREE\SCHEDULE+ FREE BUSY\EX:/o=First Organization/ou=Exchange Administrative Group (FYDIBOHF23SPDLT)" -User syncuser -AccessRights Owner
```

### Remounting the Exchange Information Stores ###

The Exchange Information store caches permissions data. To flush the cache immediately after setting new permissions, dismount and remount the appropriate information store or restart the Information Store service. Otherwise the changes will not take effect until Exchange rebuilds the permissions cache.

To dismount and remount the public folder store:
  1. Open Exchange System Manager
  1. In tree view, expand **Administrative Groups**, _{Your Administrative Group}_, **Servers**, _{Your local Exchange server}_, and _{Your Storage Group}_.
  1. Right click on the **Public Folder Store**, select **Dismount Store** and click **Yes** to continue.
  1. Right click on the **Public Folder Store**, select **Mount Store** and click **Yes** at the success message dialog.

To restart the service Microsoft Exchange Information Store:
  1. Open the **services.msc**
  1. In right pane view, locate **Microsoft Exchange Information Store**, right click and select **Restart**

### Installation Checklist ###

| **Task** | **Completed?** | **Notes** |
|:---------|:---------------|:----------|
| Verify that Windows environment prerequisites are met |  |  |
| Create contact objects for Google Apps users in Exchange. |  |  |
|Create dedicated Google Apps Sync Service user |  |User name and password: |
|Create dedicated Active Directory user|  |User name and password: |
|Create dedicated Exchange Admin user|  |User name and password: |
|Set Exchange permissions for Query and Admin users |  |  |

## 3 Installing the Google Calendar Connector Sync Service ##

### MSI Installation ###

The installer for the Sync Service is provided in the installation package file _GoogleCalendarConnectorSyncService.msi_.

To install the Sync Service:

  1. Locate and open the file _GoogleCalendarConnectorSyncService.msi_.
  1. In the welcome dialog, click **Next**.
  1. In the **Select Installation Folder** dialog, select the desired folder or accept the default value.
  1. In the **Select Installation Folder** dialog use the radio buttons at the bottom of the dialog to determine whether to install the service only for yourself, or for all users, and click **Next**.
  1. Click **Next** to begin the installation, and then Close when the dialog displays the message "Installation Complete".

### Multiple Sync Service Instances ###

If you need to sync more than one domain with the Google Calendar Sync Service you can use the following instructions for setting up multiple instances of the Google Calendar Sync Service on a single server.

  1. Install the Google Calendar Sync Service
  1. Copy the default installation of the Google Calendar Sync Service to a new folder, for example "C:\Program Files\Google\Google Calendar Connector Sync Service - domain.com"
  1. Run the sc command to create a new service entry for the new path and exe
> _NOTE: There is a single space after binpath= and start=_
```
 %WINDIR%\System32\sc.exe create "Google Calendar Sync Service - domain.com" binpath= "C:\Program Files\Google\Google Calendar Connector Sync Service - domain.com\GoogleCalendarSyncService.exe" start= auto
```
  1. Create new logging folder paths
```
 mkdir C:\google\logs\Domain.com
 mkdir C:\google\data\Domain.com
```
  1. Configure new logging paths:  Update the new config file for the new domain with the following in the GoogleCalendarSyncService.exe.config adding the domain.com directory to each logging path
```
 <add key="GoogleApps.GCal.LogDirectory" value="C:\google\logs\domain.com\"/>
 <add key="SyncService.XmlStorageDirectory" value="C:\Google\data\domain.com\" />
 <file value="C:\Google\logs\domain.com\SyncService.log" />
 initializeData="c:\google\logs\domain.com\SyncNetTrace.log"
```
  1. Update the specific Google Apps Domain Values
```
 <add key="GoogleApps.DomainName" value="domain.com" />
 <add key="GoogleApps.AdminUser.Login" value="user" />
 <add key="GoogleApps.AdminUser.Password" value="password" />
```


## 4 Configuring the Google Calendar Connector Sync Service ##

Configuration settings for the Sync Service are stored in the configuration file _GoogleGCalExhangeSync.Service.exe.config_. You must edit this file and enter values appropriate to your setup.

### Editing the Configuration File ###

The configuration file _GoogleGCalExhangeSync.Service.exe.config_ is located in the installation directory for the sync service, by default "_C:\Program Files\Google\Google Calendar Connector Sync Service_". The "appSettings" section of _GoogleGCalExhangeSync.Service.exe.config_ contains configuration keys for setting up the Sync Service.

**Note**: If you have already set up the Google Calendar Connector Web Service in your environment, the Sync Service should use most the same settings for the following subsections of appSettings:

  * Active Directory
  * Exchange
  * Google Apps

| Configuration Key | Description | Default Value |
|:------------------|:------------|:--------------|
| ActiveDirectory.DomainController | Specify the fully qualified domain name of a domain controller for your Windows Active Directory domain. This server is used for LDAP queries to retrieve user login and Exchange user attributes. For queries directly against a domain controller, prefix the machine name with LDAP://. For queries against the Global Catalog, prefix the machine name with GC://. For example: `<add key="ActiveDirectory.DomainController" value="LDAP://HQAD1.corp.acme.com"/>` |  |
| Exchange.ServerName | The full qualified domain name of the Exchange server that hosts a copy of the Public Free/Busy Information Store. For example: `<add key="Exchange.ServerName" value="HQEXCH1.corp.acme.com"/>` |  |
| ActiveDirectory.DomainUser.Login | The userPrincipalName for the user account used by the sync service to perform Active Directory lookups. [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on this user account. For example: `<add key="ActiveDirectory.DomainUser.Login" value="ADDomainUser@corp.acme.com"/>` |  |
| ActiveDirectory.DomainUser.Password | The password for the user account used by the sync service to perform Active Directory lookups. |  |
| Exchange.ServerName | The full qualified domain name of the Exchange server that hosts a copy of the Public Free/Busy Information Store. For example: `<add key="Exchange.ServerName" value="HQEXCH1.corp.acme.com"/>` |  |
| Exchange.GCalQueryAdmin.Login | The userPrincipalName of the user account used when writing data to the public folder free/busy store.  See [Dedicated Windows Service Accounts](#Dedicated_Windows_Service_Accounts.md) for more information on this user account. For example: `<add key="Exchange.GCalQueryAdmin" value="GCalQueryAdmin@corp.acme.com"/>`|  |
| Exchange.GCalQueryAdmin.Password | The password for the user account used to write free/busy data to the public folder store. |  |
| Exchange.MailboxURITrailingPath | This key changes the trailing URI when accessing an Exchange user's calendar. Typical URL for calendar web access looks like _http://[Exchange.ServerName]/exchange/[user's primary email]/calendar/. However, when running localized versions of Exchange the trailing URI_calendar_is specific to the localized language and the value must be customized to match the localized string. (Ex._Calend%C3%A1rio_on a Portuguese Exchange Server). Value of this key should be given the value of the trailing URI string (i.e._Calend%C3%A1rio_). This key may need modification in only rare cases._| calendar |
| Exchange.FreeBusyServerName | This setting allows the definition of a specific Exchange server to use when reading and writing free/busy data. By default the value Exchange.ServerName is used. Use this value if the default Exchange server does not host a replica of the Free/Busy Information Store. To enable this setting remove comments and define the fully qualified domain name of the server to use. For Example: `<<add key="Exchange.FreeBusyServerName" value="http://HQEXCH2.corp.acme.com"/>` | DISABLED |
| GoogleApps.DomainName | The domain name used by Google Apps. For example: `<add key="GoogleApps.DomainName" value="acme.com" />` |  |
| GoogleApps.AdminUser.Login | The Google Apps user account name used for querying Google Calendar. This account must be a dedicated account in Google Apps setup for use with the Google Calendar Connector. This account does NOT need admin access to the Google Apps domain. For example: `<add key="GoogleApps.AdminUser.Login" value="gcc_syncsvc"/>` NOTE: If you use a non-admin account make sure you configure the SyncService.FreeBusy.DetailLevel to "BASIC" as the sync will fail for users who only share free/busy data if you don't use the "BASIC" feed option. |  |
| GoogleApps.AdminUser.Password | The password for the Google user account used for querying Google Apps and Google Calendar. |  |
| GoogleApps.GCal.EnableHttpCompression | This setting determines whether or not to enable GZip compression with the GDATA API calls to Google Apps. The default setting is true for optimum performance. `<add key="GoogleApps.GCal.EnableHttpCompression" value="true"/>` | true |
| GoogleApps.GCal.LogDirectory | This setting defines the directory to write debug output for the GDATA API feed for each user account queried from Google Apps. An individual file is written out per user request. The output contains the calendar feed data for the specific user. If no value is defined no output is written. For Example: `<add key="GoogleApps.GCal.LogDirectory" value="C:\Google\logs"/>` | DISABLED |
| GoogleApps.GCal.DomainMapping | This setting allows the definition of SMTP domain name mappings. Use this setting if your Exchange Primary SMTP address is different from the Google Apps SMTP domain name. When enabled the connector will properly map the external Google Apps SMTP domain name to the internal Exchange SMTP domain name. This setting by default is commented out in the config file. To enable this setting remove comments and define the External and Internal SMTP domain names. For example: `<add key="GoogleApps.GCal.DomainMapping" value="acme.com,exchange.acme.com"/>` To configure multiple domain mappings use ";" as the delimiter between the domain mapping values. For Example: `<add key="GoogleApps.GCal.DomainMapping value="acme.com,exchange.acme.com;company.com,exchange.company.com"/>` NOTE: This value is case sensitive. If  the user's SMTP domain does not match the case it will not match the mapping value. | DISABLED |
| Exchange.DefaultDomain | This setting prevents Exchange WebDav redirects to servers outside of the DNS domain defined. Enable this feature if the connector should not redirect Exchange server outside of this DNS domain. For example: `<add key="Exchange.DefaultDomain" value=".corp.acme.com"/>` | DISABLED |
| SyncService.ErrorCountThreshold | This setting specifies how many errors the service allows before aborting the current synchronization run. If the value is reached during a synchronization run it halts. Once the SyncService.RefreshTimeInMinutes has expired the counter is reset to 0 and a synchronization starts again. `<add key="SyncService.ErrorCountThreshold" value="20"/>` | 20 |
| SyncService.LDAPUserFilter | This setting allows the option to define an LDAP filer which determines which users or contacts are included in the synchronizing of data  from Google Calendar to Exchange. For example to include all members of an Active Directory Group: `<add key="SyncService.LDAPUserFilter" value="(memberof=CN=GCAL_Users,CN=Users,DC=corp,DC=acme,DC=com)"/>` _NOTE: If left blank, all users in Active Directory are included in the sync process._ |  |
| SyncService.RefreshTimeInMinutes | This setting specifies the interval in minutes to sleep between each completed synchronization before starting again. `<add key="SyncService.RefreshTimeInMinutes" value="15"/>` | 15 |
| SyncService.ThreadCount | This setting specifies the number of threads to create when performing a synchronization from Google Calendar in to Exchange. The more threads the fast the sync will complete. NOTE: The more threads the more system and network resources the sync service will utilize. `<add key="SyncService.ThreadCount" value="10"/>` | 1 |
| SyncService.XmlStorageDirectory | This setting specifies where the service stores data cached as XML documents. _NOTE: The service needs write access to this directory._ `<add key="SyncService.XmlStorageDirectory" value="C:\Google\data"/>` | C:\Google\data\ |
| SyncService.DirectorySearch.TimeoutInSeconds | This setting specifies the LDAP query timeout for Active Directory queries, in seconds. | 300 |
| Configuration.EncryptOnNextRun | If set to “true”, the configuration file is automatically encrypted the next time the application runs. See [Encrypting the Configuration File](#Encrypting_the_Configuration_File.md) for more information. | false |
| SyncService.FreeBusy.DetailLevel | If set to "Full", enables distinguishing between tentative and busy in the Free/Busy lookups. Setting it to "Basic" treats both tentative and busy as busy. | Full |
| SyncService.PlaceHolderMessage | This setting allows for customization of the subject text placed in each calendar placeholder appointment when running the Sync service SyncService.FreeBusy.Writer in Appointment mode. For example: `<add key="SyncService.PlaceHolderMessage" value="GCal Free/Busy Placeholder"/>` _NOTE: This property only applies to only new placeholder appointments being created. It will no go back and rewrite existing placeholder values._ | GCal Free/Busy Placeholder |
| SyncService.FreeBusy.DetailLevel | This setting defines the gdata feed type to use when retrieving free/busy data from Google Apps. The setting has two values Basic or Full. The basic feed only contains free and busy blocks. This means a meeting in GCal marked as "Maybe" will display as "Busy" when viewing the free/busy data. The full feed contains much more detail and includes the user's meeting response. Using this feed provides more verbose detail and  reflects the free/busy status more accurately. If the value is set to full a meeting in GCal marked as "Maybe" will be displayed as "Tentative when viewing the free/busy data. For Example: `<add key="SyncService.FreeBusy.DetailLevel" value="Full" />`| Full |






To edit the configuration file:
  1. Using your preferred XML or text editor, open _GoogleGCalExhangeSync.Service.exe.config_. The default location is C:\Program Files\Google\Google Calendar Connector Sync Service\.
  1. Scroll to the appSettings section and enter appropriate values for each of the required values shown in the table above.
  1. Save the file.
  1. Save or copy a backup copy of the file to a secure location.

### Encrypting the Configuration File ###

Because some configuration keys contain user names and passwords in plain text, it is recommended that you encrypt these configuration parameters. To enable encryption, set the value of the `Config.EncryptOnNextRun` key to _true_. The next time the sync service starts, the _appSettings_ keys will be automatically encrypted.
This encryption method will encrypt the entire appSettings node of the configuration file, rendering it unreadable. Once encrypted, the settings are no longer in plain text on the file system. However, settings can still be changed through the IIS Manager.

### HTTP Proxy Configuration Optional ###

The Sync Service can optionally be configured to work with an HTTP proxy. This configuration may be required if all out-bound communication is routed through a proxy. Configuring the HTTP proxy parameters in _GoogleGCalExhangeSync.Service.exe.config_ would instruct the Sync Service to properly forward its communication through such a proxy.

Microsoft Support includes an overview of the same process and configuration in the following Knowledge Base article: http://support.microsoft.com/kb/307220.

The table below outlines the configuration parameters located under the `<system.net.defaultProxy>` node of _GoogleGCalExhangeSync.Service.exe.config_:
| **Configuration Key** | **Description** | Default Value |
|:----------------------|:----------------|:--------------|
| bypasslist.address | Allows the Sync Service to bypass the proxy for additional, non-local addresses. As per Microsoft's instructions, this field can contain a host name, or a regular expression. |  |
| proxy.usesystemdefault | Instructs the .NET client to either use the default system profile for access, or to use a custom proxy defined in `proxy.proxyaddress`. |To override the system default and configure a custom web proxy for the Sync Service, this variable should be `FALSE` | True |
| proxy.proxyaddress | The URL:PORT pair for your proxy server. For example: `http://proxyserver.internal.yourdomain.com:3128` |  |
| proxy.bypassonlocal | Allows the Sync Service to bypass the proxy for local addresses containing a ".".	 | True |

_**Important Note: Please carefully consider whether your Microsoft Exchange server communication should be sent through a custom proxy. Including Microsoft Exchange servers in a custom proxy scheme could result in decreased performance and certain types of Windows authentication may fail**_.

## 5 Installation Success ##

### Start the Google Calendar Connector Sync Service ###

  1. Open the Windows Services Management console.
  1. Select the "Google Calendar Sync Service" and right-click for properties.
  1. Click "Start" if the service is not already started.
  1. Browse to log file path as defined by GoogleApps.GCal.LogDirectory (i.e. C:\google\logs)
  1. Review the log file SyncService.log and verify success messages

## 6 Testing and Troubleshooting ##

The Sync Service logs debugging information to a text log file, which is configurable through the log4net.config node in _GoogleGCalExhangeSync.Service.exe.config_. For more information, see [Logging Configuration](#Logging_Configuration.md).

Monitoring the debug output in the log files is the best way to troubleshoot issues with the sync service. It is recommended to use the freeware log file utility Baretail to monitor the file in real time.

Important Note: Please review [Logging Configuration](#Logging_Configuration.md) before beginning to test any of the functionality of the service.

### Running Diagnostics Tests ###

The Google Calendar Connector Web Service contains a diagnostics page that can help diagnose issues. **Diagnostics.aspx**, located in the root of the Web Service virtual directory, contains the following diagnostic tests relevant to the Sync Service:

  * **Verify free/busy data can be found in Google Calendar**
> This test attempts to retrieve free/busy information for a specific Google Calendar user. If the Sync Service and Google Apps are configured correctly, free/busy data should be returned.

  * **Verify free/busy can be written to Exchange**
> The Sync Service is by default configured to write free/busy data directly into the Exchange public free/busy store. This test attempts to perform a free/busy write to the Exchange public store for the specified user.

To run diagnostic tests:

  1. Use a web browser to navigate to `/Diagnostics.aspx` in the root folder of the Web Service's virtual directory.
  1. Select a test and enter any optional details in its text area.
  1. Click "**Verify**" to run the diagnostic and view its results.

### Logging Configuration ###

The Google Calendar Connector Sync Service includes a configuration node named 

&lt;log4net&gt;

_in_GoogleGCalExchangeSync.Service.exe.config_which controls the logging behavior of the Sync Service._

To use logging to troubleshoot Sync Service issues:
  * Verify an appropriate log path
  * Increase the logging level for more information
  * Verify log file system permissions if the file is not created.

### Verify Log Path ###

To change where the log file is stored, edit the node path _configuration/log4net/appender/file_ and change the value attribute of the file node to the new location. Include the file name in the value attribute. _NOTE: The SYSTEM account needs "**Modify**" access to the file system path_.

### Increase the logging level ###

There are four logging levels, with DEBUG providing the most output and ERROR only logging severe events:
  * DEBUG
  * INFO
  * WARN
  * ERROR

The Sync Service has many potential logging points for the DEBUG and ERROR levels. When logging is set to these levels, the size of the log file may grow very rapidly.

To set the Sync Service logging to a certain level, edit the node path `configuration/log4net/root/level` and change the value attribute to one of the four levels listed. Lower severity levels are inclusive of higher levels. For example if the Sync Service is set to INFO, it also logs WARN and ERROR but not DEBUG messages.

### Verify log file permissions ###

Verify that the following log file permissions have been set up properly:

In _GoogleGCalExchangeSync.Service.exe.config\appSettings\_:

  * **`SYSTEM`** should have "**MODIFY**" privileges to the `GoogleApps.GCal.LogDirectory` directory.
  * **`SYSTEM`** should have "**MODIFY**" privileges to the `SyncService.XmlStorageDirectory` directory.
  * **`SYSTEM`** should have "**MODIFY**" privileges to the install directory or the file _GoogleGCalExchangeSync.Service.exe.config_.

In `GoogleGCalExchangeSync.Service.exe.config\configuration\log4net\appender\`:

  * `SYSTEM` should have "MODIFY" privileges to path defined configuration `\log4net\appender\file`.

To grant "Modify" access to a directory do the following:
  1. Open Windows Explorer.
  1. Navigate to the directory root folder (i.e. C:\Google\Data).
  1. Right click on the folder and select **Properties**.
  1. Select the **Security** tab and click **Add**, type SYSTEM and click **OK**.
  1. With the **`SYSTEM`** user highlighted, check **Allow** for **Modify** privileges.


## 7 Known Issues ##

  * See [Google Code Project](http://code.google.com/p/google-calendar-connectors/wiki) for any known issues.


## 8 Copyright Notices ##

Library and license attributions are provided to conform with the Apache License, Version 2.0. A copy of the Apache License, Version 2.0 can be found [here](http://www.apache.org/licenses/LICENSE-2.0.html). The following licenses and libraries are used in the Google Calendar Connector Sync Service:

Google Data (GData) API .NET Client Library and its dependencies licensed under the **Apache License, Version 2.0.** ([project](http://code.google.com/p/google-gdata/), [license](http://www.apache.org/licenses/LICENSE-2.0))
Apache log4net licensed under the **Apache License, Version 2.0**. ([project](http://logging.apache.org/log4net/index.html), [license](http://logging.apache.org/log4net/license.html))
tz4net v3.0.2.0 licensed under the **GNU LGPL V2**. ([project](http://www.babiej.demon.nl/Tz4Net/main.htm), [license](http://www.gnu.org/copyleft/lgpl.html))

Portions Copyright (c) 2002 James W. Newkirk, Michael C. Two, Alexei
A. Vorontsov or Copyright (c) 2000-2002 Philip A. Craig


---


_Google, Google Calendar, Google Calendar Connector, Google Calendar Connector Web Service, Google Calendar Connector Sync Service_ are trademarks of Google, Inc.
All other company and product names may be trademarks of the respective companies with which they are associated.