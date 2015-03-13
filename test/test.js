var assert = require('assert'),
    request = require('supertest'),
    cheerio = require('cheerio'),
    app = 'http://localhost:8080'

describe('Launcher App', function(){
    it('Should respond with html when Accept="text/html"', function(done){
        request(app)
        .get('/')
        .set('Accept', 'text/html')
        .expect('Content-Type', /html/)
        .expect(200, done);
    })
    it('<title> should be "Launcher"', function(done){
    request(app)
        .get('/')
        .set('Accept', 'text/html')
        .expect('Content-Type', /html/)
        .expect(200)
        .expect(function(res) {
            var $ = cheerio.load(res.text);
            assert.equal('Launcher', $('title').html());
        })
        .end(function(err, res){
            if (err) return done(err);
            done()
        });
    })
    it('Should respond with json when Accept="application/json"', function(done){
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .expect('Content-Type', /json/)
        .expect(200, done);
    })
    it('Json respond should contain Launcher.results"', function(done){
        request(app)
        .get('/')
        .set('Accept', 'application/json')
        .expect('Content-Type', /json/)
        .expect(200)
        .expect(function(res) {
            var json = JSON.parse(res.text)
            assert(json.results);
        })
        .end(function(err, res){
            if (err) return done(err);
            done()
        });
    })
})