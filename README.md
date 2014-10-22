Launcher
========

Application Launcher

### How to run

1. Check out Polyjuice/Launcher repo from GitHub
2. Open `Launcher.proj` in Visual Studio
3. Set the subdirectory `Client` as the working directory for the project. This is the root of the static files accessed using the build-in the HTTP server.
4. Build the solution
5. Run it by launching `run.bat` in Windows, or executing `cmd //c "run.bat"` in Git Bash, or using Debug in Visual Studio
6. Go to [http://localhost:8080/](http://localhost:8080/)

This will bring you an empty Launcher (with Launchpad, Dashboard and Search field). You need to repeat the above steps to run the actual apps (available in [Polyjuice](https://github.com/Polyjuice) organisation). You can bulk multiple app projects into a single Visual Studio solution.

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
