var assert = require('assert'),
    request = require('supertest'),
    cheerio = require('cheerio'),
    app = 'http://localhost:8080';

describe('Launcher App', function () {
    it('Should respond with html when Accept="text/html"', function (done) {
        request(app)
        .get('/')
        .set('Accept', 'text/html')
        .expect('Content-Type', /html/)
        .expect(200, done);
    });
    it('<title> should be "Launcher"', function (done) {
        request(app)
            .get('/')
            .set('Accept', 'text/html')
            .expect('Content-Type', /html/)
            .expect(200)
            .expect(function (res) {
                var $ = cheerio.load(res.text);
                assert.equal('Launcher', $('title').html());
            })
            .end(function (err, res) {
                if (err) return done(err);
                done();
            });
    });
    it('Should respond with json when Accept="application/json"', function (done) {
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .expect('Content-Type', /json/)
        .expect(200, done);
    });
    it('Json respond should contain Launcher.results"', function (done) {
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .expect('Content-Type', /json/)
        .expect(200)
        .expect(function (res) {
            var json = JSON.parse(res.text);
            assert(json.results);
        })
        .end(function (err, res) {
            if (err) return done(err);
            done();
        });
    });

    //Requires SignIn app running
    it("Application name should be case insensitive", function (done) {
        var test = function (url, referer, callback) {
            request(app)
            .get(url)
            .set('Accept', 'application/json-patch+json')
            .set("X-Referer", referer)
            .expect('Content-Type', "application/json-patch+json")
            .expect(200)
            .expect(function (res) {
                var json = JSON.parse(res.text.replace(/,,/gi, ","));
                var patch = json[json.length - 1];
                var reg = new RegExp("^/workspaces/0", "gi");

                assert(reg.test(patch.path), "Application name should be case insensitive!");
            })
            .end(function (err, res) {
                if (err) {
                    return done(err);
                }

                callback();
            });
        };

        request(app)
        .get('/signin/signinuser')
        .set('Accept', 'application/json-patch+json')
        .expect('Content-Type', 'application/json-patch+json')
        .expect(200)
        .end(function (err, res) {
            var cookies = res.header["set-cookie"];
            var referer;

            for (var i = 0; i < cookies.length; i++) {
                if (/^Location/gi.test(cookies[i])) {
                    referer = cookies[i].replace(/^Location=/gi, "").replace(new RegExp("; path=/$"), "").replace(/%2F/gi, "/");
                    break;
                }
            }

            test("/signin/signin/username/password", referer, function () {
                test("/signIn/signin/username/password", referer, function () {
                    done();
                });
            });
        });
    });
});