# &lt;juicy-tiles-setup-sync&gt;

> Polymer Element to handle `<juicy-tile-list>` (or `-grid`) setup changes (staging, reverting, saving to storage).
## Demo

[Check it live!](http://Juicy.github.io/juicy-tiles-setup-sync)

## Install

Install the component using [Bower](http://bower.io/):

```sh
$ bower install juicy-tiles-setup-sync --save
```

Or [download as ZIP](https://github.com/Juicy/juicy-tiles-setup-sync/archive/master.zip).

## Usage

1. Import Web Components' polyfill:

    ```html
    <script src="bower_components/platform/platform.js"></script>
    ```

2. Import Custom Element:

    ```html
    <link rel="import" href="bower_components/juicy-tiles-setup-sync/src/juicy-tiles-setup-sync.html">
    ```

3. Start using it!

    ```html
    <juicy-tiles-setup-sync></juicy-tiles-setup-sync>
    <juicy-tile-list>...
    ```
    :exclamation: Please make sure to placed it as direct previous sibling :couple: of the applicable `<juicy-tile-list>`

## Attributes

Attribute     | Options     | Default      | Description
---           | ---         | ---          | ---
`storedValue` | *Object*    |              | [`<juicy-tile-list>.setup`](https://github.com/Juicy/juicy-tile-list#options)

## Properties

Property      | Type              | Description
---           | ---               | ---
`tileList`    | *juicy-tile-list* | Associated `<juicy-tile-list>`
`storedValue` | *Object*          | [`<juicy-tile-list>.setup`](https://github.com/Juicy/juicy-tile-list#options)

## Methods

Method        | Parameters | Returns   | Description
---           | ---        | ---       | ---
`save()`      | None.      | self      | Saves current setup value of associated `<juicy-tile-list>`
`revert()`    | None.      | self      | Reverts current stored setup into `<juicy-tile-list>.setup`
`clear()`     | None.      | self      | Clears any storedValue
`isModified()`| None.      | *Boolean* | `true` if current setup differs from stored one.



## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## History

For detailed changelog, check [Releases](https://github.com/Juicy/juicy-tiles-setup-sync/releases).

## License

[MIT License](http://opensource.org/licenses/MIT)
