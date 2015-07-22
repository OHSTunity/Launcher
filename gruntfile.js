module.exports = function(grunt) {

  grunt.initConfig({
    watch: {
      options: {
        nospawn: true,
        livereload: true
      },

      livereload: {
        files: [
          'wwwroot/*.{html,css}',
          'wwwroot/**/*.html',
          'wwwroot/**/*.css'
        ]
      }
    },
    replace: {
      dist: {
        options: {
          patterns: [
            {
              match: /<VersionDate>.*(?=<\/VersionDate>)/,
              replacement: '<VersionDate>'+grunt.template.today('isoDateTime')+"Z"
            }
          ]
        },
        files: [
          {src: ['src/Launcher/package/package.config'], dest: './'}
        ]
      }
    },
    bump: {
      options: {
        files: ['package.json', 'src/Launcher/package/package.config'],
        commit: true,
        commitMessage: '%VERSION%',
        commitFiles: ['package.json', 'src/Launcher/package/package.config'],
        createTag: true,
        tagName: '%VERSION%',
        tagMessage: 'Version %VERSION%',
        push: false,
        // pushTo: 'origin',
        globalReplace: false,
        prereleaseName: "pre",
        regExp: new RegExp(
          '([\'|\"]?version[\'|\"]?[ ]*:[ ]*[\'|\"]?|<Version>)(\\d+\\.\\d+\\.\\d+(-' +
          "pre" + // opts.prereleaseName +
          '\\.\\d+)?(-\\d+)?)[\\d||A-a|.|-]*([\'|\"]?)', 'i'
        )
      }
    }
  });

  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-replace');
  grunt.loadNpmTasks('grunt-bump');

};
