Simple.MahApps.Template
=======================

[![Build status](https://ci.appveyor.com/api/projects/status/l0e5oai0xdn7o4cc/branch/master?svg=true)](https://ci.appveyor.com/project/oriches/simple-mahapps-template)

As with all my 'important' stuff it builds using the amazing [AppVeyor](https://ci.appveyor.com/project/oriches/simple-mahapps-template).

The app is skinned using [Mah Apps](http://mahapps.com/).

This is my example of templated WPF application using the MahApps control library to give a modern looking UI.

Okay this is *opinionated software* on the way to build a modern UI in WPF using MVVM as the MVC architecture and being 'pure' anout the separation between the View & the View Model, there are no UI concerns (colours, width's, control persistence) in the View Models. If don't agree in full separation between the View & the View Model then you might as well stop there :)

This test app makes use of node.js to create a mock backend server - it's not WCF :)

So if you're going to build this software make sure you have node.js installed and configured to build, I use VS2015 to build and test the node.js part, either by starting it from the command line 'node.js server.js' or by setting VS to start two projects at startup.

Happy coding...
