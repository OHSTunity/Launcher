launcher-display-router
======================

A Custom Element to switch visible elements based on given URI

### Usage

```html
<launcher-display-router>
  <launcher-display-route match-path="\/" uri="/" display-selector="body pj-dashboard #container"></launcher-display-route>
  <launcher-display-route match-path="\/super-crm" uri="/super-crm/people" display-selector="body pj-dashboard #workspaces .SuperCRM"></launcher-display-route>
</launcher-display-router>
```

Wrapper tags `<launcher-display-router>` are optional.

### Attributes

#### `match-path`

Specifies the regular expression that will be matched against current URI path (attribute or property)

#### `display-selector`

Specifies the CSS selector that will be used to show/hide an element.

#### `uri`

Attribute or property with current URI

### How it works

- For each **launcher-display-route**, its `uri` is checked against the regular expression provided in the `match-path` attribute.
- If there is a match, the query in `display-selector` is performed against DOM and the first found element is set with `display: block`.
- If there is no match, the query in `display-selector` is performed against DOM and the first found element is set with `display: none`.

### License

MIT
