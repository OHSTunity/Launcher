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
        }
    });

    grunt.loadNpmTasks('grunt-contrib-watch');

};