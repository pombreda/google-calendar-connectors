# This Plugin Guide is Deprecated. The Groupwise Plugin Is NOT supported. Use the [Google Calendar Sync Service](http://code.google.com/p/google-calendar-connectors/wiki/SyncServiceGuide). #





# Google Calendar Connector Plug-in Installation and Configuration Guide 1.2 #

The Google Calendar Connector Plug-in utilizes the Novell GroupWise Connector for Microsoft Exchange and retrieves Google Calendar free/busy data in real-time.
This document gives instructions for installing and configuring the Google Calendar Connector Plug-in.
Contents

  1. [Background](#1_Background.md)
    1. [For More Information](#For_More_Information.md)
  1. [Prerequisites](#2_Prerequisites.md)
  1. [Installation of Novell GroupWise connector](#3_Installation_of_Novell_GroupWise_connector.md)
    1. [Run Microsoft Exchange setup](#Run_Microsoft_Exchange_setup.md)
    1. [Install Microsoft Exchange calendar connectors](#Install_Microsoft_Exchange_calendar_connectors.md)
    1. [Reinstall any service pack and hotfixes](#Reinstall_any_service_pack_and_hotfixes.md)
  1. [Configuration of Novell GroupWise connector](#4_Configuration_of_Novell_GroupWise_connector.md)
    1. [Create an Active Directory Organizational Unit](#Create_an_Active_Directory_Organizational_Unit.md)
    1. [Create a shared folder for PlugIn](#Create_a_shared_folder_for_PlugIn.md)
    1. [GroupWise connector options](#Configure_GroupWise_connector_options.md)
    1. [Configure the Calendar Connector](#Configure_the_Calendar_Connector.md)
  1. [Google Calendar Connector PlugIn Installation](#5_Google_Calendar_Connector_PlugIn_Installation.md)
    1. [Install the Google Calendar Connector PlugIn](#Install_the_Google_Calendar_Connector_PlugIn.md)
    1. [Define LDAP filters Optional](#Define_LDAP_filters_Optional.md)
    1. [Mail routing for Google Apps contacts](#Mail_routing_for_Google_Apps_contacts.md)
    1. [Install PlugIn as a service](#Install_PlugIn_as_a_service.md)
    1. [Test configuration with Exchange gwrouter](#Test_configuration_with_Exchange_gwrouter.md)
    1. [Start Connector for Novell GroupWise](#Start_Connector_for_Novell_GroupWise.md)
    1. [Start Router for Novell GroupWise](#Start_Router_for_Novell_GroupWise.md)
    1. [Force an initial synchronization](#Force_an_initial_synchronization.md)
    1. [Start the Microsoft Calendar Connector service](#Start_the_Microsoft_Calendar_Connector_service.md)
  1. [Installation Success](#6_Installation_Success.md)
    1. [Execute Selftest](#Execute_Selftest.md)
    1. [Review the output of Selftest](#Review_the_output_of_Selftest.md)
    1. [Verify Calendar Look up](#Verify_Calendar_Look_up.md)
  1. [Enable Automatic Startup for Services](#7_Enable_Automatic_Startup_for_Services.md)
    1. [Enable Services](#Enable_Services.md)
  1. [Config Password Encryption](#8_Config_Password_Encryption.md)
    1. [Enable Encryption for Config Passwords](#Enable_Encryption_for_Config_Passwords.md)
    1. [Enabled Certificate Enrollment](#Enabled_Certificate_Enrollment.md)
    1. [Grant Administrative Rights](#Grant_Administrative_Rights.md)
    1. [Enroll in Certificate](#Enroll_in_Certificate.md)
    1. [Configure Service to Run As User Account and Encrypt Passwords](#Configure_Service_to_Run_As_User_Account_and_Encrypt_Passwords.md)
  1. [Troubleshooting](#9_Troubleshooting.md)
  1. [Copyright Notices](#10_Copyright_Notices.md)

## 1 Background ##

The Google Calendar Connector Plug-In service allows Microsoft Exchange (i.e. Outlook) users to see free/busy information for their Google Calendar colleagues from within the Exchange environment.  As Microsoft only provides two first-party connectors for its calendaring system, Google has utilized one of these, the Novell Groupwse Calendar Connector, to aid in sharing free/busy information.

The Google Calendar Connector Kit is a set of server-installed tools that require knowledgeable  installation and configuration, tailored to the particulars of a customer's environment.  These tools are only the **technological part** of the integration experience.

Before proceeding to install the Google Calendar Connector Plug-In, please read and understand the important **Security Considerations** outlined in the **Overview Guide**.

The Google Calendar Connector Plug-In, in particular, requires a sophisticated knowledge of Exchange system administration.

### For More Information ###

This document is part of the Google Calendar Connector document set, which includes the following related documents:

  * **[Overview Guide](http://code.google.com/p/google-calendar-connectors/wiki/Overview)**. Describes the components, process flow, and security considerations for the Google Calendar Connector Kit.

  * [Web Service Installation and Configuration Guide](http://code.google.com/p/google-calendar-connectors/wiki/WebServiceGuide). Provides instructions for installing and configuring the Web Service, which provides Exchange free/busy data flow to Google Calendar.

  * **[Sync Service Installation and Configuration Guide](http://code.google.com/p/google-calendar-connectors/wiki/SyncServiceGuide)**. Provides instructions for installing and configuring the Sync Service to provide free/busy data flow from Google Calendar to Exchange.

For background or general information on Google Apps, see the [Google Apps for Administrators Help Center](http://www.google.com/support/a/).

For background or general information on Windows administration, see [Microsoft TechNet](http://technet.microsoft.com/).



## 2 Prerequisites ##

In order for the Google Calendar Connector Plug-In to work, please make sure your environment satisfies the following requirements:

  1. Own a Google Apps Premier Edition domain.
  1. The Java SE Development Kit (JDK) 6 Update 2 (or later) must be installed. This is necessary because the Google Calendar Connector Plug-In is written in Java. JDK 6 Update 2 can be downloaded from http://java.sun.com/javase/downloads/index.jsp.
  1. The Microsoft .NET 2.0+ run-time environment must be installed.  The .NET run-time is required by the FreeBusyBuilder supplemental process.
  1. The Provisioning API must be enabled for your Google Apps domain (via the Administrator control panel). The Provisioning API allows the Google Calendar Connector Plug-In to programmatically access Google Apps accounts on your domain. To learn more about the Provisioning API, please visit http://code.google.com/apis/apps/gdata_provisioning_api_v2.0_reference.html.
  1. Access to an administrator account on your Google Apps domain. The Google Calendar Connector Plug-In will use this account to retrieve account and calendar information for your domain. This account should be dedicated for this purpose and not used concurrently by users or admins. The Plug-in periodically accesses your Google Apps domain, and any user sessions will be invalidated, logging the user out. Google recommends creating a specific administrator account for free/busy look up purposes.
  1. Your server should be running Microsoft Exchange Server 2003 with SP2 or later.
  1. Your server should be running Windows Server 2003 with IIS 6.0.
  1. You need a local replica of the SCHEDULE+ FREE BUSY public folder on the Exchange server running the Google Calendar Connector Plug-In. This requirement is related to the Microsoft Calendar Connector.

_More information about implementing connectivity between the Google calendar connector plug-in and Exchange is available at http://technet.microsoft.com/en-us/library/bb124065.aspx_

_More information about setting up a local replica of the SCHEDULE+ FREE BUSY Public Folder is available at http://technet.microsoft.com/en-us/library/bb124487.aspx_

The Microsoft documentation above explains Groupwse is limited to retrieving a maximum of 389 days of free/busy information. Trying to retrieve more will result in no data being returned.

## 3 Installation of Novell GroupWise connector ##

### Overview ###

The first step consists of installing and configuring the first-party Novell Groupwse Connector for Exchange developed by Microsoft. After installing the connector, you may have to re-install any service packs and hot-fixes you applied to the original Exchange install. This is not a Google-specific requirement.

  * 3.1 [Run Microsoft Exchange setup](#Run_Microsoft_Exchange_setup.md)
  * 3.2 [Install Microsoft Exchange calendar connectors](#Install_Microsoft_Exchange_calendar_connectors.md)
  * 3.3 [Reinstall any service pack and hotfixes](#Reinstall_any_service_pack_and_hotfixes.md)

### Run Microsoft Exchange setup ###

Depending on your installation, you may want to run either i386/setup.exe from your Exchange 2003 source installation folder or go to Add/Remove Programs on the Windows Control Panel and select "Change/Remove" Microsoft Exchange.

### Install Microsoft Exchange calendar connectors ###

Installing the Groupwse and Calendar connectors creates an interface by which the Google Calendar Connector Plug-In can interact with Exchange. Make sure to install both the Microsoft Exchange Connector for Novell Groupwse and the Microsoft Exchange Calendar Connector.

  * 3.2.0 Run Microsoft Exchange Setup
  * 3.2.1 Highlight the Microsoft Exchange row and set the Action to "Change"
  * 3.2.2 Select Install from the drop-down columns for Microsoft Exchange Connect for Novell Groupwse and Microsoft Exchange Calendar Connector.
  * 3.2.3 Click Next.
  * 3.2.4 Click Next to confirm the changes.

> Exchange will now install the new connectors. After setup is complete, you may be prompted to re-install an Exchange Server service pack.

### Reinstall any service pack and hotfixes ###

Updating the components of Microsoft Exchange in the previous step may have overwritten some changes made by service packs, you will need to reinstall the latest Microsoft Exchange 2003 Service Pack.

_**NOTE: For information on applying hotfixes and patches to Exchange, refer to http://support.microsoft.com/kb/328839**_

  * 3.3.1. Download Exchange Server 2003 Service Pack 2 (or later) from http://technet.microsoft.com/en-us/exchange/bb288486.aspx
  * 3.3.2. Run the installer.
  * 3.3.3. Pick a temporary directory to store the extracted setup files.
  * 3.3.4. Run the Service Pack updater by executing update.exe in the E3SP2ENG\setup\i386 directory.
  * 3.3.5. Agree to the Terms of Service.
  * 3.3.6. Select components to update.

Service Pack 2 for Exchange Server 2003 will now be installed and it will update the components marked with the "Update" Action.


## 4 Configuration of Novell GroupWise connector ##

### Overview: ###

This install stage involves relatively standard configuration of the Novell Groupwse connector. This step is concerned with specifying where the connector files are located on disk, how Groupwse (re: Google) users are treated in Active Directory and how the Novell connector should behave. Notice that this configuration is achieved entirely through built-in Exchange and Active Directory tools.

  * 4.1 [Create an Active Directory Organizational Unit](#Create_an_Active_Directory_Organizational_Unit.md)
  * 4.2 [Create a shared folder for PlugIn](#Create_a_shared_folder_for_PlugIn.md)
  * 4.3 [Configure !GroupWise connector options](#Configure_GroupWise_connector_options.md)
  * 4.4 [Configure the Calendar Connector](#Configure_the_Calendar_Connector.md)

### Create an Active Directory Organizational Unit ###

Microsoft recommends allocating a special Active Directory Organizational Unit for Groupwse (re: Google Apps) contacts (http://technet.microsoft.com/en-us/library/aa998406.aspx). During the process of full directory synchronization, when the Google Calendar Connector Plug-In returns contact information to Exchange, this organizational unit will be populated with the relevant users. After each directory synchronization initiated over the Novell API Gateway, Active Directory will contain a full set of Google Apps contacts in the special Organizational Unit. These contacts are used by Microsoft Exchange. You can configure a whitelist and blacklist to control what users are include or excluded from this import. Refer to section 5.2 [Define LDAP filters Optional](#Define_LDAP_filters_Optional.md) for details.

  * 4.1.1. Open the Active Directory Users and Computers management interface.
  * 4.1.2. Right-click on the root domain and select New->Organizational Unit.
  * 4.1.3. Name the Organizational Unit "GoogleApps" and press OK.


### Create a shared folder for PlugIn ###

> Create a local shared folder that will serve as the API Gateway Path for the Novell Groupwse connector. General Microsoft configuration instructions for the Groupwse connector can be found here: http://technet.microsoft.com/en-us/library/bb124483.aspx. Since the Google Calendar Connector Plug-In is running locally instead of on a remote Novell Gateway server, we need to create a local Exchange-accessible shared folder to serve as the API Gateway Path.

  * 4.2.1. Open the Computer Management console.
  * 4.2.2. Expand the local computer management tree until folder shares are visible.
  * 4.2.3. Right-click "Shares" and select "Create a New Share"
  * 4.2.4. Click "Next" and define a path to the folder you will use for the Google API Gateway. It cannot be located on a network mapped drive. A suggested path is: "C:\Program Files\Google\Google Calendar Connector Plug-In\".
  * 4.2.5. Respond "Yes" when prompted to create the folder.
  * 4.2.6. Add a description of your choosing to the folder share. We suggest: "Google Calendar Connector Plug-In UNC Working Share". Note that the share path is \\{Exchange Server}\{Share Name}.
  * 4.2.7. Select the permissions option "Administrators have full access; other users have read-only access."

### Configure GroupWise connector options ###

Next we configure the Exchange Connector for Novell Groupwse to utilize the appropriate locally shared API Gateway folder, act on all Google contacts, store imported contacts in the correct Active Directory Organizational Unit and schedule directory synchronization. For details on general configuration, refer to Microsoft's guide to configuring the Groupwse connector (http://technet.microsoft.com/en-us/library/bb124483.aspx).

  * 4.3.1. Open the Exchange System Manager console.
  * 4.3.2. Expand the Connectors tree to select the "Connector for Novell Groupwse"
  * 4.3.3. Right-click on the connector and select "Properties".
  * 4.3.4. On the General tab, set the "API Gateway Path" to the share path you specified in Step 4.2.4. For example, if your API Gateway folder is `C:\Program Files\Google\Google Calendar Connector Plug-In\` with a share path of `\\{Exchange Server}\Google Calendar Connector Plug-In`, set the API Gateway Path to: `\\localhost\Google Calendar Connector Plug-In`
  * 4.3.5. Click "Modify" next to the Netware Account field and enter the account details for a user who has full access privileges to the local share path. This is the account that the Novell Groupwse connector will use to read and write to the directory. _**NOTE: This account should be a local account on the server and be a member of the local Administrators group. We recommend creating a new local account for this specific purpose instead of using the existing local Administrator account**_.
  * 4.3.6. Move to the Address Space tab, since you need to assign at least one address space to the connector
  * 4.3.7. Choose "Add", select "GWISE" and press "OK".
  * 4.3.8. In the dialog that appears, put an asterisk `(*)` in the address field and press "OK". This enables the Groupwse connector for all users with a GWISE address. All free/busy requests for Google Apps users will be routed through the Novell Groupwse connector and then the Google Calendar Connector Plug-In service.
  * 4.3.9. On the Delivery Restrictions tab, ensure that "Accepted" is selected.
  * 4.3.10. Move to the Dirsync Schedule tab and specify the Exchange Groupwse directory update schedule to "Run Every Hour". This will specify the frequency at which the Novell Groupwse connector will try to synchronize directory information between Exchange and your Google Apps domain.
  * 4.3.11. Move to the Import Container tab, and select "Modify" next to the "Container" field
  * 4.3.12. Specify the Organizational Unit "GoogleApps", which you created in Step 4.1.3. This path is where the Groupwse connector will create contact objects for each Google Apps user accounts. _**NOTE: If there is an existing account in Exchange with the same SMTP address as Google Apps user no contact will not be created in this case**_.
  * 4.3.13. Acknowledge that the machine account running the Groupwse connector must be able to modify user accounts by selecting "Yes" to the popup. Notice that the Container now specifies GoogleApps.
  * 4.3.14. Move to the Export Containers tab and uncheck "Export groups". Verify that "Export contacts" is already unchecked. You do not need to export the Exchange users for this connector to work.
  * 4.3.15. Press "Apply" and "OK" to save changes.

### Configure the Calendar Connector ###

Microsoft Exchange's Calendar Connector enables the sharing of free/busy information between Exchange and the Google Calendar Connector Plug-In. We will configure Calendar Connector's awareness of the Novell Groupwse connector and its update schedule.

  * 4.4.1. Right-click on the "Calendar Connector" underneath the "Connectors" heading and select "properties".
  * 4.4.2. Open the "Calendar Connections" tab.
  * 4.4.3. Click "New"
  * 4.4.4. Select "Novell Groupwse" as the type of connector you want to connect to Exchange.
  * 4.4.5. Enter an arbitrary Groupwse API Gateway name. This has no precise bearing on the rest of the install process. Make sure your name follows the 

&lt;domain&gt;

.

&lt;gateway&gt;

 naming convention.
  * 4.4.6. Open the "Schedule" tab
  * 4.4.7. Select "Always".
  * 4.4.8 Select the General Tab and review and define the custom options for how requests are handled.
    * Number of days of free/busy information to request from Google Apps
    * Maximum age in minutes to used the existing data in free/busy cache before querying Google Apps. _**NOTE: 0 means always query Google Apps and ignore the data in the cache**_.
    * Maximum number of seconds to wait for a response from Google Apps for free/busy data.


## 5 Google Calendar Connector PlugIn Installation ##

### Overview ###

This is the final stage of the installation process, which involves bridging the Novell Groupwse API Gateway folder with the Google Calendar Connector Plug-In. This stage will also cover the configuration and installation of the Google Calendar Connector Plug-In as a Windows service. Both the connector for Novell Groupwse and the Calendar Connector will be enabled.
This process can be subdivided into the following areas:

  * 5.1 [Install the Google Calendar Connector PlugIn](#Install_the_Google_Calendar_Connector_PlugIn.md)
  * 5.2 [Define LDAP filters Optional](#Define_LDAP_filters_Optional.md)
  * 5.3 [Mail routing for Google Apps contacts](#Mail_routing_for_Google_Apps_contacts.md)
  * 5.4 [Install PlugIn as a service](#Install_PlugIn_as_a_service.md)
  * 5.5 [Test configuration with Exchange gwrouter](#Test_configuration_with_Exchange_gwrouter.md)
  * 5.6 [Start Connector for Novell !GroupWise](#Start_Connector_for_Novell_GroupWise.md)
  * 5.7 [Start Router for Novell !GroupWise](#Start_Router_for_Novell_GroupWise.md)
  * 5.8 [Force an initial synchronization](#Force_an_initial_synchronization.md)
  * 5.9 [Start the Microsoft Calendar Connector service](#Start_the_Microsoft_Calendar_Connector_service.md)

_**NOTE: Backslashes in config.txt must be escaped with another backslash. E.g., to set C:\Program Files, you should enter C:\\Program Files in config.txt**_

### Install the Google Calendar Connector PlugIn ###

This step copies the actual Google Calendar Connector Plug-In files into their install locations.

  * 5.1.1. Create a "bin" subdirectory beneath the folder share path. For example, `C:\Program Files\Google\Google Calendar Connector Plug-In\bin`. _**NOTE: Make sure that only the administrator has permission to read the config.txt file**_.
  * 5.1.2. Copy contents of the Google Calendar Connector Plug-In distribution into the \bin subdirectory.
  * 5.1.3. Open config.txt with a text editor
  * 5.1.4. Edit general.baseDirectory to point to the Share Folder Path defined earlier in Step [[4.2.4](#Create_a_shared_folder_for_PlugIn.md). E.g., C:\Program Files\Google\Google Calendar Connector Plug-In. Note, this service does not work with mapped drives. This tells the Google Calendar Connector Plug-In where files should be read and written.
  * 5.1.5. Verify general.verboseLogging = false.
  * 5.1.6. Verify general.logMessages = false.
  * 5.1.7. Verify general.logfile has been set to an appropriate path for service logging (if logging is enabled).
> _**NOTE: You must grant the account that runs Java run-time MODIFY access to your log file. If not using password encryption this is the "SYSTEM" account. If using password encryption this is the user service account defined to run the service**-.
  * 5.1.8. (Optional) Enable web request proxying for Google Apps data. This configures Google Calendar Connector Plug-In to send its web requests to Google Apps through your organization's proxy server. Un-comment the general.httpProxy and/or general.httpsProxy lines, depending upon whether your proxy server supports only regular HTTP or HTTPS. Enter the DNS or IP addresses and ports of your company's web proxy(s). For example, if your HTTP proxy is `http-proxy.acme.com` port `8000`, you would enter: `general.httpProxy=http-proxy.acme.com:8000`.
  * 5.1.9. Set gdata.username to an administrator account on your Google Apps domain. For Example: `gccplugin@acme.com`. This is the Google Apps account that will be used to access Google Calendar free/busy information. It is important that this account does not need to be used on a regular basis by a user because the use of this service may interfere with existing open sessions on the account.
  * 5.1.10. Set gdata.password to the corresponding password for the administrator account.
  * 5.1.11. Set gdata.domain to your Google Apps domain. For Example: `acme.com`.
  * 5.1.12. Check your configuration against the following checklist:_

| Configuration Key | Description | Default Value |
|:------------------|:------------|:--------------|
|general.baseDirectory | This setting defines the base directory where the Google Calendar Connector Plug-In is installed. This is also the API Share Path where the Exchange connector expects to find the API\_IN and API\_OUT folders. For Example: `general.baseDirectory=C:\\Program Files\\Google\\Google Calendar Connector Plug-In` | C:\\Program Files\\Google\\Google Calendar Connector Plug-In |
| general.logFile | This setting defines the path and file name for the Google Calendar Connector Plug-In log file. For Example: `general.logFile=c:\\Google\\logs\\!PlugIn.log` | C:\\Google\\logs\\Google Calendar Connector Plug-In.log |
| general.verboseLogging | Setting this value to "true" enables verbose output in the general.logfile for debugging purposes. For Example: `general.verboseLogging=true` | false |
| general.logMessages | Setting this value to "true" logs all incoming Groupwse API requests and the responses generated by the Google Calendar Connector PlugIn. For Example: `general.logMessages=true` _**NOTE: These log files are written to the WPCSIN folder defined by general.basedDirectory**_. | false |
| general.httpProxy | This setting enables HTTP proxy support. The setting defines the proxy server address and port that all web requests will be forwarded through. For Example: `general.httpProxy=http-proxy.acme.com:8080` | DISABLED |
| general.httpsProxy | This setting enables HTTPS proxy support. The setting defines the proxy server address and port that all web requests will be forwarded through. For Example: `general.httpsProxy=https-proxy.acme.com:8181` | DISABLED |
| gdata.username | Admin user name used to login to the GData service for the Google Apps domain. For Example: `gdata.username=gccplugin@acme.com` _**NOTE: This user account must be granted administrator privileges within your Google Apps domain. The provisioning API must be enabled for the Google Apps domain as well**_. |  |
| gdata.password | The password for the gdata.username login to the Google Apps domain. For Example: `gdata.password=adminpassword01` |  |
| gdata.domain | The Google Apps domain name. For Example: `gdata.domain=acme.com` |  |
| fbfix.command | The executable filename of a helper process for the Google Calendar Connector plug-in. This tool verifies that Google Apps users are correctly registered in the Exchange free/busy store so free/busy lookups are routed through the Groupwse/Google Calender Connector PlugIn. For Example: `fbfix.command=freebusybuilder.exe` | freebusybuilder.exe |
| fbfix.timeout | Specifies the number of seconds before the fbfix.command is killed. For Example: `fbfix.timeout=60` | 60 |
| fbfix.frequency | Specifies the interval in seconds at which the Google Calendar Connector Plug-in will execute the fbfix.command. For Example: `fbfix.frequency=1080` | 1080 |
| win.certname | This setting defines the name of the certificate in the user's Personal certificate store to be used for encryption of the passwords in the config.txt  This setting is typically the "Issued To" value or CN of the user who requested the certificate. For Example: `win.certname=pluginsvc` | DISABLED |

### Define LDAP filters Optional ###

An optional step in setting up the Google Calendar Connector Plug-In is specifying a whitelist or blacklist LDAP query in config.txt. This is useful if your organization has users who exist in both Active Directory and Google Apps. This feature allows you to control what accounts are imported into Exchange to support Exchange lookup of a Google Apps user's free/busy data.

This LDAP query is either a:

  * **whitelist**: includes a specific subset of Active Directory users. A contact will be created for Google Apps accounts that match for these users.

  * **blacklist**: specifically excludes a subset of Active Directory users. No contact will be created for Google Apps accounts that match for these users.

This is an important feature for environments with users who exist in both Active Directory and Google Apps, but who are to be excluded from the Google Calendar Connector Plug-In.

  * 5.2.1. Open config.txt and enter the LDAP server url ldap.url, username ldap.user and password ldap.password. These are the login credentials used to query your company's LDAP server for a list of user names.
  * 5.2.2. Set ldap.authMethod=simple. This is equivalent to "unencrypted', but can still be used with LDAPS, if you can't support DIGEST-MD5
  * 5.2.3. Set ldap.base to the base LDAP query for your domain. This is useful for generally specifying your users' domain and organizational information.
  * 5.2.4. Uncomment either ldap.whitelist or ldap.blacklist and ldap.domainMap then enter your custom LDAP filter. This filter specifies which users to explicitly include or exclude from the Google Calendar Connector Plug-In service, respectively. An example white-list is displayed below. You must also configure the ldap.domainMap feature as well to map internal SMTP addresses returned from the whitelist or blacklist query to the SMTP addresses as defined in Google Apps.
> _**NOTE: If neither ldap.whitelist or ldap.blacklist is uncommented, no LDAP filter will be applied. If no ldap.domainMap is configured then SMTP address matching will not take effect and the whitelist or blacklist filter is not applied**_.
  * 5.2.5. Verify ldap.blindFaith is set to "true". This enables the Google Calendar Connector Plug-In to accept any TLS certificate when connecting to a secure LDAP server regardless of whether the certificate is trusted by a Certificate Authority listed in the machine's Trust Root Certificates certificate store.
  * 5.2.6. Verify your LDAP configuration against the following checklist:

| Configuration Key | Description | Default Value |
|:------------------|:------------|:--------------|
| ldap.url | This setting specifies the Active Directory LDAP server to connect to. For Example: `ldap.url=LDAPS://ldap1.acme.com` |  |
| ldap.user | The userPrincipalName for the user account used by the plugin to perform Active Directory queries for ldap.whitelist and ldap.blacklist values. |  |
| ldap.password | The password for the ldap.user account |  |
|  ldap.authMethod | This setting defines the authentication method used to access LDAP; supported modes are "simple"and "DIGEST-MD5". Default method is "DIGEST-MD5" if none is specified. **_NOTE: To use DIGEST-MD5, the configured windows user account in Active Directory must have enabled "Store password using reversible encryption" set via Active Directory Users and Computers. Once this value is enabled the password must be changed in order to store the password in the reversible encryption format._| simple**|
| ldap.base | The search base DN for the ldap.blacklist and ldap.whitelist queries. |  |
|ldap.blacklist | This setting defines an LDAP filter of users to exclude from the Google Apps directory sync import. For Example: `ldap.blacklist=(&(objectClass=user)(msExchHomeServerName=*))` This filter returns a list of all user objects that have an Exchange mailbox. |  |
| ldap.whitelist | This setting defines an LDAP filter of users to include in the Google Apps directory sync import. For Example: `ldap.whitelist=(mail=*@exchange.acme)(objectcategory=person)` This filter returns a list of all user and contact objects that have an internal mail address of @exchange.acme.com. This includes mailbox enabled users, mail enabled users  and contacts. |  |
| ldap.blindfaith | Enables the Google Calendar Connector Plug-In to accept any TLS certificate when connecting to a secure LDAP server regardless of whether the certificate is trusted by a Certificate Authority listed in the machine's Trust Root Certificates certificate store. | true |
| ldap.domainMap | This setting allows the definition of SMTP domain name mappings. This value is used in conjunction with ldap.blacklist and/or ldap.whitelist. The mapping is applied to the LDAP black/white list response and is used to match against the Google Apps user SMTP address returned from the GData API. This setting maps an internal SMTP address from Exchange to the Google Apps SMTP address. For Example: `ldap.domainMap=exchange.acme.com,acme.com` An Exchange user has the primary SMTP address of user@exchange.acme.com would match with the Google Apps user which has the SMTP address user@acme.com. To configure multiple domain mappings use ";" as the delimiter between the domain mapping values. For Example: `ldap.domainMap=acme.com,exchange.acme.com;company.com,exchange.company.com` |  |

### Mail routing for Google Apps contacts ###

In order for mail to be correctly routed to Google Apps users, we tell the directory sync to write the Google Apps users' SMTP mail addresses as their contact objects' target email addresses. By default, the Novell Groupwse connector will have mapped the namespaces in a safe but incompatible format for Google Apps. We will re-configure the default mapping from Groupwse to Active Directory so that the Google users' Active Directory targetAddress attributes point to their Google Apps email addresses.

This allows the Google Apps users' free/busy information to be made available to Exchange users via the Novell Groupwse connector, and their mail to be delivered to their Google Apps accounts via the standard SMTP routing from Exchange.

This requires a two-step process:
Properly define the mapping of Google Apps users mail attributes into variables that the Novell Groupwse connector can understand.
Map the Google Apps email address variable to Active Directory as an SMTP e-mail address.

_**NOTE: Although the specific values described below may differ across environments, the end goal of this step is always to set the targetAddress Active Directory attribute to SMTP:user@acme.com**_.

  * 5.3.1. Open C:\Program Files\Exchsrvr\conndata\dxagwise\mexamap.tbl.
  * 5.3.2. Verify a line of the form:
> TA {some number like 64, 256} targetAddress exists. Do not make any changes to this line.

> _**NOTE: Do not use TAB for spacing. Only use spaces for whitespace**_

  * 5.3.3. Open C:\Program Files\Exchsrvr\conndata\dxagwise\mapgwise.tbl.
  * 5.3.4. Modify the line TA = "GWISE:" to read TA = "SMTP:" OBJECT
  * 5.3.5. Save changes and close mapgwise.tbl.

> _**NOTE: If you are concerned, for other reasons, about overwriting an existing mapping by changing the value of the variable TA, you can use the alternative method below:**_

> _**NOTE: An alternative method to the one described above is to add the following new line to mapgwise.tbl**_:

> Set `GAPPSALIAS = "SMTP:" OBJECT`
> Then to modify the targetAdress mapping in mexamap.tbl to `GAPPSALIAS 256 targetAddress`

### Install PlugIn as a service ###

  * 5.4.1. Open a command line in the API Gateway share directory C:\Program Files\Google\Google Calendar Connector Plug-In\bin.
  * 5.4.2. Execute install.bat This verifies that the necessary files exist, runs a self-test and installs the Google Calendar Connector Plug-In as a Windows service on your machine.
  * 5.4.3. Open the Windows Services management console.
  * 5.4.4. Find the "Google Calendar Connector Plug-In" service and right-click for properties.
  * 5.4.5. Click "Start" to start the Google service.

### Test configuration with Exchange gwrouter ###

This is an optional step that can be used to verify the correct functioning of the services activated to this point. Following this step will allow Exchange to verify that it can reach out to the Novell Groupwse API Gateway folders that the Google service created.

  * 5.5.1. To test and make sure the service was installed correctly, run C:\Program Files\Exchsrvr\bin\gwrouter.exe.

  * 5.5.2. If the service is working correctly, you should see output similar to that below:
```
  Service Running
  [00000D48]: The Microsoft Exchange Router for Novell Groupwse v6.5 (Build 6944.0) has started
  [00000688]: API_IN directory is \\localhost\GCCPLUGIN\API_IN\.
  [00000688]: API_OUT directory is \\localhost\GCCPLUGIN\API_OUT\.
  [00000688]: ATT_IN directory is \\localhost\GCCPLUGIN\ATT_IN\.
  [00000688]: ATT_OUT directory is \\localhost\GCCPLUGIN\ATT_OUT\.
  [00000688]: Free Busy directory is C:\Program Files\Exchsrvr\conndata\gwrouter\freebusy\.
  [00000688]: Dir Sync directory is C:\Program Files\Exchsrvr\conndata\gwrouter\dirsync\.
  [00000688]: Inbound To Router directory is C:\Program Files\Exchsrvr\conndata\gwrouter\togwise\.
  [00000688]: Bad Files directory is C:\Program Files\Exchsrvr\conndata\gwrouter\badfiles\.
  [00000688]: GW2MEX directory is C:\Program Files\Exchsrvr\conndata\gwrouter\GW2MEX\.
  [00000688]: GW2MEXA directory is C:\Program Files\Exchsrvr\conndata\gwrouter\GW2MEXA\.
  [00000688]: MEX2GW directory is C:\Program Files\Exchsrvr\conndata\gwrouter\MEX2GW\.
  [00000688]: MEX2GWA directory is C:\Program Files\Exchsrvr\conndata\gwrouter\MEX2GWA\.
  [00000688]: Groupwse Share Name is \\localhost\GCCPLUGIN.
  [00000688]: Groupwse UserID is gccplugin.
  [00000688]: Groupwse User Password has been retrieved.
  [00000688]: Logging on NetWare server with remote location '\\localhost\GCCPLUGIN', userid 'gccplugin'
  [00000688]: Novell NetWare server is connected
  [00000688] (Debug): Checking mail from Groupwse API Gateway output queue...
  [00000688] (Debug): Checking mail from Mex2Gw Router queue...
  [00000688] (Debug): Checking mail from Inbound To Router queue...
  [00000688] (Debug): Checking mail from Groupwse API Gateway output queue...
  [00000688] (Debug): Checking mail from Mex2Gw Router queue...
  [00000688] (Debug): Checking mail from Inbound To Router queue...
  [00000688] (Debug): Checking mail from Groupwse API Gateway output queue...
  [00000688] (Debug): Checking mail from Mex2Gw Router queue...
  [00000688] (Debug): Checking mail from Inbound To Router queue...
  [00000688] (Debug): Checking mail from Groupwse API Gateway output queue...
  [00000688] (Debug): Checking mail from Mex2Gw Router queue...
  [00000688] (Debug): Checking mail from Inbound To Router queue...
```

  * 5.5.3 Once you are satisfied the services are activated correctly, close the command line window.

### Start Connector for Novell GroupWise ###

After installing all of the connectors, they need to be activated. First, we activate the Microsoft Exchange Connector for Novell Groupwse.

  * 5.6.1. Open the Windows Services Management console.
  * 5.6.2. Select the "Microsoft Exchange connector for Novell Groupwse" and right-click for properties.
  * 5.6.3. Click "Start" if the service is not already started.

### Start Router for Novell GroupWise ###

Next, activate the Microsoft Exchange Router for Novell Groupwse.

  * 5.7.1. Open the Windows Services Management console.
  * 5.7.2. Select the "Microsoft Exchange Router for Novell Groupwse" and right-click it for properties.
  * 5.7.3. Click "Start" if the service is not already started

### Force an initial synchronization ###

Now that the basic connectors are connected and routing, we initiate an immediate full reload for directory synchronization. This will populate the GoogleApps Organizational Unit in Active Directory, which Microsoft Exchange uses. _**NOTE that the Calendar Connector has not yet been started**_.

  * 5.8.1. Navigate back to the Exchange System Manager.
  * 5.8.2. Select the "Connector for Novell Groupwse" and right-click to display its properties.
  * 5.8.3. Open the "Dirsync Schedule" tab.
  * 5.8.4. In the "Groupwse to Exchange" directory synchronization box, click "Immediate full reload".
  * 5.8.5. Acknowledge that the process has started by clicking "OK".
  * 5.8.6. After the directory synchronization has completed, contacts from Google Apps should have been populated in the GoogleApps Active Directory Organizational Unit.

### Start the Microsoft Calendar Connector service ###

Finally, we enable the free/busy lookup service.

  * 5.9.1. Open the Windows Services management console.
  * 5.9.2. Find the "Microsoft Exchange Calendar Connector" service.
  * 5.9.3. Start the service by right-clicking and selecting "Start".

## 6 Installation Success ##

All necessary services should now be correctly installed and configured for Exchange users to see free/busy information for Google Apps users. This section is intended to verify the correct configuration and functioning of your deployment.

  * 6.1 [Execute Selftest](#Execute_Selftest.md)
  * 6.2 [Review the output of Selftest](#Review_the_output_of_Selftest.md)
  * 6.3 [Verify Calendar Look up](#Verify_Calendar_Look_up.md)

First, we will independently run Selftest.exe, an automated testing script that verifies:

  * Java 6 Update 2 or later is installed correctly
  * The API Gateway Share Path is accessible
  * Your Google Apps domain is accessible via GData
  * The Provisioning API is accessible for your Google Apps domain
  * If defined, the LDAP query and filter complete successfully.
  * Config.txt contains the correct configuration information to complete the above tests successfully.

_**NOTE: !Selftest.exe will not be able to confirm that you have configured Microsoft Exchange and the Groupwse connector correctly**_.

Next, we will use Microsoft Outlook to complete an end-to-end verification test by creating a new appointment and inviting Google Apps users to attend. Along the way, we will view their Google Apps free/busy information from within Microsoft Outlook.

### Execute Selftest ###

  * 6.1.1 Open a command line and navigate to the C:\Program Files\Google\Google Calendar Connector Plug-In\bin directory with Selftest.exe
  * 6.1.2 Execute Selftest.exe from within the command line.

> ## Review the output of Selftest ##

If you see the final output line: "All tests passed you're good to go :-)", !Selftest.exe has completed successfully. A sample execution of !Selftest.exe has been included below for comparison.
```
  C:\Program Files\Google\Google Calendar Connector Plug-in\bin>Selftest.exe
  ResourceManager resource configuration:
  - Resource category: JAVA
  - Current directory: C:\Program Files\Google\Google Calendar Connector Plug-in\bin\
  - Property count: 16
  - Property:  =<>
  - Property: arguments=<>
  - Property: currentdir=<${EXECUTABLEPATH}>
  - Property: embedjar=<true>
  - Property: initialheap=<67108864>
  - Property: javapropertiescount=<1>
  - Property: javaproperty_name_0=<config>
  - Property: javaproperty_value_0=<config.txt>
  - Property: jvmsearch=<registry;javahome;jrepath;jdkpath;exepath;jview>
  - Property: mainclassname=<com.google.calendar.interoperability.connectorplugin.Selftest>
  - Property: maxheap=<134217728>
  - Property: maxversion=<>
  - Property: minversion=<1.6>
  - Property: skel_Debug=<1>
  - Property: skel_Message=<This program needs Java to run. Please download it at
  http://www.java.com/en/download>
  - Property: skel_PressKey=<0>
  Now searching the JVM installed on the system...
  JVM Lookup: found VM (V(1)(6)(0)) in registry.
  JVM Lookup: found VM (V(1)(6)(0)) in registry.
  JVM Lookup: found VM (V(1)(6)(0)) in registry.
  JVM Lookup: found VM (V(1)(6)(0)) in registry.
  JVM Lookup: Env-Var JAVA_HOME not defined on this system.
  JVM Lookup: Env-Var JRE_HOME not defined on this system.
  JVM Lookup: Env-Var JDK_HOME not defined on this system.
  Current directory is C:\Program Files\Google\Google Calendar Connector Plug-in\bin\
  JSmooth will now try to use the VM in the following order: registry;javahome;jrepath;jdkpath;exepath;jview
  ------------------------------
  Trying to use a JVM defined in the registry (4 available) VM will be tried in the following order: 1.6.0;1.6.0;1.6.0;1.6.0;  
  - Trying registry: <C:\Program Files\Java\jre1.6.0_05><C:\Program Files\Java\jre1.6.0_05\bin\client\jvm.dll><1.6.0>
  Created temporary filename to hold the jar (C:\DOCUME~1\ADMINI~1.1BO\LOCALS~1\Temp\2\temp3.jar)
  NO JNI SMOOTH ID !!
  COMMAND: <"C:\Program Files\Java\jre1.6.0_05\bin\java.exe" -Xmx134217728 -Xms67108864 "-Dconfig"="config.txt" -classpath "C:\DOCUME~1\ADMINI~1.1BO\LOCALS~1\Temp\2\temp3.jar;" com.google.calendar.interoperability.connectorplugin.Selftest >
  Started successfully
  Loading configuration...
  ... Done
  Test base configuration...
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setProxy
  INFO: httpProxy not set, will not use proxy
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setProxy
  INFO: httpsProxy not set, will not use proxy
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main  setUp
  INFO: base directory is C:\Program Files\Google\Google Calendar Connector Plug-In
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setUp
  INFO: Building scanner...
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setUp
  INFO: Building stage 1...
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setUp
  INFO: Building stage 2...
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setUp
  INFO: Building stage 3...
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.Main setUp
  INFO: Building stage 4...
  ... Done
  Evaluating base directory...
  (Make sure that the Exchange side also uses C:\Program Files\Google\Google Calendar Connector Plug-In)
  ... Done
  Evaluating Connectivity to GData
  Performing a test user query for domain 1bot.info
  May 7, 2008 2:52:38 PM com.google.calendar.interoperability.connectorplugin.impl.google.GDataAccessObject retrieveAllUsers
  INFO: Retrieving all users.
  ... Done
  Evaluating LDAP filtering
  LDAP filter not used
  ... Done
  All tests passed -- you're good to go :-)
```

## Verify Calendar Look up ##

  * 6.3.1 Create a new appointment in Microsoft Outlook
  * 6.3.2 Invite a Google Apps user
  * 6.3.3 Move to the Scheduling tab
  * 6.3.4 Click "Yes" on the security warning
  * 6.3.5 See free/busy Information for Google Apps!

## 7 Enable Automatic Startup for Services ##

Once the Verify Calendar Look up test has been completed, you are ready to enable the calendar services to startup automatically upon reboot. The following services should be set to "Startup Type" "Automatic" in the Windows Services Management Console:

  * Google Calendar Connector Plug-In connector service
  * Microsoft Exchange Calendar Connector
  * Microsoft Exchange Connector for Novell Groupwse

## Enable Services ##

  * 7.1.1. Open the Windows Services Management Console.
  * 7.1.2. Highlight the service you wish to enable to start up automatically.
  * 7.1.3. Right-click on the service and select "properties".
  * 7.1.4. Open the "Startup Type" drop-down menu and select "Automatic". Click the "OK" button.

## 8 Config Password Encryption ##

The config.txt contains passwords in plain text. To secure these passwords obfuscation or encryption is used so they are not saved is clear text. If win.certname is commented out obfuscation is used by default. The first time the service is started the passwords will be replaced with for example: `_LDP_=-OB-fxtmpjZcmBgZHxhdyMh`. The value _LDP_ indicates the ldap.password password and _GDP_ indicates the gdata.password. The "-OB-" after the equals denotes obfuscation was used and then is followed by the mask password. If certificate based encryption is used the value "-MS-" is used and is followed by the encrypted password.

Encryption is accomplished by using a user account enrolled with a user certificate. To accomplish this an Active Directory user account will need to be setup and enrolled with a certificate. The steps below detail how to accomplish this using a Microsoft Certificate Authority. The Google Calendar Connector Plug-in service is then configured to run under the security context of this user, which gives it access to the user's certificate private key which is used to encrypt the passwords in the config.txt file.

  * 8.1 [Enable Encryption for Config Passwords](#Enable_Encryption_for_Config_Passwords.md)
  * 8.2 [Enabled Certificate Enrollment](#Enabled_Certificate_Enrollment.md)
  * 8.3 [Grant Administrative Rights](#Grant_Administrative_Rights.md)
  * 8.4 [Enroll in Certificate](#Enroll_in_Certificate.md)
  * 8.5 [Configure Service to Run As User Account and Encrypt Passwords](#Configure_Service_to_Run_As_User_Account_and_Encrypt_Passwords.md)

### Enable Encryption for Config Passwords ###

  * 8.1.1 Create an Active Directory user account which will run the Google Calendar Connector Plug-in service.

### Enabled Certificate Enrollment ###

  * 8.2.1 Run the Certificates Templates MMC with an administrative account that can modify certificate templates for the Certificate Authority.
  * 8.2.2 Locate the "Users" certificate template and select Properties
  * 8.2.3 Select the Security Tab and add the user account.
  * 8.2.4 Grant the user or group "enroll" privileges

### Grant Administrative Rights ###

  * 8.3.1 Login as an Administrator to the server where the plug-in is installed
  * 8.3.2 Run "compmgmt.msc"
  * 8.3.3 Expand Users and Groups
  * 8.3.4 Click on Groups
  * 8.3.5 Double Click on the Administrators group
  * 8.3.6 Click Add, type the name of the user account and press enter
  * 8.3.7 Click OK

### Enroll in Certificate ###

  * 8.4.1 Login to the server as the user account that was granted Administrative rights.
  * 8.4.2 Run "mmc.exe"
  * 8.4.3 Click File | Add/Remove Snap-In
  * 8.4.4 Click Add and select the "Certificates" snap-in then click ADD
  * 8.4.5 Select "My user account and click Finish
  * 8.4.6 Click Close and Click OK
  * 8.4.7 Expand Certificates and right click on Personal
  * 8.4.8 Select All Tasks | Request New Certificate
  * 8.4.9 Click Next on the "Certificate Request Wizard"
  * 8.4.10 Select the template "User" from the list
  * 8.4.11 Click Next and Finish
  * 8.4.12 At this point the certificate request should have completed and you should have a user certificate in the Personal store for the user.
  * 8.4.13 Note the name of the "Issued To" value which is typically the CN of the user account.
  * 8.4.14 Edit the config.txt in the bin folder in the path defined by general.baseDirectory
  * 8.4.15 Configure the option win.certname= to the value you obtained in step 8.4.13

### Configure Service to Run As User Account and Encrypt Passwords ###

  * 8.5.1 Login to the server where the plug-in is installed
  * 8.5.2 Run "services.msc"
  * 8.5.3 Select "Google Calendar Connector Plug-in" service and right click and select Properties
  * 8.5.4 Click on the Log On Tab
  * 8.5.5 Select the option "This account" to define the account to run the service as.
  * 8.5.6 Enter the Active Directory user account name or click browse to locate it.
  * 8.5.7 Enter and confirm the password for the user account
  * 8.6.8 Click Apply
  * 8.6.9 Select "Google Calendar Connector Plug-in" service and right click and select Restart
  * 8.6.10 Review the config.txt in the bin folder in the path defined by general.baseDirectory option to verify the passwords are now displayed as encrypted lines and contain =-MS- to signify password encryption, not obfuscation.

## 9 Troubleshooting ##
  * Run !Selftest.exe to verify the Google Calendar Connector Plug-In settings are working.
  * Run FreeBusyBuilder.exe from the command line and verify it is seeing the Google Apps contacts properly and updating the free/busy store.
> _**NOTE: If you are using password encryption you must run the Selftest.exe as the user account which contains the "User" certificate**_.

  * Calendar lookups are not working

> Ensure the windows services are running:
    * Google Calendar Connector Plug-In
    * Microsoft Exchange Calendar Connector
    * Microsoft Exchange Connectivity Controller
    * Microsoft Exchange Connector for Novell Groupwse
    * Microsoft Exchange Router for Novell Groupwse

  * Enabled general.logMessage, general.logFile and general.verboseLogging.
    * Enabling general.logMessage this will have the Google Calendar Connector Plug-In write each API message and its response into "C:\Program Files\Google\Google Calendar Connector Plug-In\WPCSIN". Run a directory sync and a free/busy request from Outlook for a Google Apps contact. Verify the API requests are being processed.
    * Enabling general.logFile and general.verboseLogging will get you a very verbose log will indicate where the Google Calendar Connector Plug-in is failing.

  * Some users in my Google Apps Domain are not showing up in Active Directory
    * Verify there is not a user or contact in Active Directory with the e-mail address of the Google Apps user. No two AD objects can have the same e-mail address.
    * Enable general.logMessage and review the API response file for the directory syncs to determine if the user is in the response being processed by Exchange.

## 10 Copyright Notices ##

Library and license attributions are provided to conform with the Apache License, Version 2.0. A copy of the Apache License, Version 2.0 can be found [here](http://www.apache.org/licenses/LICENSE-2.0.html).

The following licenses and libraries are used in the Google Calendar Connector Plug-In:

  * Google Data (GData) Java Client Library and its dependencies licensed under the **Apache License, Version 2.0**. ([project](http://code.google.com/p/gdata-java-client/) , [http://www.apache.org/licenses/LICENSE-2.0 license)
  * Google Collections Library and its dependencies licensed under the **Apache License, Version 2.0** ([project](http://code.google.com/p/google-collections/), [license](http://www.apache.org/licenses/LICENSE-2.0))

The following licenses and libraries are used to build the Google Calendar Connector Plug-In:

  * Apache Ant Project and its dependencies licensed under the **Apache License, Version 2.0**. ([project](http://ant.apache.org/), [license](http://ant.apache.org/license.html))

The following licenses and libraries are used for testing the Google Calendar Connector Plug-In:

  * JUnit and its dependencies ([project](http://www.junit.org/))
  * JMock and its dependencies ([project](http://jmock.codehaus.org/), [license](http://jmock.codehaus.org/license.html))

The Google Calendar Connector Plug-In optionally supports, but does not require, building Windows executables using JSmooth.  JSmooth is not distributed with the Google Calendar Connector Plug-In open source release and its use represents an optional step in the build process.  JSmooth may be obtained and used in accordance with its license, both of which are provided below (for the sake of convenience):

JSmooth project: http://jsmooth.sourceforge.net
JSmooth license: http://jsmooth.sourceforge.net/license.php


---

_Google, Google Calendar, Google Calendar Connector, Google Calendar Connector Plug-In, Google Calendar Connector Sync Service, Google Calendar Connector Web Service are trademarks of Google, Inc_.

_All other company and product names may be trademarks of the respective companies with which they are associated_.