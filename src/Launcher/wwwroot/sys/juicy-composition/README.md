# &lt;juicy-composition&gt;

> Custom Element that adds given Document Fragment to Shadow DOM

## Demo

[Check it live!](http://Juicy.github.io/juicy-composition)

## Install

Install the component using [Bower](http://bower.io/):

```sh
$ bower install juicy-composition --save
```

Or [download as ZIP](https://github.com/Juicy/juicy-composition/archive/gh-pages.zip).

## Usage

1. Import polyfill, if needed:

    ```html
    <script src="bower_components/webcomponentsjs/webcomponents.min.js"></script>
    ```

2. Import custom element:

    ```html
    <link rel="import" href="bower_components/juicy-composition/juicy-composition.html">
    ```

3. Start using it!

    ```html
    <template id="my-shadow">
        <h1>Here goes</h1>
        <p>Any HTML structure to be put into `juicy-composition`s shadowRoot</p>
        <p>It may contain slots: <content select="[slot='my-slot']"></content></p>
    </template>
    ....
    <juicy-composition>
        <div>My thing</div>
    </juicy-composition>
    <script>
        var juicyComposition = document.querySelector('juicy-composition');
        juicyComposition.composition = document.importNode(document.querySelector('#my-shadow').content, true);
        juicyComposition.stamp();
    </script>
    ```

## Attributes

Attribute     | Options     | Default      | Description
---           | ---         | ---          | ---
`auto-stamp`  | *Boolean*   | `false`      | Set to make it stamp Shadow DOM on created and every setup change. It's also a `autoStamp` property.

## Properties

Attribute     | Options            | Default | Description
---           | ---                | ---     | ---
`composition` | *DocumentFragment* |         | Document Fragment to be used to in element's shadowRoot
`autoStamp`   | *Boolean*          | `false` | See [#Attributes]

## Methods

Method        | Parameters   | Returns     | Description
---           | ---          | ---         | ---
`stamp`       |              |             | Call it to imperatively stamp shadow DOM tree. If `auto-stamp` attribute is set, it's done automatically, when element is created, or composition is changed.

## Events

Event     | Description
---       | ---
`stamped` | Triggered once shadow DOM is stamped

## [Contributing and Development](CONTRIBUTING.md)

## History

For detailed changelog, check [Releases](https://github.com/Juicy/juicy-composition/releases).

## License

[MIT License](http://opensource.org/licenses/MIT)
