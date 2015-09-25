:construction: Current dev version of Launcher (`master` branch) requires Starcounter 2.1+ and the [SysUpgrade](https://github.com/polyjuice/sysupgrade) app running. 
Last version that runs on Starcounter 2.0.3343.3 is [Launcher 2.0.8 (7f092e6)](https://github.com/Polyjuice/Launcher/tree/7f092e6981f4133d5048a8666b488269b4aa8023)

---

Launcher
========

Wrap all apps in a UI frame with a common launchpad, menu and a search bar.

### How to run

1. Check out Polyjuice/Launcher repo from GitHub
2. Open `Launcher.proj` in Visual Studio
3. Build the solution
4. Run it by launching `run.bat` in Windows, or executing `cmd //c "run.bat"` in Git Bash, or using Debug in Visual Studio
5. Go to [http://localhost:8080/](http://localhost:8080/)

This will bring you an empty Launcher (with Launchpad, Dashboard and Search field). You need to repeat the above steps to run the actual apps (available in [Polyjuice](https://github.com/Polyjuice) organisation). You can bulk multiple app projects into a single Visual Studio solution.

### How to test

1. Install [Node.js](https://nodejs.org/)
2. Run `npm install` to install the test framework
3. Start Launcher on [http://localhost:8080/](http://localhost:8080/)
4. Run `npm test`

### How to release a package

1. Install [Node.js](https://nodejs.org/)
2. Run `npm install` to install all dependencies
2. Run `grunt package` to generate a packaged version, (you can use `:minor`, `:major`, etc. as [grunt-bump](https://github.com/vojtajina/grunt-bump) does)
4. Publish `dist/<AppName>.zip` package to the App Store.

### Browsers

#### Chrome Stable & Dev Channel

Make sure the following flags are set (chrome://flags/):

 - Experimental JavaScript - **disabled**
 - Experimental Web Platform features - **disabled**
 - HTML Imports - **disabled** (note: this flag was merged into *Experimental Web Platform features* in recent versions of Chrome)

#### Chrome Canary

Make sure the following flags are set:

 - Experimental JavaScript - **enabled**
 - Experimental Web Platform features - **enabled**

 **Note:** the application has been migrated to Polymer 1.x.
- Latest Polymer 0.5 commit: https://github.com/Polyjuice/Launcher/commit/78f17c9a89419f1d19997da118cae249577b61b6
- Latest Polymer 0.5 release: https://github.com/Polyjuice/Launcher/releases/tag/2.0.8
