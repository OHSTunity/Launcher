var myApp = new Framework7({
    preprocess: function (content, url, next) {
        if (url === 'people.html') {
            var template = Template7.compile(content);
            var resultContent = template({
                title: 'People',
                people: ['John', 'Ivan', 'Mary']
            })
            return resultContent;
        }
    }
});
