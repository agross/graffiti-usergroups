Two plugins, macros and a theme for user groups to manage their events and talks.

# Contents #
  * A sample theme
  * Event plugin to manage user group events including metadata like
    * Speaker
    * Location
    * Begin and end date and time
    * Allowed number of registrations
  * Talk plugin to manage talk downloads and slides
  * Two sets of Chalk macros for accessing metadata conveniently

# Installation #
To try Graffiti-Usergroups just copy the release files to a fresh Graffiti CMS installation. Then log in to the admin control panel and select the Graffiti-UserGroups theme. Go back to the home page and run the installation wizard. The wizard will configure the plugins and create a couple of sample events, talks and a registration page for your visitors.

[![](http://www.therightstuff.de/content/binary/WindowsLiveWriter/Graffiti-UserGroups/Video.png)](http://www.therightstuff.de/download/Graffiti%2DUserGroups/)

**[Watch an introductory video here!](http://www.therightstuff.de/download/Graffiti%2DUserGroups/)**

# A note on the theme that comes with Graffiti-UserGroups #
The theme included in the download is there to make the initial setup of Graffiti-UserGroups a no-brainer for you. It also provides a reference implementation to show you how to use the Chalk macros included in Graffiti-UserGroups. The theme also includes the ASP.NET handlers (inside <Theme directory>\Handlers) for the registration page and the calendar item download.

**You may very well use Graffiti-UserGroups with your own theme.**

# Requirements #
  * [Graffiti CMS](http://graffiticms.com)
  * .NET Framework 3.5
  * A full trust environment due to a [bug](http://support.graffiticms.com/t/616.aspx) in Graffiti CMS 1.0 and 1.1, we need reflection to work around it

Tested with SQL Server 2005, but should work with any other database supported by Graffiti CMS as the plugins do not touch the database directly.


---


Written by [Alexander Groß](http://therightstuff.de/).