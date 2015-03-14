Summary The current release notes

# Introduction #

The release notes show bugfixes and feature additions over the course of the project.
The wiki page shows the release notes for the current binary download, the subversion project contains a text version with the most recent changes.

# Details #

[Latest Release Notes](#Release_Notes.md)

[Latest Upgrade Guide](#Upgrade_Guide.md)


---


## Release Notes ##

### Google Calendar Web Service v1.3.3 and Google Calendar Sync Service v1.3.1: ###

1) AppointmentWriter option is no more supported.

2) Updated GData calendar library to v1.7


---

## 1.3.1.0 ##
This release is done only for Web Service.

### Google Calendar Web Service: ###
1) Multiple login support. This is an internal change and there is no change required in the configuration file to use this binary.

Users who have upgraded their calendar to multi login, they have to update their GCC Web Service to version 1.3.1.0


---

## 1.3.0.0 ##

### Google Calendar Sync Service: ###

1) Added support for Exchange 2007. Additional configuration steps are required during installation. Please refer to SyncServiceGuide.wiki for details.

2) Bug fixes -

2.1) The Sync Service uses WebDAV to write calendar info from Google Calendar to Exchange. A typical URL for calendar web access looks like _http://[Exchange.ServerName]/exchange/[user's primary email]/calendar/. However, when running localized versions of Exchange the trailing URI_calendar_is specific to the localized language and it must be customized to match the localized string. (e.g._Calend%C3%A1rio_on a Portuguese Exchange Server)._

> To address this issue, the trailing URI was made configurable. The following key's value should contain the trailing component of the URI (e.g. _Calend%C3%A1rio_):

> GoogleCalendarSyncService.exe.config changes:
```
 <!-- <add key="Exchange.MailboxURITrailingPath" value="calendar"/> -->
```

> Default Value: "calendar"

2.2) The Sync Service was found to be timing out and in deployment environments, unresponsive/hanging after a period of time in some customers' environments. COM exceptions were logged and memory leaks were found in most of those cases.

> COM exceptions were addressed and a few memory leaks were identified and fixed. Additionally, one possible causes of hanging was identified to be the logic around Active Directory search and this was fixed by adding timeouts to the code. The timeout value is customizable via the config:

> GoogleCalendarSyncService.exe.config changes:
```
 <!-- <add key="!SyncService.DirectorySearch.TimeoutInSeconds" value="300"/> -->
```

> Default Value: 300 (seconds).

### Google Calendar Web Service: ###

1) Added Exchange 2007 support. Further, the free/busy fetch can be configured to be done either using the legacy WebDAV API or using the Exchange Web Services (EWS) API. The latter is highly recommended by Microsoft and the free/busy results retrieved are up-to-date (while the former API returns Free/Busy from public folders which are updated only periodically).

> The choice of API is set by using the following config key (in web.config):
```
  <!-- API to be used to interact with Exchange -
       "EWS" for Exchange 2007 or "WebDAV" (default value) for legacy Exchange -->
  <add key="Exchange.ExchangeAPI" value="WebDAV"/>
```

> Default Value: "WebDAV"

> NOTE: "WebDAV" is the only option available for Exchange 2003 environments.

2) Added support for querying/uniquely identifying users by specifying a configurable LDAP query.

> In the earlier releases, users were identified by their primary SMTP email id. However, with at least one customer, primary SMTP was formatted in a not-so-friendly way and they wanted their users to be identified by a user-friendly SMTP alias. To support such scenarios (and various other), the LDAP query was made customizable:

```
  <!-- Custom LDAP query for Active Directory lookup -->
  <!-- <add key="Exchange.LDAPQuery" value="(mail={0})"/> -->
```

> Default Value: (mail={0})
> Note that the above LDAP filter is equivalent to querying for users by their primary SMTP addresses - the default behaviour so far.
> Example 1: If the users have unique UPNs, the filter has to be set to "(userPrincipalName={0})".
> Example 2: If the users can be identified either by their UPN or by their email, the filter should be set to "(|(userPrincipalName={0})(mail={0}))"


---


## 1.2.0.289 ##

### Google Calendar Sync Service: ###

1) Added a new feature to retrieve a full detailed calendar feed to display a more detailed free/busy view. This allows Google Calendar "Maybe" responses to be displayed as "Tentative" in Exchange.

> Default Value: Full

> GoogleCalendarSyncService.exe.config changes:
```
<!-- Free/Busy detail level settings --> 
<!-- Setting it to Full enables distinguishing between tentative and busy in the Free Busy look ups -->
<!-- Setting it to Basic treats tentative and busy as busy in the Free Busy look ups -->
<add key="SyncService.FreeBusy.DetailLevel" value="Full" />
```

2) Customize "Appointment" sync placeholder text

_NOTE: This only applies to only new placeholder appointments being created. It will no go back and rewrite existing placeholder values._

> Default Value: "GCal Free/Busy Placeholder"

> GoogleCalendarSyncService.exe.config changes:
```
 <!-- Customize Appointment placeholder message text -->
 <!-- <add key="SyncService.PlaceHolderMessage" value="GCal Free/Busy Placeholder" /> -->
```

3) Experimental Feature Update: Write meeting details when syncing in "Appointment" mode.

_NOTE: The option of writing Google Calendar free/busy data into an Exchange mailbox is possible, but is not the recommended configuration. The appointment writer has very limited functionality and is considered an experimental feature._

> This feature only writes the appointment details when it creates the placeholder appointment on the Exchange calendar for the very first time. This means if the appointment placeholder already exists no changes such as subject, description etc will not be updated on future syncs. If however, the meeting time changes a new meeting invite is created at a new time and therefore the details of the meeting will be written when the new entry is created.

> Default Value: False

> To enable add the following to GoogleCalendarSyncService.exe.config:
```
<add key="SyncService.SyncAppointmentDetails" value="true" />
```

4) Added connectivity back off algorithm for error conditions.

> This feature implements a sleep state when a network or server error is received. For example, if the server responds with an error the sync service will sleep for 1000ms (± 33%) then try to connect again. If another error condition is received the sleep value will increase to 2000ms (± 33%) and continues to grow exponentially up to a max of ~30 minutes. Once any successful response is received the sleep value is reset to 1000ms.


5) Removed the need for a Exchange free/busy template user. Free/busy messages are created from scratch now.

> _**Deprecated Web.Config Values:**_
```
 <add key="SyncService.FreeBusy.AdminGroup" value="/o=GooLab/ou=First Administrative Group" />
 <add key="SyncService.FreeBusy.TemplateUserName" value="GCalExchangeTemplate" />
```


### Google Calendar Web Service: ###

1) Added security feature to by default only allow access to the Diagnostics page from the local machine.

> Default Value: True

> Web.config changes:
```
<!-- Allow diagnostics page to be accessible from location other than localhost -->
<add key="WebService.RequireLocalAccessforDiagnostics" value="true"/>
```
```
<system.web>
   <customErrors mode="RemoteOnly" defaultRedirect="errors/errors.htm">
   <error statusCode="403" redirect="errors/403.htm"/>
   </customErrors>
 </system.web>
```

2) Added Encrypt web.config button to diagnostics page. Removed the option from web.config

> _**Deprecated Web.Config Value:**_
```
<add key ="Configuration.EncryptOnNextRun" value="false"/>
```

3) Removed the need for a Exchange free/busy template user. Free/busy messages are created from scratch now.

> _**Deprecated Web.Config Values:**_
```
<add key="SyncService.FreeBusy.AdminGroup" value="/o=GooLab/ou=First Administrative Group" />
<add key="SyncService.FreeBusy.TemplateUserName" value="GCalExchangeTemplate" />
```

### Google Calendar Connector Plugin: ###

1) Added connectivity back off algorithm for error conditions.

> This feature implements a sleep state when a network or server error is recieved. For example, if the server responds with an error the Plugin will sleep for 10ms (± 25%) then try to connect again. If another error condition is received the sleep value will increase to 20ms (± 25%) and grow exponentially up to a max of 40 seconds (± 25%). Once any successful response is received the sleep value is reset to 10ms.


---


## Upgrade Guide ##

## 1.2.0.289 ##

### Google Calendar Connector Web & Sync Service ###

1) Make a backup of the current .config files.
  * Web Service: web.config
  * Sync Servce: GoogleCalendarSyncService.exe.config

2) Install the new versions of the Google Calendar Web Service and Google Calendar Sync Service from the .msi packages.

3) Edit the newly installed web.config using your backup copy as a guide. Since there are new settings and some old settings have been deprecated, please do not simply replace the newly installed .config file with your backup copy.

_NOTE: If you encrypted the file you can review the settings of the file in IIS Manager by viewing the Properties of the virtual web directory where you installed the Google Calendar Web Service._

4) Edit the newly installed GoogleCalendarSyncService.exe.config using your backup copy as a guide. Since there are new settings and some old settings have been deprecated, please do not simply replace the newly installed .config file with your backup copy.

5) Verify that all the Web Service Diagnostics.aspx tests pass successfully.

6) Verify that an Exchange user free/busy look up from within Google Calendar works successfully.

7) Start the Google Calendar Connector Sync Service and verify that no errors are reported in the SyncService.Log.

8) The user account configured as SyncService.FreeBusy.TemplateUserName is no longer required. If you created a specific user for this purpose, you can delete this account's Active Directory user and Exchange mailbox.

### Google Calendar Connector Plugin: ###

1) Backup your current config.txt.

2) Stop the Google Calendar Connector Plug-In service.

3) Replace the bin install directory with the new binary files.

4) Replace the config.txt with your backup copy.

5) Start the Google Calendar Connector Plug-In service.



---


## 1.1.0 ##
- Initial Release -