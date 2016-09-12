Launcher
========
A launcher is an app that gives a common UI feeling to multiple apps running simultaneously.

What it does specifically:

- create a new session
- initialize a Puppet connnection
- load a global stylesheet
- provide features to switch between apps
- includes [starcounter-debug-aid](https://github.com/StarcounterSamples/starcounter-debug-aid) for debugging.

This repository contains the code for the default launcher provided by Starcounter. Not impressed? Please fork it and create one that show us how it should be done!

To read more about launchers in general, please see the [Launcher](http://starcounter.io/guides/web/launcher) page over on **starcounter.io**.

### This development version works with Starcounter version: 2.2.1.3031

Past versions that work with Starcounter stable: [RELEASES.md](RELEASES.md)

### How to run

1. Check out StarcounterSamples/Launcher repo from GitHub
2. Open `Launcher.proj` in Visual Studio
3. Build the solution
4. Run it by launching `run.bat` in Windows, or executing `cmd //c "run.bat"` in Git Bash, or using Debug in Visual Studio
5. Go to [http://localhost:8080/](http://localhost:8080/)

This will bring you an empty Launcher (with Launchpad, Dashboard and Search field). You need to repeat the above steps to run the actual apps (available in [StarcounterSamples](https://github.com/StarcounterSamples) and [StarcounterPrefabs](https://github.com/StarcounterPrefabs) organisations). You can bulk multiple app projects into a single Visual Studio solution.

### How to test

1. Install [Node.js](https://nodejs.org/)
2. Run `npm install` to install the test framework
3. Start Launcher on [http://localhost:8080/](http://localhost:8080/)
4. Run `npm test`

### How to release a package

1. Install [Node.js](https://nodejs.org/).
2. Run `npm install` to install all dependencies.
2. Run `grunt package` to generate a packaged version. You can use `grunt package:minor`, `grunt package:major`, `grunt package --setversion=1.0.0-develop.0`, etc. as [grunt-bump](https://github.com/vojtajina/grunt-bump) does.
4. Publish `dist/<AppName>.zip` package to the App Store.
5. Run `git push && git push --tags` to push the changes made by `grunt package`.

## License

MIT
