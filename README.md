NetMetrix SDK
=============

Net-Metrix SDK for Windows Phone applications to simplify reporting app activity to [NET-Metrix](http://www.net-metrix.ch), a Swiss web and mobile usage monitoring agency.

## Install

Using [NuGet](https://www.nuget.org/packages/NetMetrixSdk/) package manager:

	PM> Install-Package NetMetrixSdk

This adds also the file ``Properties\NetMetrix.xml`` with configuration values. 
Make sure you assign your offer ID (Angebotskennung) and app ID.

## How to use

### Automatic tracking

Once configured, the easiest way to use is to enable the automatic tracking, that reports one view every time there is a ``Navigated`` event on the root frame.

One good place to put this initialization is on your ``Application`` initialization code:

    private void Application_Launching(object sender, LaunchingEventArgs e)
    {
      NetMetrix.EnableNavigationTracker(RootFrame);
    }

### Manual tracking

To manually track events:

    NetMetrix.Tracker.Track("section_name");

The section is optional, and defaults to  ``general``.

### Monitoring the reported events

NET-Metrix provides a sandbox to verify if the events are being reported correctly. The URL to access it is usually http://[OFFER-ID].wemfbox-test.ch/index.php

This library automatically reports to the sandbox if the debugger is attached.

# Contributing

If you feel like contributing to this project, go ahead and send us a pull request or an report issue! 

## Publish on nuget.org

    > nuget pack -sym -Build -Prop Configuration=Release NetMetrixSdk.csproj
    > nuget push NetMetrixSdk.x.x.x.x.nupkg



