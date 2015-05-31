Launcher's &lt;polyjuice-include&gt;
==============

This is Launcher's version of [`<polyjuice-include>`](https://github.com/Polyjuice/polyjuice-include).
Launcher overwrites it with is a custom element that not only let's you load HTML partials into your Polyjuice page with full control over loaded `<script>`s and `<link rel="import">`s, but also allows you to gently manage output from many apps mixed by Polyjuice mappings.

## Demo/Examples
:construction:
[Example in Starcounter Fiddle!]()

## Related custom elements

 - [`<polyjuice-include>`](https://github.com/Polyjuice/polyjuice-include) - includes Polyjuice Partial,
 - [`<imported-template>`](https://github.com/Juicy/imported-template) - includes HTML Template,
 - [`<juicy-tile-grid>`](https://github.com/Juicy/juicy-tile-grid),
 - [`<juicy-tile-list>`](https://github.com/Juicy/juicy-tile-list),
 - [`<juicy-tile-editor>`](https://github.com/Juicy/juicy-tile-editor),
 - [`<juicy-tiles-setup-sync>`](https://github.com/Juicy/juicy-tiles-setup-sync)

## Related wiki articles: 

 - https://github.com/Polyjuice/Launcher/wiki/Partials
 - https://github.com/Polyjuice/Launcher/wiki/Layout-setup
 - https://github.com/Polyjuice/Launcher/wiki/include-template-in-Polyjuice


## Install

[Launcher](https://github.com/Polyjuice/Launcher) has it already pre-installed, under `/sys/polyjuice-include/polyjuice-include.html` (overwrites Polyjuice's one), but if you want to use it separately as well.

Install the component using [Bower](http://bower.io/):

```sh
$ bower install Polyjuice/launcher-include --save
```

Or [download as ZIP](https://github.com/Polyjuice/launcher-include/archive/master.zip).

## Usage

1. Import Web Components' polyfill (if needed):

    ```html
    <script src="bower_components/webcomponentsjs/webcomponents.js">
    ```

2. Import Custom Element:

    ```html
    <link rel="import" href="bower_components/polyjuice-include/polyjuice-include.html">
    ```

3. Start using it!

    ```html
    <polyjuice-include partial="{{ViewModel}}"></polyjuice-include>
    ```
    or without mustache-style data-binding:
    ```js
    document.querySelector("polyjuice-include").partial = ViewModel;
    ```


## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## History

For detailed changelog, check [Releases](https://github.com/Polyjuice/launcher-include/releases).
