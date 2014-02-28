module.exports = function(grunt) {

    grunt.initConfig({
        watch: {
            options: {
                nospawn: true,
                livereload: true
            },

            livereload: {
                files: [
                    'Client/*.{html,css}',
                    'Client/**/*.html',
                    'Client/**/*.css',
                    'Apps/*/Client/*.html',
                    'Apps/*/Client/*.css'
                ]
            }
        }
    });

    grunt.loadNpmTasks('grunt-contrib-watch');

};