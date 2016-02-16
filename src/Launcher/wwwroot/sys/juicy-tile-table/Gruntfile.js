module.exports = function(grunt) {

    grunt.initConfig({
        watch: {
            all: {
                options: {
                    livereload: true
                },
                files: [
                    'juicy-tile-table.html',
                    '*.html',
                    'examples/**/*.html',
                    'test/*.*',
                    'test/**/*.html'
                ],
                // tasks: ['jshint'],
            },
        },
        // Mocha
        mocha: {
          all: {
            src: ['test/index.html'],
          },
          options: {
            run: true
          }
        },
        bump: {
          options: {
            files: ['package.json', 'bower.json', 'juicy-tile-table.html'],
            commit: true,
            commitMessage: '%VERSION%',
            commitFiles: ['package.json', 'bower.json', 'juicy-tile-table.html'],
            createTag: true,
            tagName: '%VERSION%',
            tagMessage: 'Version %VERSION%',
            push: false,
            // pushTo: 'origin',
            globalReplace: false,
            prereleaseName: false,
            regExp: false
          }
        }
    });

    // grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-mocha');
    grunt.loadNpmTasks('grunt-bump');


    grunt.registerTask('default', ['watch']);
    grunt.registerTask('test', ['mocha']);

};
