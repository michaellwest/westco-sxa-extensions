# Westco SXA Extensions

**Westco SXA Extensions** is a set of extensions for **Sitecore Experience Accelerator** module.

Solution follows [Helix](http://helix.sitecore.net/).

Current **Westco SXA Extensions** project is compatible with:

| Product   |      Version      |  Revision |
|----------|:-------------:|:------:|
| Sitecore |  **8.2** | rev. 171121 |
| SXA  |  **1.6** | rev. 180103 |

[![License](https://img.shields.io/badge/license-MIT%20License-brightgreen.svg)](https://opensource.org/licenses/MIT)

## Setup

Getting started is fairly straightforward. Install the package.

---

## Asset Include

Create a new tenant and ensure that the _Westco Theming_ feature is enabled.

![Imgur](https://i.imgur.com/cyOjzyk.png)

Create a new site and ensure that the _Westco Geospatial_ and _Westco Maps_ features are enabled. These two are similar to those OOTB with SXA but offer a Static Maps component.

![Imgur](https://i.imgur.com/deW28II.png)

### Assets served by a CDN with local fallback

In the _Media Library_ you simply create new **Assets** to represent your JavaScript and CSS resources from a CDN.

![Imgur](https://i.imgur.com/QAvZ7nz.png)

**Note:** The Url, SRI, and CORS values are all made available by the CDN.

You can associate assets at a site level by specifying on the **Settings** item in the **Asset Configuration** section.

![Imgur](https://i.imgur.com/ZJpBR9Y.png)

You can associate assets on a page level by specifying in the **Asset Configuration** section.

![Imgur](https://i.imgur.com/0W3jdoi.png)

Finally, see that the Html is injected in the `<head>` and `<body>`.

![Imgur](https://i.imgur.com/xNO4dy2.png)

The final Html output may look like the following:

```html
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.2.1/jquery.min.js" integrity="sha256-hwg4gsxgFZhOsEEamdOYGBf13FyQuiTwlAQgxVSNgt4=" crossorigin="anonymous"></script>
<script>window.jQuery || document.write('<script src="/-/media/Assets/jquery/jquery-3-2-1/Scripts/optimized-min.js?t=20170614T032244Z">\x3C/script>')</script>
<script>console.log('Run after CDN link and fallback test.');</script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.18.1/moment.min.js" integrity="sha256-1hjUhpc44NwiNg8OwMu2QzJXhD8kcj+sJA3aCQZoUjg=" crossorigin="anonymous"></script>
```

### Asset modes

The configured mode determines the type of html to generate.

- Disabled : Asset is ignored completely.
- Script : Asset and raw content rendered using `<script>`.
- ScriptAsync : Same as **Script** plus the `async` attribute.
- ScriptDefer : Same as **Script** the `defer` attribute.
- Style : Asset and raw content rendered using `<style>` and `<link>`.

### Url and SRI

The details needed for specifying the Url and SRI can be found on [cdnjs.com](https://cdnjs.com/libraries/jquery)

![Imgur](https://i.imgur.com/9AepjZa.png)

---

| [![Michael West](https://gravatar.com/avatar/a2914bafbdf4e967701eb4732bde01c5?s=220)](https://github.com/michaellwest) |
| --- |
| [Michael West](https://michaellwest.blogspot.com) |
