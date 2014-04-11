Launcher
========

Application Launcher

### How to run

1. Check out Polyjuice/Launcher repo from GitHub
2. Set up the Launcher solution in your Visual Studio:
   - go to Solution properties > Startup Project
   - enable "Multiple startup projects"
   - set them in the order:
     - Launcher - Start
     - SuperCRM - Start
     - Skyper - Start
     - Map - Start
     - Store - None
     - Noise - Start
3. Set up the working directories for the projects:
     - Launcher - `Client\`
     - SuperCRM - `Apps\SuperCRM\Client\`
     - Skyper - `Apps\Skyper\Client\`
     - Map - `Apps\Map\Client\`
     - Store - `Apps\Store\Client\`
     - Noise - `Apps\Noise\Client\`
4. Build the solution and go to [http://localhost:8080/](http://localhost:8080/)

### Browsers

#### Chrome Stable & Dev Channel

Make sure the following flags are set:

 - Experimental JavaScript - **disabled**
 - Experimental Web Platform features - **disabled**
 - HTML Imports - **disabled** (note: this flag was merged into *Experimental Web Platform features* in recent versions of Chrome)

#### Chrome Canary

Make sure the following flags are set:

 - Experimental JavaScript - **enabled**
 - Experimental Web Platform features - **enabled**
