# &lt;juicy-tile-table&gt;

> Masonry-like Custom Element for sortable tiles that packs efficiently without changing HTML structure (changes CSS and ShadowDOM only), and adapts it to `<table>` that mimic CSS Grid Layout.
> So you will get (bin-packed) gap-less layout, with responsive, and adaptive features, handled natively by browser.
> Extended renderer for [`juicy-tile-list`](https://github.com/Juicy/juicy-tile-list).
> Flag-less fallback for  [`juicy-tile-grid`](https://github.com/Juicy/juicy-tile-grid).

## Features

[`juicy-tile-list` features](https://github.com/Juicy/juicy-tile-list#features), plus
 - `<table>` that mimics CSS Grid Layout,
 - awesome mix of fresh Web Components with lovely table layout from 90s :heart:,
 - adapting to dynamic changes of the element, parent container, and tile's content, without need to repack.

## Demo

[Check it live!](http://Juicy.github.io/juicy-tile-table)

### Rationale
As CSS Grid Layout is not yet widely supported across major browsers, without a need to set custom flags, and we would like to use all the features of [`<juicy-tile-grid>`](https://github.com/Juicy/juicy-tile-grid) on production, we have fallback to widely supported, good old `<table>`.
This custom element is layer on top of [`<juicy-tile-list>`](https://github.com/Juicy/juicy-tile-list) that renders packed items using specifically created `<table>`, so it leverages native way to handle tiles' overflow and container re-sizes, and should work in any browser.

## Install

Install the component using [Bower](http://bower.io/):

```sh
$ bower install juicy-tile-table --save
```

Or [download as ZIP](https://github.com/Juicy/juicy-tile-table/archive/gh-pages.zip).

## Usage

1. Import Web Components' polyfill, if needed:

    ```html
    <script src="bower_components/webcomponentsjs/webcomponents.js"></script>
    ```

2. Import Custom Element:

    ```html
    <link rel="import" href="bower_components/juicy-tile-table/juicy-tile-table.html">
    ```

3. Start using it!

    ```html
    <juicy-tile-table></juicy-tile-table>
    ```

## Requirements
This is custom element and it uses ShadowDOM, so if your environment does not support it, you may use [webcomponents/webcomponentsjs](https://github.com/webcomponents/webcomponentsjs) polyfill.

## Options

All [`juicy-tile-list` options](https://github.com/Juicy/juicy-tile-list#options), plus

Attribute                       | Options   | Default | Description
---                             | ---       | ---     | ---
`setup.items[.].heightFlexible` | *Boolean* | `false` | Let the tile's row adapt to the element/parent HTML element resize - grow/shrink proportionally, without a need to repack items.
`setup.items[.].widthFlexible`  | *Boolean* | `false` | Let the tile's column adapt to the element/parent HTML element resize - grow/shrink proportionally, without a need to repack items.
`setup.items[.].heightDynamic`  | *Boolean* | `false` | Let the tile's row adapt to the tile HTML element resize - grow/shrink accordingly, without a need to repack items.
`setup.items[.].widthDynamic`   | *Boolean* | `false` | Let the tile's column adapt to the tile HTML element resize - grow/shrink accordingly, without a need to repack items.

## [Properties](https://github.com/Juicy/juicy-tile-list#properties)

## [Methods](https://github.com/Juicy/juicy-tile-list#methods)

## [Events](https://github.com/Juicy/juicy-tile-list#events)

## [Tile ids](https://github.com/Juicy/juicy-tile-list#tile-ids)

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## History

For detailed changelog, check [Releases](https://github.com/Juicy/juicy-tile-table/releases).

## License

MIT
